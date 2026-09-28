using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    class TradeOfferInfo
    {
        public ulong Partner;
        public int Giving;
        public int Receiving;
    }

    class TradeItem
    {
        public string AppId;
        public string ClassId;
        public string InstanceId;
        public string IconUrl;
        public int Amount = 1;
    }

    class TradeOfferItems
    {
        public ulong Partner;
        public List<TradeItem> Mine = new List<TradeItem>();
        public List<TradeItem> Theirs = new List<TradeItem>();
    }

    // Reads the page behind a confirmation, for the auto accept rules and for showing what is in it
    static class TradeOffers
    {
        private const ulong SteamIdBase = 76561197960265728UL;

        public static async Task<string> DetailsAsync(SteamGuardAccount account, Confirmation confirmation)
        {
            try
            {
                string url = APIEndpoints.COMMUNITY_BASE + "/mobileconf/details/" + confirmation.ID + "?" + account.GenerateConfirmationQueryParams("details");
                string response = await SteamWeb.GETRequest(url, account.Session.GetCookies());
                if (response == null) return null;

                var json = JObject.Parse(response);
                if (json.Value<bool?>("success") != true) return null;
                return json.Value<string>("html");
            }
            catch (Exception ex)
            {
                Log.Error("Details of confirmation " + confirmation.ID + " for " + account.AccountName, ex);
                return null;
            }
        }

        public static async Task<TradeOfferInfo> ReadAsync(SteamGuardAccount account, Confirmation confirmation)
        {
            return Parse(await DetailsAsync(account, confirmation), account.Session.SteamID);
        }

        // Returns null whenever the page does not look the way it should, callers treat that as "do not touch"
        internal static TradeOfferInfo Parse(string html, ulong mySteamId)
        {
            var items = ParseItems(html, mySteamId);
            if (items == null) return null;
            return new TradeOfferInfo
            {
                Partner = items.Partner,
                Giving = items.Mine.Sum(i => i.Amount),
                Receiving = items.Theirs.Sum(i => i.Amount)
            };
        }

        // The two item lists of the offer, sorted into ours and theirs by the profile link on each side
        internal static TradeOfferItems ParseItems(string html, ulong mySteamId)
        {
            if (string.IsNullOrEmpty(html)) return null;

            var blocks = Regex.Matches(html, @"class=""tradeoffer_items\s+(primary|secondary)[\s""]");
            if (blocks.Count != 2) return null;

            var sides = new List<Tuple<ulong, List<TradeItem>>>();
            for (int i = 0; i < blocks.Count; i++)
            {
                int start = blocks[i].Index;
                int end = i + 1 < blocks.Count ? blocks[i + 1].Index : html.Length;
                string block = html.Substring(start, end - start);
                sides.Add(Tuple.Create(OwnerOf(block), ItemsIn(block)));
            }

            // The side that carries our own profile is ours. When neither side names anyone, Steam puts the
            // sender first and confirmations are for offers we sent, so the primary block is ours.
            int mine = sides[0].Item1 == mySteamId ? 0 : sides[1].Item1 == mySteamId ? 1 : -1;
            if (mine < 0)
            {
                if (sides[0].Item1 != 0 && sides[1].Item1 != 0) return null;
                mine = blocks[0].Groups[1].Value == "primary" ? 0 : 1;
            }
            if (sides[0].Item1 != 0 && sides[0].Item1 == sides[1].Item1) return null;

            return new TradeOfferItems
            {
                Partner = sides[1 - mine].Item1,
                Mine = sides[mine].Item2,
                Theirs = sides[1 - mine].Item2
            };
        }

        private static ulong OwnerOf(string block)
        {
            var mini = Regex.Match(block, @"data-miniprofile=""(\d+)""");
            if (mini.Success) return SteamIdBase + ulong.Parse(mini.Groups[1].Value);
            var link = Regex.Match(block, @"steamcommunity\.com/profiles/(\d{17})");
            if (link.Success) return ulong.Parse(link.Groups[1].Value);
            return 0;
        }

        private static List<TradeItem> ItemsIn(string block)
        {
            var items = new List<TradeItem>();
            var starts = Regex.Matches(block, @"<(?:div|a)[^>]*class=""[^""]*\btrade_item\b[^""]*""[^>]*>");
            for (int i = 0; i < starts.Count; i++)
            {
                string tag = starts[i].Value;
                int end = i + 1 < starts.Count ? starts[i + 1].Index : block.Length;
                string inner = block.Substring(starts[i].Index, end - starts[i].Index);

                var item = new TradeItem();
                var economy = Regex.Match(tag, @"data-economy-item=""classinfo/(\d+)/(\d+)(?:/(\d+))?""");
                if (economy.Success)
                {
                    item.AppId = economy.Groups[1].Value;
                    item.ClassId = economy.Groups[2].Value;
                    item.InstanceId = economy.Groups[3].Success ? economy.Groups[3].Value : "";
                }
                var image = Regex.Match(inner, @"<img[^>]*src=""([^""]+)""");
                if (image.Success) item.IconUrl = WebUtility.HtmlDecode(image.Groups[1].Value);
                else
                {
                    var background = Regex.Match(inner, @"url\(\s*'?([^')]+economy/image[^')]+)'?\s*\)");
                    if (background.Success) item.IconUrl = WebUtility.HtmlDecode(background.Groups[1].Value);
                }
                var amount = Regex.Match(inner, @"class=""item_amount[^""]*""[^>]*>\s*([\d,]+)");
                int count;
                if (amount.Success && int.TryParse(amount.Groups[1].Value.Replace(",", ""), out count) && count > 0) item.Amount = count;

                // Copies of one item share an entry, sixty keys are one line with an amount of sixty
                var same = item.ClassId == null ? null : items.FirstOrDefault(o => o.AppId == item.AppId && o.ClassId == item.ClassId && o.InstanceId == item.InstanceId);
                if (same != null) same.Amount += item.Amount;
                else items.Add(item);
            }
            return items;
        }

        // What the page says, as plain lines, for confirmations that are not trades
        internal static List<string> TextLines(string html)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(html)) return lines;

            string text = Regex.Replace(html, @"<(script|style)[^>]*>.*?</\1>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<br\s*/?>|</?(div|p|li|tr|h\d|ul|ol|table)\b[^>]*>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<[^>]+>", " ");
            text = WebUtility.HtmlDecode(text);

            foreach (string raw in text.Split('\n'))
            {
                string line = Regex.Replace(raw, @"\s+", " ").Trim();
                if (line.Length > 0 && (lines.Count == 0 || lines[lines.Count - 1] != line))
                    lines.Add(line);
            }
            return lines;
        }
    }
}
