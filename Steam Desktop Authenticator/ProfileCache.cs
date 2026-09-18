using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    class ProfileCache
    {
        private static readonly HttpClient http = new HttpClient() { Timeout = TimeSpan.FromSeconds(15) };
        private static readonly string avatarDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam Desktop Authenticator 2", "avatars");

        private readonly Dictionary<ulong, Image> avatars = new Dictionary<ulong, Image>();
        private bool refreshing;

        public event Action Updated;
        public event Action<SteamGuardAccount, string> Warning;

        static ProfileCache()
        {
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Steam Desktop Authenticator 2");
        }

        public Image GetAvatar(ulong steamId)
        {
            Image image;
            if (avatars.TryGetValue(steamId, out image))
                return image;

            string file = Path.Combine(avatarDir, steamId + ".jpg");
            if (!File.Exists(file))
                return null;

            try
            {
                image = LoadImage(File.ReadAllBytes(file));
            }
            catch (Exception)
            {
                image = null;
            }
            avatars[steamId] = image;
            return image;
        }

        public async Task RefreshAsync(Manifest manifest, SteamGuardAccount[] accounts)
        {
            if (refreshing || accounts == null) return;
            refreshing = true;

            try
            {
                bool changed = false;
                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                foreach (var account in accounts)
                {
                    var entry = manifest.GetEntry(account);
                    if (entry == null) continue;

                    ulong steamId = account.Session.SteamID;
                    bool stale = string.IsNullOrEmpty(entry.PersonaName) || now - entry.ProfileUpdated > 86400;
                    if (!stale && GetAvatar(steamId) != null) continue;

                    try
                    {
                        string xml = await http.GetStringAsync("https://steamcommunity.com/profiles/" + steamId + "?xml=1");
                        var profile = XDocument.Parse(xml).Root;
                        string name = (string)profile.Element("steamID");
                        string avatarUrl = (string)profile.Element("avatarFull");
                        if (string.IsNullOrEmpty(name)) continue;

                        if (avatarUrl != entry.AvatarUrl || GetAvatar(steamId) == null)
                        {
                            byte[] bytes = await http.GetByteArrayAsync(avatarUrl);
                            Directory.CreateDirectory(avatarDir);
                            File.WriteAllBytes(Path.Combine(avatarDir, steamId + ".jpg"), bytes);
                            avatars[steamId] = LoadImage(bytes);
                        }

                        bool first = entry.ProfileUpdated == 0;
                        entry.PersonaName = name;
                        entry.AvatarUrl = avatarUrl;
                        entry.ProfileUpdated = now;

                        // Game bans are only on the html profile, everything else is in the xml
                        string tradeBan = (string)profile.Element("tradeBanState") ?? "None";
                        bool vac = (string)profile.Element("vacBanned") == "1";
                        bool limited = (string)profile.Element("isLimitedAccount") == "1";
                        int gameBans = entry.GameBans;
                        try
                        {
                            gameBans = ParseGameBans(await http.GetStringAsync("https://steamcommunity.com/profiles/" + steamId));
                        }
                        catch (Exception)
                        {
                        }

                        string alert = null;
                        if (tradeBan != "None" && tradeBan != (entry.TradeBan ?? "None"))
                            alert = tradeBan == "Probation" ? "Steam put a trade ban probation on " : "Steam trade banned ";
                        else if (vac && !entry.VacBanned)
                            alert = "Steam recorded a VAC ban on ";
                        else if (gameBans > entry.GameBans)
                            alert = "Steam recorded a game ban on ";

                        entry.TradeBan = tradeBan;
                        entry.VacBanned = vac;
                        entry.GameBans = gameBans;
                        entry.LimitedAccount = limited;
                        changed = true;

                        // Old bans are shown in the account card, only a fresh one is worth a notification
                        if (alert != null && !first)
                            Warning?.Invoke(account, alert + manifest.GetDisplayName(account) + ".");
                    }
                    catch (Exception)
                    {
                        // Profile lookups are cosmetic, try again next time
                    }
                }

                if (changed)
                {
                    manifest.Save();
                    Updated?.Invoke();
                }
            }
            finally
            {
                refreshing = false;
            }
        }

        // The profile shows "1 game ban on record" or "Multiple game bans on record" above the days since the last one
        internal static int ParseGameBans(string html)
        {
            if (string.IsNullOrEmpty(html)) return 0;
            var match = Regex.Match(html, @"(\d+|Multiple)\s+game\s+bans?\s+on\s+record", RegexOptions.IgnoreCase);
            if (!match.Success) return 0;
            int count;
            return int.TryParse(match.Groups[1].Value, out count) ? count : 2;
        }

        private static Image LoadImage(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return new Bitmap(stream);
        }
    }
}
