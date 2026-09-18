using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public partial class WelcomeForm : Form
    {
        private Manifest man;

        public WelcomeForm()
        {
            InitializeComponent();
            Theme.Apply(this);
            Language.Apply(this);
            man = Manifest.GetManifest();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Program.ShowMessage)
            {
                Activate();
                return;
            }
            base.WndProc(ref m);
        }

        private void btnJustStart_Click(object sender, EventArgs e)
        {
            // Mark as not first run anymore
            man.FirstRun = false;
            man.Save();

            showMainForm();
        }

        private void btnImportConfig_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select your old Steam Desktop Authenticator folder or a recovery kit folder";
            folderBrowser.UseDescriptionForTitle = true;
            if (folderBrowser.ShowDialog() != DialogResult.OK) return;

            if (ImportFolder(folderBrowser.SelectedPath))
                showMainForm();
        }

        // Accepts an old install folder, its maFiles folder, or a folder of loose maFiles such as a recovery kit
        internal bool ImportFolder(string path)
        {
            string maDir = Manifest.GetExecutableDir() + "/maFiles";
            Directory.CreateDirectory(maDir);

            // An install has a manifest, a recovery kit or a backup is just loose maFiles
            string install = Directory.Exists(path + "/maFiles") ? path + "/maFiles" : File.Exists(path + "/manifest.json") ? path : null;
            string[] loose = Directory.Exists(path) ? Directory.GetFiles(path, "*.maFile") : new string[0];

            if (install != null)
            {
                foreach (string file in Directory.GetFiles(install, "*.*", SearchOption.AllDirectories))
                    File.Copy(file, file.Replace(install, maDir), true);
            }
            else if (loose.Length > 0)
            {
                foreach (string file in loose)
                    File.Copy(file, Path.Combine(maDir, Path.GetFileName(file)), true);
            }
            else
            {
                MessageForm.Show("This folder does not contain a manifest.json, a maFiles folder or any .maFile.\nPick the folder where the old Steam Desktop Authenticator was installed, or a recovery kit folder.", "Import accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                if (install != null)
                {
                    man = Manifest.GetManifest(true);
                    man.FirstRun = false;
                    man.Save();
                }
                else
                {
                    man = Manifest.GenerateNewManifest(true);
                }
            }
            catch (ManifestParseException)
            {
                // Manifest file was corrupted, generate a new one.
                try
                {
                    MessageForm.Show("Your settings were unexpectedly corrupted and were reset to defaults.", "Steam Desktop Authenticator 2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    man = Manifest.GenerateNewManifest(true);
                }
                catch (MaFileEncryptedException)
                {
                    MessageForm.Show("Sorry, but SDA was unable to recover your accounts since you used encryption.\nYou'll need to recover your Steam accounts by removing the authenticator.\nClick OK to view instructions.", "Steam Desktop Authenticator 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Startup.OpenUrl("https://github.com/Jessecar96/SteamDesktopAuthenticator/wiki/Help!-I'm-locked-out-of-my-account");
                    this.Close();
                    return false;
                }
            }
            catch (MaFileEncryptedException)
            {
                MessageForm.Show("One of these maFiles is encrypted. Copy the manifest.json that belongs to it into the same folder and try again, or use File, Import Account and enter its passkey.", "Import accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (man == null || man.Entries.Count == 0)
            {
                MessageForm.Show("No accounts were found in that folder.", "Import accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            man.FirstRun = false;
            man.Save();
            MessageForm.Show(man.Entries.Count == 1 ? "1 account was imported. Click OK to continue." : man.Entries.Count + " accounts were imported. Click OK to continue.", "Import accounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private void showMainForm()
        {
            this.Hide();
            new MainForm().Show();
        }
    }
}
