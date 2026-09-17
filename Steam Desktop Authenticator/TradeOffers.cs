using System;
using System.Collections.Generic;
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

    // Reads the trade offer behind a confirmation so auto accept rules can look at it
    static class TradeOffers
    {
        private const ulong SteamIdBase = 76561197960265728UL;

        public static async Task<TradeOfferInfo> ReadAsync(SteamGuardAccount account, Confirmation confirmation)
        {
            try
            {
                string url = APIEndpoints.COMMUNITY_BASE + "/mobileconf/details/" + confirmation.ID + "?" + account.GenerateConfirmationQueryParams("details");
                string response = await SteamWeb.GETRequest(url, account.Session.GetCookies());
                if (response == null) return null;

                var json = JObject.Parse(response);
                if (json.Value<bool?>("success") != true) return null;

                return Parse(json.Value<string>("html"), account.Session.SteamID);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Returns null whenever the page does not look the way it should, callers treat that as "do not touch"
        internal static TradeOfferInfo Parse(string html, ulong mySteamId)
        {
            if (string.IsNullOrEmpty(html)) return null;

            var blocks = Regex.Matches(html, @"class=""tradeoffer_items\s+(primary|secondary)[\s""]");
            if (blocks.Count != 2) return null;

            var sides = new List<Tuple<ulong, int>>();
            for (int i = 0; i < blocks.Count; i++)
            {
                int start = blocks[i].Index;
                int end = i + 1 < blocks.Count ? blocks[i + 1].Index : html.Length;
                string block = html.Substring(start, end - start);

                var profile = Regex.Match(block, @"data-miniprofile=""(\d+)""");
                if (!profile.Success) return null;
                ulong steamId = SteamIdBase + ulong.Parse(profile.Groups[1].Value);
                int items = Regex.Matches(block, @"class=""trade_item[\s""]").Count;
                sides.Add(Tuple.Create(steamId, items));
            }

            if (sides[0].Item1 == sides[1].Item1) return null;
            int mine = sides[0].Item1 == mySteamId ? 0 : sides[1].Item1 == mySteamId ? 1 : -1;
            if (mine < 0) return null;

            return new TradeOfferInfo
            {
                Partner = sides[1 - mine].Item1,
                Giving = sides[mine].Item2,
                Receiving = sides[1 - mine].Item2
            };
        }
    }
}
