using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamAuth;
using SteamKit2;
using SteamKit2.Authentication;
using SteamKit2.Internal;

namespace Steam_Desktop_Authenticator
{
    public partial class LoginForm : Form
    {
        public SteamGuardAccount account;
        public LoginType LoginReason;
        public SessionData Session;

        // MainForm passes the passkey in when it has one, and reads it back when the first account sets one up here
        public string PassKey;

        private UserFormAuthenticator authenticator;
        private bool closed;
        private string explanation;

        public LoginForm(LoginType loginReason = LoginType.Initial, SteamGuardAccount account = null)
        {
            InitializeComponent();
            Theme.Apply(this);
            Language.Apply(this);
            this.LoginReason = loginReason;
            this.account = account;
            this.FormClosed += (s, e) => closed = true;

            try
            {
                if (this.LoginReason != LoginType.Initial)
                {
                    txtUsername.Text = account.AccountName;
                    txtUsername.Enabled = false;
                }

                if (this.LoginReason == LoginType.Refresh)
                {
                    labelLoginExplanation.Text = "Your Steam credentials have expired. For trade and market confirmations to work properly, please login again.";
                }
                else if (this.LoginReason == LoginType.Import)
                {
                    labelLoginExplanation.Text = "Please login to your Steam account import it.";
                }
            }
            catch (Exception)
            {
                MessageForm.Show("Failed to find your account. Try closing and re-opening SDA.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        public void SetUsername(string username)
        {
            txtUsername.Text = username;
        }

        public string FilterPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Replace("-", "").Replace("(", "").Replace(")", "");
        }

        public bool PhoneNumberOkay(string phoneNumber)
        {
            if (phoneNumber == null || phoneNumber.Length == 0) return false;
            if (phoneNumber[0] != '+') return false;
            return true;
        }

        private void ResetLoginButton()
        {
            btnSteamLogin.Enabled = true;
            btnSteamLogin.Text = "Login";
            labelLoginExplanation.Text = explanation;
        }

        private async void btnSteamLogin_Click(object sender, EventArgs e)
        {
            // Disable button while we login
            btnSteamLogin.Enabled = false;
            btnSteamLogin.Text = "Logging in...";
            explanation = labelLoginExplanation.Text;

            SteamClient steamClient = new SteamClient();
            try
            {
                await Login(steamClient, txtUsername.Text, txtPassword.Text);
            }
            catch (OperationCanceledException) when (closed)
            {
                // The window was closed while Steam was still working
            }
            finally
            {
                steamClient.Disconnect();
            }
        }

        private async Task Login(SteamClient steamClient, string username, string password)
        {
            if (!await Connect(steamClient))
            {
                MessageForm.Show("Unable to connect to Steam. Check your internet connection and try again.", "Steam Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLoginButton();
                return;
            }

            // Create a new auth session and poll Steam for the result
            authenticator = new UserFormAuthenticator(this.account, false, steamClient, this);
            CredentialsAuthSession authSession;
            AuthPollResult pollResponse;
            try
            {
                authSession = await Wait(steamClient.Authentication.BeginAuthSessionViaCredentialsAsync(new AuthSessionDetails
                {
                    Username = username,
                    Password = password,
                    IsPersistentSession = false,
                    PlatformType = EAuthTokenPlatformType.k_EAuthTokenPlatformType_MobileApp,
                    ClientOSType = EOSType.Android9,
                    Authenticator = authenticator,
                }), steamClient);

                pollResponse = await Wait(authSession.PollingWaitForResultAsync(), steamClient);
            }
            catch (Exception ex) when (!closed)
            {
                if (!authenticator.Canceled)
                    MessageForm.Show(LoginError(ex), "Steam Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLoginButton();
                return;
            }

            // Build a SessionData object
            SessionData sessionData = new SessionData()
            {
                SteamID = authSession.SteamID.ConvertToUInt64(),
                AccessToken = pollResponse.AccessToken,
                RefreshToken = pollResponse.RefreshToken,
            };

            //Login succeeded
            this.Session = sessionData;

            // If we're only logging in for an account import, stop here
            if (LoginReason == LoginType.Import)
            {
                sessionData.ClientRefreshToken = await ClientLogin(steamClient, username, password, this.account);
                this.Close();
                return;
            }

            // If we're only logging in for a session refresh then save it and exit
            if (LoginReason == LoginType.Refresh)
            {
                sessionData.ClientRefreshToken = await ClientLogin(steamClient, username, password, this.account);
                Manifest man = Manifest.GetManifest();
                account.FullyEnrolled = true;
                account.Session = sessionData;
                HandleManifest(man, true);
                this.Close();
                return;
            }

            // Show a dialog to make sure they really want to add their authenticator
            var result = MessageForm.Show("Steam account login succeeded. Press OK to continue adding SDA as your authenticator.", "Steam Login", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Cancel)
            {
                MessageForm.Show("Adding authenticator aborted.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ResetLoginButton();
                return;
            }

            // Begin linking mobile authenticator
            labelLoginExplanation.Text = "Adding SDA as the authenticator for this account...";
            AuthenticatorLinker linker = new AuthenticatorLinker(sessionData);

            AuthenticatorLinker.LinkResult linkResponse = AuthenticatorLinker.LinkResult.GeneralFailure;
            while (linkResponse != AuthenticatorLinker.LinkResult.AwaitingFinalization)
            {
                try
                {
                    linkResponse = await Wait(linker.AddAuthenticator());
                }
                catch (Exception ex) when (!closed)
                {
                    MessageForm.Show("Error adding your authenticator: " + ex.Message, "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetLoginButton();
                    return;
                }

                switch (linkResponse)
                {
                    case AuthenticatorLinker.LinkResult.MustProvidePhoneNumber:

                        // Show the phone input form
                        PhoneInputForm phoneInputForm = new PhoneInputForm(account);
                        phoneInputForm.ShowDialog();
                        if (phoneInputForm.Canceled)
                        {
                            this.Close();
                            return;
                        }

                        linker.PhoneNumber = phoneInputForm.PhoneNumber;
                        linker.PhoneCountryCode = phoneInputForm.CountryCode;
                        break;

                    case AuthenticatorLinker.LinkResult.AuthenticatorPresent:
                        MessageForm.Show("This account already has an authenticator linked. You must remove that authenticator to add SDA as your authenticator.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;

                    case AuthenticatorLinker.LinkResult.FailureAddingPhone:
                        MessageForm.Show("Failed to add your phone number. Please try again or use a different phone number.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        linker.PhoneNumber = null;
                        break;

                    case AuthenticatorLinker.LinkResult.MustRemovePhoneNumber:
                        linker.PhoneNumber = null;
                        break;

                    case AuthenticatorLinker.LinkResult.MustConfirmEmail:
                        MessageForm.Show("Please check your email, and click the link Steam sent you before continuing.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;

                    case AuthenticatorLinker.LinkResult.GeneralFailure:
                        MessageForm.Show("Error adding your authenticator.", "Steam Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Close();
                        return;
                }
            } // End while loop checking for AwaitingFinalization

            Manifest manifest = Manifest.GetManifest();
            string passKey = null;
            if (manifest.Entries.Count == 0)
            {
                passKey = manifest.PromptSetupPassKey("Please enter an encryption passkey. Leave blank or hit cancel to not encrypt (VERY INSECURE).");
            }
            else if (manifest.Encrypted && PassKey != null && manifest.VerifyPasskey(PassKey))
            {
                passKey = PassKey;
            }
            else if (manifest.Entries.Count > 0 && manifest.Encrypted)
            {
                bool passKeyValid = false;
                while (!passKeyValid)
                {
                    InputForm passKeyForm = new InputForm("Please enter your current encryption passkey.", true);
                    passKeyForm.ShowDialog();
                    if (!passKeyForm.Canceled)
                    {
                        passKey = passKeyForm.txtBox.Text;
                        passKeyValid = manifest.VerifyPasskey(passKey);
                        if (!passKeyValid)
                        {
                            MessageForm.Show("That passkey is invalid. Please enter the same passkey you used for your other accounts.");
                        }
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
            }

            //Save the file immediately; losing this would be bad.
            if (!manifest.SaveAccount(linker.LinkedAccount, passKey != null, passKey))
            {
                manifest.RemoveAccount(linker.LinkedAccount);
                MessageForm.Show("Unable to save mobile authenticator file. The mobile authenticator has not been linked.");
                this.Close();
                return;
            }
            PassKey = passKey;

            MessageForm.ShowCode("The Mobile Authenticator has not yet been linked. Before finalizing the authenticator, please write down your revocation code.", "Steam Login", linker.LinkedAccount.RevocationCode);

            AuthenticatorLinker.FinalizeResult finalizeResponse = AuthenticatorLinker.FinalizeResult.GeneralFailure;
            while (finalizeResponse != AuthenticatorLinker.FinalizeResult.Success)
            {
                InputForm smsCodeForm = new InputForm("Enter the code Steam sent you. Depending on the account it arrives by SMS or by email.");
                smsCodeForm.ShowDialog();
                if (smsCodeForm.Canceled)
                {
                    manifest.RemoveAccount(linker.LinkedAccount);
                    this.Close();
                    return;
                }

                InputForm confirmRevocationCode = new InputForm("Please enter your revocation code to ensure you've saved it.");
                confirmRevocationCode.ShowDialog();
                if (confirmRevocationCode.txtBox.Text.ToUpper() != linker.LinkedAccount.RevocationCode)
                {
                    MessageForm.Show("Revocation code incorrect; the authenticator has not been linked.");
                    manifest.RemoveAccount(linker.LinkedAccount);
                    this.Close();
                    return;
                }

                string smsCode = smsCodeForm.txtBox.Text;
                labelLoginExplanation.Text = "Finishing the link with Steam...";
                try
                {
                    finalizeResponse = await Wait(linker.FinalizeAddAuthenticator(smsCode));
                }
                catch (Exception ex) when (!closed)
                {
                    // Steam may have linked it even though no answer came back, so the file stays
                    Log.Error("Finalizing the authenticator for " + username, ex);
                    MessageForm.ShowCode("Steam did not confirm the link. " + ex.Message + " It may be linked anyway, so SDA has kept the account. If its codes do not work, remove it and add it again. Please write down your revocation code.", "Steam Login", linker.LinkedAccount.RevocationCode);
                    this.Close();
                    return;
                }

                switch (finalizeResponse)
                {
                    case AuthenticatorLinker.FinalizeResult.BadSMSCode:
                        continue;

                    case AuthenticatorLinker.FinalizeResult.UnableToGenerateCorrectCodes:
                        MessageForm.ShowCode("Unable to generate the proper codes to finalize this authenticator. The authenticator should not have been linked. In the off-chance it was, please write down your revocation code, as this is the last chance to see it.", "Steam Login", linker.LinkedAccount.RevocationCode);
                        manifest.RemoveAccount(linker.LinkedAccount);
                        this.Close();
                        return;

                    case AuthenticatorLinker.FinalizeResult.GeneralFailure:
                        MessageForm.ShowCode("Unable to finalize this authenticator. The authenticator should not have been linked. In the off-chance it was, please write down your revocation code, as this is the last chance to see it.", "Steam Login", linker.LinkedAccount.RevocationCode);
                        manifest.RemoveAccount(linker.LinkedAccount);
                        this.Close();
                        return;
                }
            }

            //Linked, finally. Re-save with FullyEnrolled property.
            manifest.SaveAccount(linker.LinkedAccount, passKey != null, passKey);
            MessageForm.ShowCode("Mobile authenticator successfully linked. Please write down your revocation code.", "Steam Login", linker.LinkedAccount.RevocationCode);
            RecoveryKit.Offer(linker.LinkedAccount);

            // Last, so a slow Steam never stands between a linked authenticator and the user knowing it is linked
            sessionData.ClientRefreshToken = await ClientLogin(steamClient, username, password, linker.LinkedAccount);
            if (sessionData.ClientRefreshToken != null)
                manifest.SaveAccount(linker.LinkedAccount, passKey != null, passKey);
            this.Close();
        }

        // The mobile token cannot be used for a client connection, so get a client one too. Without it confirmations are
        // still checked, only not instantly, so Steam gets a minute and no more.
        private async Task<string> ClientLogin(SteamClient steamClient, string username, string password, SteamGuardAccount account)
        {
            labelLoginExplanation.Text = "Setting up a Steam client session so confirmations can arrive instantly. This can take up to a minute.";
            var cancel = new CancellationTokenSource();
            var login = ClientSession(steamClient, username, password, account, cancel.Token);
            for (int i = 0; i < 60 && !closed && !login.IsCompleted; i++)
                await Task.WhenAny(login, Task.Delay(1000));
            if (!login.IsCompleted && !closed)
                Log.Write("Client session for " + username + " took over a minute, confirmations are checked periodically instead");
            cancel.Cancel();
            steamClient.Disconnect();
            return login.IsCompleted ? login.Result : null;
        }

        private static async Task<string> ClientSession(SteamClient steamClient, string username, string password, SteamGuardAccount account, CancellationToken cancel)
        {
            try
            {
                if (!await Connect(steamClient, cancel))
                    return null;

                var authSession = await steamClient.Authentication.BeginAuthSessionViaCredentialsAsync(new AuthSessionDetails
                {
                    Username = username,
                    Password = password,
                    IsPersistentSession = true,
                    PlatformType = EAuthTokenPlatformType.k_EAuthTokenPlatformType_SteamClient,
                    ClientOSType = EOSType.Windows10,
                    Authenticator = new UserFormAuthenticator(account, true),
                });

                var pollResponse = await authSession.PollingWaitForResultAsync(cancel);
                return pollResponse.RefreshToken;
            }
            catch (Exception ex)
            {
                if (!cancel.IsCancellationRequested)
                    Log.Error("Client session for " + username, ex);
                return null;
            }
        }

        internal static async Task<bool> Connect(SteamClient steamClient, CancellationToken cancel = default)
        {
            if (steamClient.IsConnected) return true;
            steamClient.Connect();
            for (int i = 0; i < 60 && !steamClient.IsConnected; i++)
                await Task.Delay(500, cancel);
            return steamClient.IsConnected;
        }

        // SteamKit2 stops timing out its requests once the connection drops and WebClient's async calls never time out,
        // so without this a dropped connection leaves the window on "Logging in..." forever
        private async Task<T> Wait<T>(Task<T> task, SteamClient steamClient = null)
        {
            int seconds = 0, down = 0;
            while (await Task.WhenAny(task, Task.Delay(1000)) != task)
            {
                if (closed) throw new OperationCanceledException();
                if (authenticator != null && authenticator.Asking)
                {
                    seconds = down = 0;
                    continue;
                }

                down = steamClient == null || steamClient.IsConnected ? 0 : down + 1;
                if (down >= 10) throw new TimeoutException("Lost the connection to Steam.");
                if (++seconds >= 120) throw new TimeoutException("Steam did not answer in time.");
            }
            return await task;
        }

        private static string LoginError(Exception ex)
        {
            var auth = ex as AuthenticationException;
            if (auth != null && auth.Result == EResult.InvalidPassword)
                return "Wrong username or password.";
            if (auth != null && auth.Result == EResult.RateLimitExceeded)
                return "Steam is refusing logins after too many attempts. Wait a while and try again.";
            if (ex is TimeoutException || ex is TaskCanceledException)
                return (ex is TimeoutException ? ex.Message : "Steam did not answer in time.") + " Please try again.";
            return ex.Message;
        }

        private void HandleManifest(Manifest man, bool IsRefreshing = false)
        {
            string passKey = null;
            if (man.Entries.Count == 0)
            {
                passKey = man.PromptSetupPassKey("Please enter an encryption passkey. Leave blank or hit cancel to not encrypt (VERY INSECURE).");
            }
            else if (man.Encrypted && PassKey != null && man.VerifyPasskey(PassKey))
            {
                passKey = PassKey;
            }
            else if (man.Entries.Count > 0 && man.Encrypted)
            {
                bool passKeyValid = false;
                while (!passKeyValid)
                {
                    InputForm passKeyForm = new InputForm("Please enter your current encryption passkey.", true);
                    passKeyForm.ShowDialog();
                    if (!passKeyForm.Canceled)
                    {
                        passKey = passKeyForm.txtBox.Text;
                        passKeyValid = man.VerifyPasskey(passKey);
                        if (!passKeyValid)
                        {
                            MessageForm.Show("That passkey is invalid. Please enter the same passkey you used for your other accounts.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
            }

            man.SaveAccount(account, passKey != null, passKey);
            if (IsRefreshing)
            {
                MessageForm.Show("Your session was refreshed.", "Steam Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageForm.ShowCode("Mobile authenticator successfully linked. Please write down your revocation code.", "Steam Login", account.RevocationCode);
                RecoveryKit.Offer(account);
            }
            this.Close();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            if (account != null && account.AccountName != null)
            {
                txtUsername.Text = account.AccountName;
            }
        }

        public enum LoginType
        {
            Initial,
            Refresh,
            Import
        }
    }
}
