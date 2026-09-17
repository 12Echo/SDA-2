using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    static class RecoveryKit
    {
        public static void Offer(SteamGuardAccount account)
        {
            var result = MessageForm.Show(
                "Save a recovery kit for " + account.AccountName + "?\n\n" +
                "It is a folder named after the account with an unencrypted copy of the maFile and the revocation code. " +
                "Keep it somewhere other than this PC, like a USB stick or a password manager. " +
                "With it you can always get back into the account, even if SDA or this computer is lost.",
                "Recovery kit", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (result != DialogResult.Yes) return;

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Choose where to put the recovery kit folder";
                dialog.UseDescriptionForTitle = true;
                if (dialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    string folder = Save(account, dialog.SelectedPath);
                    MessageForm.Show("Recovery kit saved to\n" + folder, "Recovery kit", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageForm.Show("Could not save the recovery kit: " + ex.Message, "Recovery kit", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public static string Save(SteamGuardAccount account, string parent)
        {
            string name = account.AccountName;
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            string folder = Path.Combine(parent, name);
            Directory.CreateDirectory(folder);

            File.WriteAllText(Path.Combine(folder, name + ".maFile"), JsonConvert.SerializeObject(account, Formatting.Indented));
            File.WriteAllText(Path.Combine(folder, "revocation code.txt"),
                "Steam account: " + account.AccountName + "\r\n" +
                "Revocation code: " + account.RevocationCode + "\r\n" +
                "\r\n" +
                "If you lose access to Steam Desktop Authenticator 2, go to\r\n" +
                "https://store.steampowered.com/twofactor/manage\r\n" +
                "choose Remove Authenticator and enter the revocation code above.\r\n" +
                "\r\n" +
                "To move the authenticator to another PC, start Steam Desktop Authenticator 2 there,\r\n" +
                "choose Import accounts on the welcome screen and pick this folder. If it already has\r\n" +
                "accounts, use File, Import Account and pick the .maFile instead.\r\n" +
                "\r\n" +
                "Anyone with these files can log into the account. Keep them private.\r\n");

            return folder;
        }
    }
}
