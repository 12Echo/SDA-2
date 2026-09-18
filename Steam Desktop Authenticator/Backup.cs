using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    // A backup is a flat folder of unencrypted maFiles plus the revocation codes, the same shape the importer reads
    static class Backup
    {
        private const long RemindAfterDays = 90;
        private const long RemindAgainDays = 30;

        public static void Offer(Manifest manifest, SteamGuardAccount[] accounts)
        {
            if (accounts == null || accounts.Length == 0)
            {
                MessageForm.Show("There are no accounts to back up.", "Back up accounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageForm.Show(
                "Save a backup of " + (accounts.Length == 1 ? "this account" : "all " + accounts.Length + " accounts") + "?\n\n" +
                "It is a folder with an unencrypted copy of every maFile and the revocation codes. " +
                "Anyone with it can log into the accounts, so keep it somewhere other than this PC, like a USB stick or a password manager. " +
                "To restore, pick the folder on the welcome screen or when SDA asks for a forgotten passkey.",
                "Back up accounts", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result != DialogResult.Yes) return;

            Run(manifest, accounts);
        }

        public static void Run(Manifest manifest, SteamGuardAccount[] accounts)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose where to put the backup folder";
                dialog.UseDescriptionForTitle = true;
                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string folder = Save(accounts, dialog.SelectedPath);
                    manifest.LastBackup = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    manifest.Save();
                    Log.Write("Backed up " + accounts.Length + " accounts");
                    MessageForm.Show("Backup saved to\n" + folder, "Back up accounts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageForm.Show("Could not save the backup: " + ex.Message, "Back up accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static string Save(SteamGuardAccount[] accounts, string parent)
        {
            string folder = Path.Combine(parent, "SDA2 backup " + DateTime.Now.ToString("yyyy-MM-dd HHmm"));
            Directory.CreateDirectory(folder);

            var codes = new StringBuilder();
            foreach (var account in accounts)
            {
                string name = RecoveryKit.SafeName(account.AccountName);
                File.WriteAllText(Path.Combine(folder, name + ".maFile"), JsonConvert.SerializeObject(account, Formatting.Indented));
                codes.AppendLine(account.AccountName + ": " + account.RevocationCode);
            }

            File.WriteAllText(Path.Combine(folder, "revocation codes.txt"),
                "Revocation codes, one per Steam account\r\n" +
                "\r\n" +
                codes +
                "\r\n" +
                "If you lose access to Steam Desktop Authenticator 2, go to\r\n" +
                "https://store.steampowered.com/twofactor/manage\r\n" +
                "choose Remove Authenticator and enter the code for that account.\r\n" +
                "\r\n" +
                "To restore, start Steam Desktop Authenticator 2 and choose Import accounts on the\r\n" +
                "welcome screen, or use File, Import Account and pick the .maFile files.\r\n" +
                "\r\n" +
                "Anyone with these files can log into the accounts. Keep them private.\r\n");

            return folder;
        }

        // Nags at most once a month, and only when there has been no backup for a quarter
        public static void Remind(Manifest manifest, SteamGuardAccount[] accounts)
        {
            if (accounts == null || accounts.Length == 0) return;
            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now - manifest.LastBackup < RemindAfterDays * 86400) return;
            if (now - manifest.BackupReminded < RemindAgainDays * 86400) return;

            manifest.BackupReminded = now;
            manifest.Save();

            string age = manifest.LastBackup == 0
                ? "Your accounts have never been backed up."
                : "Your last backup is " + (now - manifest.LastBackup) / 86400 + " days old.";
            var result = MessageForm.Show(age + " If this PC dies, the recovery codes are the only way back into the accounts.\n\nBack up now? It takes a moment and goes into a folder you choose.",
                "Back up accounts", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
                Run(manifest, accounts);
        }
    }
}
