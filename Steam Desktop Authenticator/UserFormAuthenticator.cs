using SteamAuth;
using SteamKit2;
using SteamKit2.Authentication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    internal class UserFormAuthenticator : IAuthenticator
    {
        private SteamGuardAccount account;
        private int deviceCodesGenerated = 0;
        private static readonly Dictionary<string, string> lastCodes = new Dictionary<string, string>();

        // Quiet is for the client session set up after the real login: it never asks the user anything, it just fails
        private bool quiet;

        // SteamKit2 calls back on its own threads after the first prompt, so dialogs are shown through the login window
        private Control owner;
        private SteamClient steamClient;

        // True while the user is typing a code or a retry waits for the next one, the login window does not time that out
        public bool Asking { get; private set; }
        public bool Canceled { get; private set; }

        public UserFormAuthenticator(SteamGuardAccount account, bool quiet = false, SteamClient steamClient = null, Control owner = null)
        {
            this.account = account;
            this.quiet = quiet;
            this.steamClient = steamClient;
            this.owner = owner;
        }

        public Task<bool> AcceptDeviceConfirmationAsync()
        {
            return Task.FromResult(false);
        }

        public async Task<string> GetDeviceCodeAsync(bool previousCodeWasIncorrect)
        {
            Asking = true;
            try
            {
                string code = await DeviceCode(previousCodeWasIncorrect);
                await Reconnect();
                return code;
            }
            finally
            {
                Asking = false;
            }
        }

        private async Task<string> DeviceCode(bool previousCodeWasIncorrect)
        {
            // If a code fails wait 30 seconds for a new one to regenerate
            if (previousCodeWasIncorrect)
            {
                // After 2 tries tell the user that there seems to be an issue
                if (deviceCodesGenerated > 2 && !quiet)
                    OnUi(() => MessageForm.Show("There seems to be an issue logging into your account with these two factor codes. Are you sure SDA is still your authenticator?"));

                await Task.Delay(30000);
                await TimeAligner.AlignTimeAsync();
            }

            string deviceCode;

            if (account == null)
            {
                if (quiet) return null;
                OnUi(() => MessageForm.Show("This account already has an authenticator linked. You must remove that authenticator to add SDA as your authenticator.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Error));
                return null;
            }
            else
            {
                deviceCode = await account.GenerateSteamGuardCodeAsync();

                // Steam refuses a code that was already used, wait for the next one instead of failing
                string lastCode;
                string key = account.AccountName ?? "";
                if (lastCodes.TryGetValue(key, out lastCode) && lastCode == deviceCode)
                {
                    long time = await TimeAligner.GetSteamTimeAsync();
                    await Task.Delay((int)(31 - time % 30) * 1000);
                    deviceCode = await account.GenerateSteamGuardCodeAsync();
                }
                lastCodes[key] = deviceCode;
                deviceCodesGenerated++;
            }

            return deviceCode;
        }

        public async Task<string> GetEmailCodeAsync(string email, bool previousCodeWasIncorrect)
        {
            if (quiet) return null;
            string message = "Enter the code sent to your email:";
            if (previousCodeWasIncorrect)
            {
                message = "The code you provided was invalid. Enter the code sent to your email:";
            }

            Asking = true;
            try
            {
                string code = OnUi(() =>
                {
                    InputForm emailForm = new InputForm(message);
                    emailForm.ShowDialog();
                    return emailForm.Canceled ? null : emailForm.txtBox.Text;
                });
                Canceled = code == null;

                // Fetching the email often takes long enough for Steam to drop the idle connection
                if (!Canceled) await Reconnect();
                return code;
            }
            finally
            {
                Asking = false;
            }
        }

        private async Task Reconnect()
        {
            if (steamClient != null && !steamClient.IsConnected)
                await LoginForm.Connect(steamClient);
        }

        private T OnUi<T>(Func<T> func)
        {
            if (owner != null && owner.InvokeRequired)
                return owner.Invoke(func);
            return func();
        }
    }
}
