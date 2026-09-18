using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SteamAuth;
using Newtonsoft.Json;

namespace Steam_Desktop_Authenticator
{
    public partial class ImportAccountForm : Form
    {
        private Manifest mManifest;

        public ImportAccountForm()
        {
            InitializeComponent();
            Theme.Apply(this);
            Language.Apply(this);
            this.mManifest = Manifest.GetManifest();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            string importKey = txtBox.Text;
            this.Close();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "maFiles (.maFile)|*.maFile|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Select one or more maFiles to import";
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            ImportFiles(openFileDialog1.FileNames, importKey);
        }

        // Every file gets its turn, problems are collected and reported together at the end
        internal void ImportFiles(string[] files, string importKey)
        {
            var imported = new List<string>();
            var failed = new List<string>();
            string passKey = null;

            foreach (string path in files)
            {
                string error;
                SteamGuardAccount maFile = ReadMaFile(path, importKey, out error);
                if (maFile == null)
                {
                    failed.Add(Path.GetFileName(path) + ": " + error);
                    continue;
                }

                if (maFile.Session == null || maFile.Session.SteamID == 0 || maFile.Session.IsAccessTokenExpired())
                {
                    // Have the user to relogin to steam to get a new session
                    LoginForm loginForm = new LoginForm(LoginForm.LoginType.Import, maFile);
                    loginForm.ShowDialog();

                    if (loginForm.Session == null || loginForm.Session.SteamID == 0)
                    {
                        failed.Add(Path.GetFileName(path) + ": login failed, try importing it again");
                        continue;
                    }

                    maFile.Session = loginForm.Session;
                }

                bool cancelled;
                if (!SaveImported(maFile, ref passKey, out cancelled))
                {
                    if (cancelled) break;
                    failed.Add(Path.GetFileName(path) + ": could not be saved");
                    continue;
                }
                imported.Add(maFile.AccountName);
            }

            if (imported.Count == 0 && failed.Count == 0) return;
            if (files.Length == 1 && failed.Count == 1)
            {
                MessageForm.Show(failed[0].Substring(failed[0].IndexOf(": ") + 2), "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string text = imported.Count == 0 ? "No accounts were imported."
                : imported.Count == 1 ? "Account imported."
                : imported.Count + " accounts imported: " + string.Join(", ", imported) + ".";
            if (failed.Count > 0)
                text += "\n\nNot imported:\n" + string.Join("\n", failed);
            MessageForm.Show(text, "Account Import", MessageBoxButtons.OK, failed.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }

        private static SteamGuardAccount ReadMaFile(string path, string importKey, out string error)
        {
            string fileContents;
            try
            {
                fileContents = File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                error = "could not read the file, " + ex.Message;
                return null;
            }

            if (importKey.Length > 0)
            {
                fileContents = DecryptImport(path, fileContents, importKey, out error);
                if (fileContents == null) return null;
            }

            try
            {
                var maFile = JsonConvert.DeserializeObject<SteamGuardAccount>(fileContents);
                if (maFile == null || string.IsNullOrEmpty(maFile.SharedSecret)) throw new InvalidDataException();
                error = null;
                return maFile;
            }
            catch (Exception)
            {
                error = importKey.Length > 0
                    ? "the file did not decrypt into a valid maFile. Check the passkey and try again."
                    : "this file is not a valid SteamAuth maFile. If it is encrypted, enter its passkey first.";
                return null;
            }
        }

        // Imports into an encrypted install are encrypted with the install's own passkey, asked for once per import run
        private bool SaveImported(SteamGuardAccount maFile, ref string passKey, out bool cancelled)
        {
            cancelled = false;
            if (!mManifest.Encrypted)
                return mManifest.SaveAccount(maFile, false);

            while (passKey == null)
            {
                InputForm passKeyForm = new InputForm("Enter the passkey this SDA install uses, so the imported accounts are stored encrypted like the others.", true);
                passKeyForm.ShowDialog();
                if (passKeyForm.Canceled)
                {
                    cancelled = true;
                    return false;
                }

                if (mManifest.VerifyPasskey(passKeyForm.txtBox.Text))
                    passKey = passKeyForm.txtBox.Text;
                else
                    MessageForm.Show("That passkey is invalid.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return mManifest.SaveAccount(maFile, true, passKey);
        }

        // An encrypted maFile needs the salt and IV from the manifest.json that sat next to it
        private static string DecryptImport(string fullPath, string fileContents, string passKey, out string error)
        {
            error = null;
            string manifestPath = Path.Combine(Path.GetDirectoryName(fullPath), "manifest.json");
            if (!File.Exists(manifestPath))
            {
                error = "manifest.json is missing next to the maFile, so it cannot be decrypted. Copy the manifest.json from the same maFiles folder and try again.";
                return null;
            }

            ImportManifestEntry entry;
            try
            {
                var manifest = JsonConvert.DeserializeObject<ImportManifest>(File.ReadAllText(manifestPath));
                string fileName = Path.GetFileName(fullPath);
                entry = manifest.Entries?.FirstOrDefault(en => string.Equals(en.Filename, fileName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception)
            {
                error = "invalid content inside manifest.json.";
                return null;
            }

            if (entry == null)
            {
                error = "this maFile is not listed in the manifest.json next to it.";
                return null;
            }
            if (string.IsNullOrEmpty(entry.Salt) || string.IsNullOrEmpty(entry.IV))
            {
                error = "manifest.json says this maFile is not encrypted. Leave the passkey empty and try again.";
                return null;
            }

            string decrypted = FileEncryptor.DecryptData(passKey, entry.Salt, entry.IV, fileContents);
            if (decrypted == null)
                error = "decryption failed. Check the passkey and try again.";
            return decrypted;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Import_maFile_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }


    public class AppManifest
    {
        [JsonProperty("encrypted")]
        public bool Encrypted { get; set; }
    }


    public class ImportManifest
    {
        [JsonProperty("encrypted")]
        public bool Encrypted { get; set; }

        [JsonProperty("entries")]
        public List<ImportManifestEntry> Entries { get; set; }
    }

    public class ImportManifestEntry
    {
        [JsonProperty("encryption_iv")]
        public string IV { get; set; }

        [JsonProperty("encryption_salt")]
        public string Salt { get; set; }

        [JsonProperty("filename")]
        public string Filename { get; set; }

        [JsonProperty("steamid")]
        public ulong SteamID { get; set; }
    }
}
