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
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            string fileContents;
            try
            {
                fileContents = File.ReadAllText(openFileDialog1.FileName);
            }
            catch (Exception ex)
            {
                MessageForm.Show("Could not read the file: " + ex.Message, "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (importKey.Length > 0)
            {
                fileContents = DecryptImport(openFileDialog1.FileName, fileContents, importKey);
                if (fileContents == null) return;
            }

            SteamGuardAccount maFile;
            try
            {
                maFile = JsonConvert.DeserializeObject<SteamGuardAccount>(fileContents);
                if (maFile == null || string.IsNullOrEmpty(maFile.SharedSecret)) throw new InvalidDataException();
            }
            catch (Exception)
            {
                MessageForm.Show(importKey.Length > 0
                    ? "The file did not decrypt into a valid maFile. Check the passkey and try again."
                    : "This file is not a valid SteamAuth maFile.\nIf it is encrypted, enter its passkey first.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (maFile.Session == null || maFile.Session.SteamID == 0 || maFile.Session.IsAccessTokenExpired())
            {
                // Have the user to relogin to steam to get a new session
                LoginForm loginForm = new LoginForm(LoginForm.LoginType.Import, maFile);
                loginForm.ShowDialog();

                if (loginForm.Session == null || loginForm.Session.SteamID == 0)
                {
                    MessageForm.Show("Login failed. Try to import this account again.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                maFile.Session = loginForm.Session;
            }

            if (!SaveImported(maFile)) return;
            MessageForm.Show("Account imported.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Imports into an encrypted install are encrypted with the install's own passkey
        private bool SaveImported(SteamGuardAccount maFile)
        {
            if (!mManifest.Encrypted)
                return mManifest.SaveAccount(maFile, false);

            while (true)
            {
                InputForm passKeyForm = new InputForm("Enter the passkey this SDA install uses, so the imported account is stored encrypted like the others.", true);
                passKeyForm.ShowDialog();
                if (passKeyForm.Canceled) return false;

                string passKey = passKeyForm.txtBox.Text;
                if (mManifest.VerifyPasskey(passKey))
                    return mManifest.SaveAccount(maFile, true, passKey);

                MessageForm.Show("That passkey is invalid.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // An encrypted maFile needs the salt and IV from the manifest.json that sat next to it
        private static string DecryptImport(string fullPath, string fileContents, string passKey)
        {
            string manifestPath = Path.Combine(Path.GetDirectoryName(fullPath), "manifest.json");
            if (!File.Exists(manifestPath))
            {
                MessageForm.Show("manifest.json is missing next to the maFile, so it cannot be decrypted.\nCopy the manifest.json from the same maFiles folder and try again.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageForm.Show("Invalid content inside manifest.json.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            if (entry == null)
            {
                MessageForm.Show("This maFile is not listed in the manifest.json next to it.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            if (string.IsNullOrEmpty(entry.Salt) || string.IsNullOrEmpty(entry.IV))
            {
                MessageForm.Show("manifest.json says this maFile is not encrypted. Leave the passkey empty and try again.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            string decrypted = FileEncryptor.DecryptData(passKey, entry.Salt, entry.IV, fileContents);
            if (decrypted == null)
                MessageForm.Show("Decryption failed. Check the passkey and try again.", "Account Import", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
