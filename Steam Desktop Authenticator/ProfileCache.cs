using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
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

                        entry.PersonaName = name;
                        entry.AvatarUrl = avatarUrl;
                        entry.ProfileUpdated = now;
                        changed = true;
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

        private static Image LoadImage(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
                return new Bitmap(stream);
        }
    }
}
