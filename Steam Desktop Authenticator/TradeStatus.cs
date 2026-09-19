using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    // Steam tells only the account itself why it cannot trade, and only on the pages where a trade would start,
    // so those are fetched with the account's own session and searched for whatever Steam says about trading
    static class TradeStatus
    {
        private static readonly string[] Pages =
        {
            APIEndpoints.COMMUNITY_BASE + "/tradeoffer/new/?partner=22202",
            APIEndpoints.COMMUNITY_BASE + "/my/tradeoffers/privacy",
            APIEndpoints.COMMUNITY_BASE + "/my/tradeoffers/"
        };

        private static readonly Regex Keywords = new Regex(@"unable to trade|cannot trade|can't trade|not able to trade|able to trade (again|after|on)|trade hold|trade restrict|restricted from trading|trading privileges|trade ban|new device|Steam Guard", RegexOptions.IgnoreCase);

        public static async Task<List<string>> CheckAsync(SteamGuardAccount account)
        {
            var cookies = account.Session.GetCookies();
            var found = new List<string>();
            foreach (string url in Pages)
            {
                string html;
                try
                {
                    html = await SteamWeb.GETRequest(url, cookies);
                }
                catch (WebException ex)
                {
                    Log.Write("Trade status page " + url + " failed: " + ex.Message);
                    continue;
                }
                foreach (string line in Extract(html))
                    if (!found.Contains(line)) found.Add(line);
            }

            Log.Write("Trade status for " + account.AccountName + ": " + (found.Count == 0 ? "nothing about trading on the pages" : string.Join(" || ", found)));
            return found;
        }

        // The error box on the trade page first, then any sentence on the page that talks about trading
        internal static List<string> Extract(string html)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(html)) return lines;

            var error = Regex.Match(html, @"<div[^>]*id=""error_msg""[^>]*>(.*?)</div>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (error.Success)
            {
                string text = Clean(error.Groups[1].Value);
                if (text.Length > 0) lines.Add(text);
            }

            string body = Regex.Replace(html, @"<(script|style)[^>]*>.*?</\1>", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            body = Clean(body);
            foreach (string sentence in Regex.Split(body, @"(?<=[.!?])\s+"))
            {
                string s = sentence.Trim();
                if (s.Length < 12 || s.Length > 300 || !Keywords.IsMatch(s)) continue;
                if (s.IndexOf("Privacy Policy", StringComparison.OrdinalIgnoreCase) >= 0 || s.IndexOf("Legal", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (!lines.Contains(s)) lines.Add(s);
                if (lines.Count >= 6) break;
            }
            return lines;
        }

        private static string Clean(string html)
        {
            string text = Regex.Replace(html, @"<br\s*/?>|</p>|</li>|</div>", " ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<[^>]+>", " ");
            text = WebUtility.HtmlDecode(text);
            return Regex.Replace(text, @"\s+", " ").Trim();
        }
    }
}
