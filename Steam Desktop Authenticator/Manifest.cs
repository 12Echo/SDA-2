using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SteamAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public enum ConfirmationMode
    {
        Off,
        Periodic,
        Live
    }

    public enum NotificationStyle
    {
        Windows,
        Popup
    }

    public class Manifest
    {
        [JsonProperty("encrypted")]
        public bool Encrypted { get; set; }

        [JsonProperty("first_run")]
        public bool FirstRun { get; set; } = true;

        [JsonProperty("entries")]
        public List<ManifestEntry> Entries { get; set; }

        [JsonProperty("periodic_checking")]
        public bool PeriodicChecking { get; set; } = false;

        [JsonProperty("periodic_checking_interval")]
        public int PeriodicCheckingInterval { get; set; } = 5;

        [JsonProperty("periodic_checking_checkall")]
        public bool CheckAllAccounts { get; set; } = false;

        [JsonProperty("auto_confirm_market_transactions")]
        public bool AutoConfirmMarketTransactions { get; set; } = false;

        [JsonProperty("auto_confirm_trades")]
        public bool AutoConfirmTrades { get; set; } = false;

        [JsonProperty("live_notifications")]
        public bool LiveNotifications { get; set; } = false;

        [JsonProperty("start_with_windows")]
        public bool StartWithWindows { get; set; } = false;

        [JsonProperty("start_minimized")]
        public bool StartMinimized { get; set; } = false;

        [JsonProperty("notification_style")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NotificationStyle NotificationStyle { get; set; } = NotificationStyle.Windows;

        [JsonProperty("check_updates")]
        public bool CheckUpdates { get; set; } = true;

        [JsonProperty("language")]
        public string Language { get; set; } = "";

        [JsonProperty("lock_after_minutes")]
        public int LockAfterMinutes { get; set; } = 0;

        [JsonProperty("last_backup")]
        public long LastBackup { get; set; }

        [JsonProperty("backup_reminded")]
        public long BackupReminded { get; set; }

        [JsonProperty("list_group")]
        public string ListGroup { get; set; } = "";

        private static Manifest _manifest { get; set; }

        public static string GetExecutableDir()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public static Manifest GetManifest(bool forceLoad = false)
        {
            // Check if already staticly loaded
            if (_manifest != null && !forceLoad)
            {
                return _manifest;
            }

            // Find config dir and manifest file
            string maDir = Manifest.GetExecutableDir() + "/maFiles/";
            string manifestFile = maDir + "manifest.json";

            // If there's no config dir, create it
            if (!Directory.Exists(maDir))
            {
                _manifest = GenerateNewManifest(false);
                return _manifest;
            }

            // If there's no manifest, throw exception
            if (!File.Exists(manifestFile))
            {
                throw new ManifestParseException();
            }

            try
            {
                string manifestContents = File.ReadAllText(manifestFile);
                _manifest = JsonConvert.DeserializeObject<Manifest>(manifestContents);

                if (_manifest.Encrypted && _manifest.Entries.Count == 0)
                {
                    _manifest.Encrypted = false;
                    _manifest.Save();
                }

                _manifest.RecomputeExistingEntries();
                _manifest.MigrateSettings();
                _manifest.AdoptLooseFiles();

                return _manifest;
            }
            catch (Exception)
            {
                throw new ManifestParseException();
            }
        }

        public static Manifest GenerateNewManifest(bool scanDir = false)
        {
            // No directory means no manifest file anyways.
            Manifest newManifest = new Manifest();
            newManifest.Encrypted = false;
            newManifest.PeriodicCheckingInterval = 5;
            newManifest.PeriodicChecking = false;
            newManifest.AutoConfirmMarketTransactions = false;
            newManifest.AutoConfirmTrades = false;
            newManifest.LiveNotifications = false;
            newManifest.Entries = new List<ManifestEntry>();
            newManifest.FirstRun = true;

            // Take a pre-manifest version and generate a manifest for it.
            if (scanDir)
            {
                string maDir = Manifest.GetExecutableDir() + "/maFiles/";
                if (Directory.Exists(maDir))
                {
                    DirectoryInfo dir = new DirectoryInfo(maDir);
                    var files = dir.GetFiles();

                    foreach (var file in files)
                    {
                        if (file.Extension != ".maFile") continue;

                        string contents = File.ReadAllText(file.FullName);
                        try
                        {
                            SteamGuardAccount account = JsonConvert.DeserializeObject<SteamGuardAccount>(contents);
                            ManifestEntry newEntry = new ManifestEntry()
                            {
                                Filename = file.Name,
                                SteamID = account.Session.SteamID
                            };
                            newManifest.Entries.Add(newEntry);
                        }
                        catch (Exception)
                        {
                            throw new MaFileEncryptedException();
                        }
                    }

                    if (newManifest.Entries.Count > 0)
                    {
                        newManifest.Save();
                        newManifest.PromptSetupPassKey("This version of SDA has encryption. Please enter a passkey below, or hit cancel to remain unencrypted");
                    }
                }
            }

            if (newManifest.Save())
            {
                _manifest = newManifest;
                return newManifest;
            }

            return null;
        }

        // maFiles dropped into the folder by hand, or left behind by an interrupted import, are picked up
        private void AdoptLooseFiles()
        {
            if (this.Encrypted) return;
            string maDir = Manifest.GetExecutableDir() + "/maFiles/";
            if (!Directory.Exists(maDir)) return;

            bool added = false;
            foreach (string file in Directory.GetFiles(maDir, "*.maFile"))
            {
                string name = Path.GetFileName(file);
                if (this.Entries.Any(e => string.Equals(e.Filename, name, StringComparison.OrdinalIgnoreCase))) continue;
                try
                {
                    var account = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(file));
                    if (account?.Session == null || account.Session.SteamID == 0 || string.IsNullOrEmpty(account.SharedSecret)) continue;
                    if (this.Entries.Any(e => e.SteamID == account.Session.SteamID)) continue;
                    this.Entries.Add(new ManifestEntry { Filename = name, SteamID = account.Session.SteamID });
                    added = true;
                }
                catch (Exception)
                {
                }
            }
            if (added) this.Save();
        }

        // Confirmation settings used to be global, they are per account now
        private void MigrateSettings()
        {
            if (!this.PeriodicChecking && !this.LiveNotifications) return;

            foreach (var entry in this.Entries)
            {
                entry.Confirmations = this.LiveNotifications ? ConfirmationMode.Live : ConfirmationMode.Periodic;
                entry.CheckInterval = Math.Max(5, this.PeriodicCheckingInterval);
                entry.AutoConfirmTrades = this.AutoConfirmTrades;
                entry.AutoConfirmMarket = this.AutoConfirmMarketTransactions;
            }

            this.PeriodicChecking = false;
            this.LiveNotifications = false;
            this.Save();
        }

        public ManifestEntry GetEntry(SteamGuardAccount account)
        {
            return this.Entries.FirstOrDefault(e => e.SteamID == account.Session.SteamID);
        }

        // The name shown for an account: whatever the user chose, else the Steam persona, else the login
        public string GetDisplayName(SteamGuardAccount account)
        {
            var entry = GetEntry(account);
            if (!string.IsNullOrEmpty(entry?.DisplayName)) return entry.DisplayName;
            if (!string.IsNullOrEmpty(entry?.PersonaName)) return entry.PersonaName;
            return account.AccountName;
        }

        private static string BackupDir
        {
            get { return Manifest.GetExecutableDir() + "/maFiles.backup/"; }
        }

        // Left behind only when an encryption change was interrupted before it could finish
        public static bool HasBackup()
        {
            return Directory.Exists(BackupDir) && File.Exists(BackupDir + "manifest.json");
        }

        public static void RestoreBackup()
        {
            CopyMaFiles(BackupDir, Manifest.GetExecutableDir() + "/maFiles/");
            Directory.Delete(BackupDir, true);
            _manifest = null;
        }

        public static void DiscardBackup()
        {
            if (Directory.Exists(BackupDir))
                Directory.Delete(BackupDir, true);
        }

        private static void CopyMaFiles(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (string file in Directory.GetFiles(from))
            {
                string name = Path.GetFileName(file);
                if (name == "manifest.json" || name.EndsWith(".maFile", StringComparison.OrdinalIgnoreCase))
                    File.Copy(file, Path.Combine(to, name), true);
            }
        }

        public class IncorrectPassKeyException : Exception { }
        public class ManifestNotEncryptedException : Exception { }

        // recover runs when the user says the passkey is lost, and returns true once the install no longer needs it
        public string PromptForPassKey(Func<bool> recover = null)
        {
            if (!this.Encrypted)
            {
                throw new ManifestNotEncryptedException();
            }

            bool passKeyValid = false;
            string passKey = null;
            while (!passKeyValid)
            {
                InputForm passKeyForm = new InputForm("Please enter your encryption passkey.", true);
                if (recover != null)
                    passKeyForm.ShowExtra("Forgot passkey?");
                passKeyForm.ShowDialog();
                if (passKeyForm.ExtraClicked)
                {
                    if (recover()) return null;
                    continue;
                }
                if (!passKeyForm.Canceled)
                {
                    passKey = passKeyForm.txtBox.Text;
                    passKeyValid = passKey.Length > 0 && this.VerifyPasskey(passKey);
                    if (!passKeyValid)
                    {
                        MessageForm.Show(passKey.Length == 0 ? "Enter your passkey, or press Cancel to close SDA." : "That passkey is invalid.");
                    }
                }
                else
                {
                    return null;
                }
            }
            return passKey;
        }

        public string PromptSetupPassKey(string initialPrompt = "Enter passkey, or hit cancel to remain unencrypted.")
        {
            InputForm newPassKeyForm = new InputForm(initialPrompt, true);
            newPassKeyForm.ShowDialog();
            if (newPassKeyForm.Canceled || newPassKeyForm.txtBox.Text.Length == 0)
            {
                MessageForm.Show("WARNING: You chose to not encrypt your files. Doing so imposes a security risk for yourself. If an attacker were to gain access to your computer, they could completely lock you out of your account and steal all your items.");
                return null;
            }

            InputForm newPassKeyForm2 = new InputForm("Confirm new passkey.", true);
            newPassKeyForm2.ShowDialog();
            if (newPassKeyForm2.Canceled)
            {
                MessageForm.Show("WARNING: You chose to not encrypt your files. Doing so imposes a security risk for yourself. If an attacker were to gain access to your computer, they could completely lock you out of your account and steal all your items.");
                return null;
            }

            string newPassKey = newPassKeyForm.txtBox.Text;
            string confirmPassKey = newPassKeyForm2.txtBox.Text;

            if (newPassKey != confirmPassKey)
            {
                MessageForm.Show("Passkeys do not match.");
                return null;
            }

            if (!this.ChangeEncryptionKey(null, newPassKey))
            {
                MessageForm.Show("Unable to set passkey.");
                return null;
            }
            else
            {
                MessageForm.Show("Passkey successfully set.");
            }

            return newPassKey;
        }

        public SteamAuth.SteamGuardAccount[] GetAllAccounts(string passKey = null, int limit = -1)
        {
            if (passKey == null && this.Encrypted) return new SteamGuardAccount[0];
            string maDir = Manifest.GetExecutableDir() + "/maFiles/";

            List<SteamAuth.SteamGuardAccount> accounts = new List<SteamAuth.SteamGuardAccount>();
            foreach (var entry in this.Entries)
            {
                if (!File.Exists(maDir + entry.Filename)) continue;
                string fileText = File.ReadAllText(maDir + entry.Filename);
                if (this.Encrypted)
                {
                    string decryptedText = FileEncryptor.DecryptData(passKey, entry.Salt, entry.IV, fileText);
                    if (decryptedText == null) return new SteamGuardAccount[0];
                    fileText = decryptedText;
                }

                var account = JsonConvert.DeserializeObject<SteamAuth.SteamGuardAccount>(fileText);
                if (account == null) continue;
                accounts.Add(account);

                if (limit != -1 && limit >= accounts.Count)
                    break;
            }

            return accounts.ToArray();
        }

        public bool ChangeEncryptionKey(string oldKey, string newKey)
        {
            if (this.Encrypted)
            {
                if (!this.VerifyPasskey(oldKey))
                {
                    return false;
                }
            }
            bool toEncrypt = newKey != null;
            string maDir = Manifest.GetExecutableDir() + "/maFiles/";

            // Every file is rewritten, so keep a copy until the new set is proven readable
            var previous = this.Entries.Select(e => new ManifestEntry { SteamID = e.SteamID, Salt = e.Salt, IV = e.IV }).ToList();
            bool wasEncrypted = this.Encrypted;
            try
            {
                DiscardBackup();
                CopyMaFiles(maDir, BackupDir);

                for (int i = 0; i < this.Entries.Count; i++)
                {
                    ManifestEntry entry = this.Entries[i];
                    string filename = maDir + entry.Filename;
                    if (!File.Exists(filename)) continue;

                    string fileContents = File.ReadAllText(filename);
                    if (wasEncrypted)
                    {
                        fileContents = FileEncryptor.DecryptData(oldKey, entry.Salt, entry.IV, fileContents);
                        if (fileContents == null) throw new InvalidOperationException("Could not decrypt " + entry.Filename);
                    }

                    string newSalt = null;
                    string newIV = null;
                    string toWriteFileContents = fileContents;

                    if (toEncrypt)
                    {
                        newSalt = FileEncryptor.GetRandomSalt();
                        newIV = FileEncryptor.GetInitializationVector();
                        toWriteFileContents = FileEncryptor.EncryptData(newKey, newSalt, newIV, fileContents);
                        if (toWriteFileContents == null) throw new InvalidOperationException("Could not encrypt " + entry.Filename);
                    }

                    File.WriteAllText(filename, toWriteFileContents);
                    entry.IV = newIV;
                    entry.Salt = newSalt;
                }

                this.Encrypted = toEncrypt;
                if (!this.Save()) throw new InvalidOperationException("Could not save the manifest");

                var check = this.GetAllAccounts(newKey);
                if (check.Length != this.Entries.Count) throw new InvalidOperationException("The rewritten files do not read back");

                DiscardBackup();
                return true;
            }
            catch (Exception)
            {
                try
                {
                    CopyMaFiles(BackupDir, maDir);
                    DiscardBackup();
                }
                catch (Exception)
                {
                }
                foreach (var entry in this.Entries)
                {
                    var old = previous.FirstOrDefault(e => e.SteamID == entry.SteamID);
                    if (old == null) continue;
                    entry.Salt = old.Salt;
                    entry.IV = old.IV;
                }
                this.Encrypted = wasEncrypted;
                this.Save();
                return false;
            }
        }

        public bool VerifyPasskey(string passkey)
        {
            if (!this.Encrypted || this.Entries.Count == 0) return true;

            var accounts = this.GetAllAccounts(passkey, 1);
            return accounts != null && accounts.Length == 1;
        }

        public bool RemoveAccount(SteamGuardAccount account, bool deleteMaFile = true)
        {
            ManifestEntry entry = (from e in this.Entries where e.SteamID == account.Session.SteamID select e).FirstOrDefault();
            if (entry == null) return true; // If something never existed, did you do what they asked?

            string maDir = Manifest.GetExecutableDir() + "/maFiles/";
            string filename = maDir + entry.Filename;
            this.Entries.Remove(entry);

            if (this.Entries.Count == 0)
            {
                this.Encrypted = false;
            }

            if (this.Save() && deleteMaFile)
            {
                try
                {
                    File.Delete(filename);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }

        public bool SaveAccount(SteamGuardAccount account, bool encrypt, string passKey = null)
        {
            if (encrypt && String.IsNullOrEmpty(passKey)) return false;
            if (!encrypt && this.Encrypted) return false;

            string salt = null;
            string iV = null;
            string jsonAccount = JsonConvert.SerializeObject(account);

            if (encrypt)
            {
                salt = FileEncryptor.GetRandomSalt();
                iV = FileEncryptor.GetInitializationVector();
                string encrypted = FileEncryptor.EncryptData(passKey, salt, iV, jsonAccount);
                if (encrypted == null) return false;
                jsonAccount = encrypted;
            }

            string maDir = Manifest.GetExecutableDir() + "/maFiles/";
            string filename = account.Session.SteamID.ToString() + ".maFile";

            ManifestEntry newEntry = new ManifestEntry()
            {
                SteamID = account.Session.SteamID,
                IV = iV,
                Salt = salt,
                Filename = filename
            };

            bool foundExistingEntry = false;
            for (int i = 0; i < this.Entries.Count; i++)
            {
                if (this.Entries[i].SteamID == account.Session.SteamID)
                {
                    this.Entries[i].IV = iV;
                    this.Entries[i].Salt = salt;
                    this.Entries[i].Filename = filename;
                    foundExistingEntry = true;
                    break;
                }
            }

            if (!foundExistingEntry)
            {
                this.Entries.Add(newEntry);
            }

            bool wasEncrypted = this.Encrypted;
            this.Encrypted = encrypt || this.Encrypted;

            if (!this.Save())
            {
                this.Encrypted = wasEncrypted;
                return false;
            }

            try
            {
                File.WriteAllText(maDir + filename, jsonAccount);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Save()
        {
            string maDir = Manifest.GetExecutableDir() + "/maFiles/";
            string filename = maDir + "manifest.json";
            if (!Directory.Exists(maDir))
            {
                try
                {
                    Directory.CreateDirectory(maDir);
                }
                catch (Exception)
                {
                    return false;
                }
            }

            try
            {
                string contents = JsonConvert.SerializeObject(this);
                File.WriteAllText(filename, contents);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void RecomputeExistingEntries()
        {
            List<ManifestEntry> newEntries = new List<ManifestEntry>();
            string maDir = Manifest.GetExecutableDir() + "/maFiles/";

            foreach (var entry in this.Entries)
            {
                string filename = maDir + entry.Filename;
                if (File.Exists(filename))
                {
                    newEntries.Add(entry);
                }
            }

            this.Entries = newEntries;

            if (this.Entries.Count == 0)
            {
                this.Encrypted = false;
            }
        }

        public List<string> Groups()
        {
            return Entries.Select(e => e.Group).Where(g => !string.IsNullOrWhiteSpace(g))
                .Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(g => g, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public void MoveEntry(int from, int to)
        {
            if (from < 0 || to < 0 || from >= Entries.Count || to >= Entries.Count || from == to) return;
            ManifestEntry sel = Entries[from];
            Entries.RemoveAt(from);
            Entries.Insert(to, sel);
            Save();
        }

        public class ManifestEntry
        {
            [JsonProperty("encryption_iv")]
            public string IV { get; set; }

            [JsonProperty("encryption_salt")]
            public string Salt { get; set; }

            [JsonProperty("filename")]
            public string Filename { get; set; }

            [JsonProperty("steamid")]
            public ulong SteamID { get; set; }

            [JsonProperty("confirmations")]
            [JsonConverter(typeof(StringEnumConverter))]
            public ConfirmationMode Confirmations { get; set; } = ConfirmationMode.Off;

            [JsonProperty("check_interval")]
            public int CheckInterval { get; set; } = 10;

            [JsonProperty("auto_confirm_trades")]
            public bool AutoConfirmTrades { get; set; } = false;

            [JsonProperty("auto_confirm_market")]
            public bool AutoConfirmMarket { get; set; } = false;

            [JsonProperty("auto_confirm_trades_receive_only")]
            public bool AutoConfirmTradesReceiveOnly { get; set; } = false;

            [JsonProperty("auto_confirm_trades_partners_only")]
            public bool AutoConfirmTradesPartnersOnly { get; set; } = false;

            [JsonProperty("auto_confirm_trade_partners")]
            public List<ulong> AutoConfirmTradePartners { get; set; } = new List<ulong>();

            [JsonProperty("display_name")]
            public string DisplayName { get; set; }

            [JsonProperty("persona_name")]
            public string PersonaName { get; set; }

            [JsonProperty("avatar_url")]
            public string AvatarUrl { get; set; }

            [JsonProperty("profile_updated")]
            public long ProfileUpdated { get; set; }

            [JsonProperty("trade_ban")]
            public string TradeBan { get; set; }

            [JsonProperty("vac_banned")]
            public bool VacBanned { get; set; }

            [JsonProperty("game_bans")]
            public int GameBans { get; set; }

            [JsonProperty("limited_account")]
            public bool LimitedAccount { get; set; }

            [JsonProperty("group")]
            public string Group { get; set; }
        }
    }
}
