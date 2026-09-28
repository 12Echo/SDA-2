using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Steam_Desktop_Authenticator
{
    class ItemInfo
    {
        public string Name;
        public string Type;
        public string IconUrl;
        public string Color;
    }

    // Names and icons for economy items, from the public hover endpoint the Steam site itself uses
    static class Economy
    {
        private static readonly HttpClient http = new HttpClient();
        private static readonly Dictionary<string, Task<ItemInfo>> cache = new Dictionary<string, Task<ItemInfo>>();
        private static readonly Regex hover = new Regex(@"BuildHover\(\s*'[^']*',\s*(\{.*\})\s*\);", RegexOptions.Singleline);

        static Economy()
        {
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            http.Timeout = TimeSpan.FromSeconds(20);
        }

        public static string ImageUrl(string iconHash, int size)
        {
            if (string.IsNullOrEmpty(iconHash)) return null;
            return "https://community.fastly.steamstatic.com/economy/image/" + iconHash + "/" + size + "fx" + size + "f";
        }

        public static Task<ItemInfo> GetAsync(string appId, string classId, string instanceId)
        {
            string key = appId + "/" + classId + "/" + instanceId;
            lock (cache)
            {
                Task<ItemInfo> pending;
                if (!cache.TryGetValue(key, out pending))
                {
                    pending = FetchAsync(appId, classId, instanceId);
                    cache[key] = pending;
                }
                return pending;
            }
        }

        private static async Task<ItemInfo> FetchAsync(string appId, string classId, string instanceId)
        {
            try
            {
                string url = "https://steamcommunity.com/economy/itemclasshover/" + appId + "/" + classId
                    + (string.IsNullOrEmpty(instanceId) ? "" : "/" + instanceId) + "?content_only=1&l=english";
                string body = await http.GetStringAsync(url);
                var match = hover.Match(body);
                if (!match.Success) return null;

                var json = JObject.Parse(match.Groups[1].Value);
                string name = json.Value<string>("market_name");
                if (string.IsNullOrEmpty(name)) name = json.Value<string>("name");
                if (string.IsNullOrEmpty(name)) return null;

                return new ItemInfo
                {
                    Name = name,
                    Type = json.Value<string>("type"),
                    IconUrl = ImageUrl(json.Value<string>("icon_url"), 64),
                    Color = json.Value<string>("name_color")
                };
            }
            catch (Exception ex)
            {
                Log.Error("Item lookup " + appId + "/" + classId, ex);
                lock (cache) cache.Remove(appId + "/" + classId + "/" + instanceId);
                return null;
            }
        }
    }
}
