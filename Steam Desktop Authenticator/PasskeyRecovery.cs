using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    // What to do when the passkey is gone: the encrypted files are set aside, never deleted, and the
    // install is rebuilt from a recovery kit or backup, or left empty so the authenticator can be set up again
    static class PasskeyRecovery
    {
        // Returns true when the install was replaced and no longer needs the old passkey
        public static bool Offer()
        {
            var answer = MessageForm.Show(
                "Without the passkey the maFiles in this install cannot be read, and Steam does not hand the secrets out again.\n\n" +
                "Yes: restore from a recovery kit or backup folder. You log into each account with its password so only the owner can do this.\n\n" +
                "No: start over. The encrypted files are set aside and you remove the authenticator through Steam, with the revocation code or Steam support, then set the account up again.\n\n" +
                "Cancel: try the passkey again.",
                "Forgot passkey", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

            if (answer == DialogResult.Yes) return RestoreFromKit();
            if (answer == DialogResult.No) return StartOver();
            return false;
        }

        private static bool RestoreFromKit()
        {
            string path;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Pick the recovery kit or backup folder";
                dialog.UseDescriptionForTitle = true;
                if (dialog.ShowDialog() != DialogResult.OK) return false;
                path = dialog.SelectedPath;
            }

            var found = new List<SteamGuardAccount>();
            int unreadable = 0;
            foreach (string file in Directory.GetFiles(path, "*.maFile", SearchOption.AllDirectories))
            {
                try
                {
                    var account = JsonConvert.DeserializeObject<SteamGuardAccount>(File.ReadAllText(file));
                    if (account?.Session == null || account.Session.SteamID == 0 || string.IsNullOrEmpty(account.SharedSecret)) throw new InvalidDataException();
                    if (!found.Exists(a => a.Session.SteamID == account.Session.SteamID))
                        found.Add(account);
                }
                catch (Exception)
                {
                    unreadable++;
                }
            }

            if (found.Count == 0)
            {
                MessageForm.Show(unreadable > 0
                    ? "The maFiles in that folder cannot be read either, they are probably encrypted too. A recovery kit or backup made by SDA is unencrypted."
                    : "That folder has no maFile in it.", "Forgot passkey", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // A fresh login is the proof of ownership, and the kit's old session is stale anyway
            var restored = new List<SteamGuardAccount>();
            foreach (var account in found)
            {
                MessageForm.Show("Log into " + account.AccountName + " with its password to restore it. The Steam Guard code is filled in from the kit.", "Forgot passkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
                var login = new LoginForm(LoginForm.LoginType.Refresh, account);
                login.ShowDialog();
                if (login.Session != null && login.Session.SteamID == account.Session.SteamID)
                    restored.Add(account);
                else
                    Log.Write("Passkey recovery: login for " + account.AccountName + " did not finish, skipped");
            }

            if (restored.Count == 0)
            {
                MessageForm.Show("No account was logged in, so nothing was changed.", "Forgot passkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            string aside;
            try
            {
                aside = SetAside();
                var manifest = Manifest.GenerateNewManifest(false);
                manifest.FirstRun = false;
                foreach (var account in restored)
                    manifest.SaveAccount(account, false);
                manifest.Save();
            }
            catch (Exception ex)
            {
                MessageForm.Show("Could not replace the files: " + ex.Message, "Forgot passkey", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Log.Write("Passkey recovery: restored " + restored.Count + " accounts from a kit, old files in " + aside);
            MessageForm.Show((restored.Count == 1 ? "1 account was restored." : restored.Count + " accounts were restored.") +
                (found.Count > restored.Count ? " " + (found.Count - restored.Count) + " could not be logged in and were left out." : "") +
                "\n\nThe old encrypted files are kept in\n" + aside + "\nin case the passkey comes back to you. Use Setup Encryption to choose a new one.",
                "Forgot passkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private static bool StartOver()
        {
            var answer = MessageForm.Show(
                "The encrypted files are moved to a folder next to the maFiles folder, nothing is deleted. SDA then starts empty.\n\n" +
                "Steam's help page opens so you can remove the old authenticator, with the revocation code from a recovery kit or through account recovery. After that, use Setup New Account.\n\nContinue?",
                "Forgot passkey", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (answer != DialogResult.Yes) return false;

            string aside;
            try
            {
                aside = SetAside();
                var manifest = Manifest.GenerateNewManifest(false);
                manifest.FirstRun = false;
                manifest.Save();
            }
            catch (Exception ex)
            {
                MessageForm.Show("Could not move the files: " + ex.Message, "Forgot passkey", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Log.Write("Passkey recovery: started over, old files in " + aside);
            Startup.OpenUrl("https://help.steampowered.com/en/wizard/HelpWithLogin");
            MessageForm.Show("The old files are in\n" + aside, "Forgot passkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private static string SetAside()
        {
            string maDir = Path.Combine(Manifest.GetExecutableDir(), "maFiles");
            string aside = Path.Combine(Manifest.GetExecutableDir(), "maFiles.locked-" + DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            Directory.Move(maDir, aside);
            return aside;
        }
    }
}
