using SteamAuth;
using SteamKit2.Authentication;
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

        public UserFormAuthenticator(SteamGuardAccount account)
        {
            this.account = account;
        }

        public Task<bool> AcceptDeviceConfirmationAsync()
        {
            return Task.FromResult(false);
        }

        public async Task<string> GetDeviceCodeAsync(bool previousCodeWasIncorrect)
        {
            // If a code fails wait 30 seconds for a new one to regenerate
            if (previousCodeWasIncorrect)
            {
                // After 2 tries tell the user that there seems to be an issue
                if (deviceCodesGenerated > 2)
                    MessageForm.Show("There seems to be an issue logging into your account with these two factor codes. Are you sure SDA is still your authenticator?");

                await Task.Delay(30000);
                await TimeAligner.AlignTimeAsync();
            }

            string deviceCode;

            if (account == null)
            {
                MessageForm.Show("This account already has an authenticator linked. You must remove that authenticator to add SDA as your authenticator.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        public Task<string> GetEmailCodeAsync(string email, bool previousCodeWasIncorrect)
        {
            string message = "Enter the code sent to your email:";
            if (previousCodeWasIncorrect)
            {
                message = "The code you provided was invalid. Enter the code sent to your email:";
            }

            InputForm emailForm = new InputForm(message);
            emailForm.ShowDialog();
            return Task.FromResult(emailForm.txtBox.Text);
        }
    }
}
