using System;
using System.Diagnostics;
using System.Windows.Forms;
using SteamAuth;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Linq;

namespace Steam_Desktop_Authenticator
{
    public partial class MainForm : Form
    {
        private SteamGuardAccount currentAccount = null;
        private SteamGuardAccount[] allAccounts;
        private Manifest manifest;
        private static SemaphoreSlim confirmationsSemaphore = new SemaphoreSlim(1, 1);
        private HashSet<ulong> notifiedConfirmations = new HashSet<ulong>();
        private HashSet<ulong> expiredSessions = new HashSet<ulong>();
        private SteamGuardAccount notifiedAccount;
        private List<AccountWatcher> watchers = new List<AccountWatcher>();
        private string liveStatus = "";

        private long steamTime = 0;
        private long currentSteamChunk = 0;
        private string passKey = null;
        private bool startSilent = false;

        public MainForm()
        {
            InitializeComponent();
            Theme.Apply(this);
            Theme.Apply(menuStripTray);
        }

        public void SetEncryptionKey(string key)
        {
            passKey = key;
        }

        public void StartSilent(bool silent)
        {
            startSilent = silent;
        }

        // Form event handlers

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.labelVersion.Text = String.Format("v{0}", Application.ProductVersion);
            try
            {
                this.manifest = Manifest.GetManifest();
            }
            catch (ManifestParseException)
            {
                MessageBox.Show("Unable to read your settings. Try restating SDA.", "Steam Desktop Authenticator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            // Make sure we don't show that welcome dialog again
            this.manifest.FirstRun = false;
            this.manifest.Save();

            // Tick first time manually to sync time
            timerSteamGuard_Tick(new object(), EventArgs.Empty);

            if (manifest.Encrypted)
            {
                if (passKey == null)
                {
                    passKey = manifest.PromptForPassKey();
                    if (passKey == null)
                    {
                        Application.Exit();
                    }
                }

                btnManageEncryption.Text = "Manage Encryption";
            }
            else
            {
                btnManageEncryption.Text = "Setup Encryption";
            }

            btnManageEncryption.Enabled = manifest.Entries.Count > 0;

            loadSettings();
            loadAccountsList();
            listAccounts.Focus();

            checkForUpdates();

            if (startSilent)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            trayIcon.Icon = this.Icon;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopWatchers();
            Application.Exit();
        }


        // UI Button handlers

        private void btnSteamLogin_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.ShowDialog();
            this.loadAccountsList();
        }

        private void btnTradeConfirmations_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;

            string oText = btnTradeConfirmations.Text;
            btnTradeConfirmations.Text = "Loading...";
            btnTradeConfirmations.Text = oText;

            ConfirmationFormWeb confirms = new ConfirmationFormWeb(currentAccount);
            confirms.Show();
        }

        private void btnManageEncryption_Click(object sender, EventArgs e)
        {
            if (manifest.Encrypted)
            {
                InputForm currentPassKeyForm = new InputForm("Enter current passkey", true);
                currentPassKeyForm.ShowDialog();

                if (currentPassKeyForm.Canceled)
                {
                    return;
                }

                string curPassKey = currentPassKeyForm.txtBox.Text;

                InputForm changePassKeyForm = new InputForm("Enter new passkey, or leave blank to remove encryption.");
                changePassKeyForm.ShowDialog();

                if (changePassKeyForm.Canceled && !string.IsNullOrEmpty(changePassKeyForm.txtBox.Text))
                {
                    return;
                }

                InputForm changePassKeyForm2 = new InputForm("Confirm new passkey, or leave blank to remove encryption.");
                changePassKeyForm2.ShowDialog();

                if (changePassKeyForm2.Canceled && !string.IsNullOrEmpty(changePassKeyForm.txtBox.Text))
                {
                    return;
                }

                string newPassKey = changePassKeyForm.txtBox.Text;
                string confirmPassKey = changePassKeyForm2.txtBox.Text;

                if (newPassKey != confirmPassKey)
                {
                    MessageBox.Show("Passkeys do not match.");
                    return;
                }

                if (newPassKey.Length == 0)
                {
                    newPassKey = null;
                }

                string action = newPassKey == null ? "remove" : "change";
                if (!manifest.ChangeEncryptionKey(curPassKey, newPassKey))
                {
                    MessageBox.Show("Unable to " + action + " passkey.");
                }
                else
                {
                    MessageBox.Show("Passkey successfully " + action + "d.");
                    this.loadAccountsList();
                }
            }
            else
            {
                passKey = manifest.PromptSetupPassKey();
                this.loadAccountsList();
            }
        }

        private void labelUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (newVersion == null || currentVersion == null)
            {
                checkForUpdates();
            }
            else
            {
                compareVersions();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            CopyLoginToken();
        }


        // Tool strip menu handlers

        private void menuQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuRemoveAccountFromManifest_Click(object sender, EventArgs e)
        {
            if (manifest.Encrypted)
            {
                MessageBox.Show("You cannot remove accounts from the manifest file while it is encrypted.", "Remove from manifest", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult res = MessageBox.Show("This will remove the selected account from the manifest file.\nUse this to move a maFile to another computer.\nThis will NOT delete your maFile.", "Remove from manifest", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    manifest.RemoveAccount(currentAccount, false);
                    MessageBox.Show("Account removed from manifest.\nYou can now move its maFile to another computer and import it using the File menu.", "Remove from manifest");
                    loadAccountsList();
                }
            }
        }

        private void menuLoginAgain_Click(object sender, EventArgs e)
        {
            this.PromptRefreshLogin(currentAccount);
        }

        private void menuImportAccount_Click(object sender, EventArgs e)
        {
            ImportAccountForm currentImport_maFile_Form = new ImportAccountForm();
            currentImport_maFile_Form.ShowDialog();
            loadAccountsList();
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            new SettingsForm().ShowDialog();
            manifest = Manifest.GetManifest(true);
            loadSettings();
        }

        private async void menuDeactivateAuthenticator_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;

            // Check for a valid refresh token first
            if (currentAccount.Session.IsRefreshTokenExpired())
            {
                MessageBox.Show("Your session has expired. Use the login again button under the selected account menu.", "Deactivate Authenticator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check for a valid access token, refresh it if needed
            if (currentAccount.Session.IsAccessTokenExpired())
            {
                try
                {
                    await currentAccount.Session.RefreshAccessToken();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Deactivate Authenticator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DialogResult res = MessageBox.Show("Would you like to remove Steam Guard completely?\nYes - Remove Steam Guard completely.\nNo - Switch back to Email authentication.", "Deactivate Authenticator: " + currentAccount.AccountName, MessageBoxButtons.YesNoCancel);
            int scheme = 0;
            if (res == DialogResult.Yes)
            {
                scheme = 2;
            }
            else if (res == DialogResult.No)
            {
                scheme = 1;
            }
            else if (res == DialogResult.Cancel)
            {
                scheme = 0;
            }

            if (scheme != 0)
            {
                string confCode = currentAccount.GenerateSteamGuardCode();
                InputForm confirmationDialog = new InputForm(String.Format("Removing Steam Guard from {0}. Enter this confirmation code: {1}", currentAccount.AccountName, confCode));
                confirmationDialog.ShowDialog();

                if (confirmationDialog.Canceled)
                {
                    return;
                }

                string enteredCode = confirmationDialog.txtBox.Text.ToUpper();
                if (enteredCode != confCode)
                {
                    MessageBox.Show("Confirmation codes do not match. Steam Guard not removed.");
                    return;
                }

                bool success = await currentAccount.DeactivateAuthenticator(scheme);
                if (success)
                {
                    MessageBox.Show(String.Format("Steam Guard {0}. maFile will be deleted after hitting okay. If you need to make a backup, now's the time.", (scheme == 2 ? "removed completely" : "switched to emails")));
                    this.manifest.RemoveAccount(currentAccount);
                    this.loadAccountsList();
                }
                else
                {
                    MessageBox.Show("Steam Guard failed to deactivate.");
                }
            }
            else
            {
                MessageBox.Show("Steam Guard was not removed. No action was taken.");
            }
        }

        // Tray menu handlers
        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            trayRestore_Click(sender, EventArgs.Empty);
        }

        private void trayRestore_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void trayQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void trayTradeConfirmations_Click(object sender, EventArgs e)
        {
            btnTradeConfirmations_Click(sender, e);
        }

        private void trayCopySteamGuard_Click(object sender, EventArgs e)
        {
            if (txtLoginToken.Text != "")
            {
                Clipboard.SetText(txtLoginToken.Text);
            }
        }

        private void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (notifiedAccount == null) return;
            new ConfirmationFormWeb(notifiedAccount).Show();
        }

        private void trayAccount_Click(object sender, EventArgs e)
        {
            listAccounts.SelectedItem = ((ToolStripMenuItem)sender).Text;
        }


        // Misc UI handlers
        private void listAccounts_SelectedValueChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < allAccounts.Length; i++)
            {
                // Check if index is out of bounds first
                if (i < 0 || listAccounts.SelectedIndex < 0)
                    continue;

                SteamGuardAccount account = allAccounts[i];
                if (account.AccountName == (string)listAccounts.Items[listAccounts.SelectedIndex])
                {
                    currentAccount = account;
                    loadAccountInfo();
                    loadTrayAccounts();
                    startWatchers();
                    break;
                }
            }
        }

        private void txtAccSearch_TextChanged(object sender, EventArgs e)
        {
            List<string> names = new List<string>(getAllNames());
            names = names.FindAll(new Predicate<string>(IsFilter));

            listAccounts.Items.Clear();
            listAccounts.Items.AddRange(names.ToArray());
            loadTrayAccounts();
        }


        // Timers

        private async void timerSteamGuard_Tick(object sender, EventArgs e)
        {
            showStatus("Aligning time with Steam...");
            steamTime = await TimeAligner.GetSteamTimeAsync();
            showStatus("");

            currentSteamChunk = steamTime / 30L;
            int secondsUntilChange = (int)(steamTime - (currentSteamChunk * 30L));

            loadAccountInfo();
            if (currentAccount != null)
            {
                pbTimeout.Value = 30 - secondsUntilChange;
            }
        }

        private async void timerTradesPopup_Tick(object sender, EventArgs e)
        {
            if (currentAccount == null) return;
            if (!confirmationsSemaphore.Wait(0))
            {
                return; //Only one thread may access this critical section at once. Mutex is a bad choice here because it'll cause a pileup of threads.
            }

            try
            {
                await checkConfirmations(manifest.CheckAllAccounts ? allAccounts : new SteamGuardAccount[] { currentAccount });
            }
            finally
            {
                confirmationsSemaphore.Release();
            }
        }

        private async void watcher_ConfirmationsChanged(AccountWatcher watcher)
        {
            await confirmationsSemaphore.WaitAsync();
            try
            {
                await checkConfirmations(new SteamGuardAccount[] { watcher.Account });
            }
            finally
            {
                confirmationsSemaphore.Release();
            }
        }

        private async Task checkConfirmations(SteamGuardAccount[] accs)
        {
            try
            {
                showStatus("Checking confirmations...");

                foreach (var acc in accs)
                {
                    if (acc.Session.IsRefreshTokenExpired())
                    {
                        if (expiredSessions.Add(acc.Session.SteamID))
                            Notify(acc, "Session expired", "Login again from the Selected Account menu to keep checking confirmations for " + acc.AccountName + ".");
                        continue;
                    }

                    try
                    {
                        if (acc.Session.IsAccessTokenExpired())
                        {
                            showStatus("Refreshing session...");
                            await acc.Session.RefreshAccessToken();
                            showStatus("Checking confirmations...");
                        }

                        List<Confirmation> autoAccept = new List<Confirmation>();
                        int pending = 0;

                        foreach (var conf in await acc.FetchConfirmationsAsync())
                        {
                            if ((conf.ConfType == Confirmation.EMobileConfirmationType.MarketListing && manifest.AutoConfirmMarketTransactions) ||
                                (conf.ConfType == Confirmation.EMobileConfirmationType.Trade && manifest.AutoConfirmTrades))
                            {
                                autoAccept.Add(conf);
                            }
                            else if (notifiedConfirmations.Add(conf.ID))
                            {
                                pending++;
                            }
                        }

                        if (autoAccept.Count > 0)
                            await acc.AcceptMultipleConfirmations(autoAccept.ToArray());

                        if (pending > 0)
                            Notify(acc, "New confirmations", pending + (pending == 1 ? " confirmation is" : " confirmations are") + " waiting for " + acc.AccountName + ".");
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            finally
            {
                showStatus("");
            }
        }

        // Live notifications

        private void startWatchers()
        {
            if (!manifest.LiveNotifications || allAccounts == null || currentAccount == null)
            {
                stopWatchers();
                return;
            }

            SteamGuardAccount[] wanted = manifest.CheckAllAccounts ? allAccounts : new SteamGuardAccount[] { currentAccount };
            if (wanted.Length == watchers.Count && wanted.All(a => watchers.Any(w => w.Account == a)))
                return;

            stopWatchers();
            foreach (var acc in wanted)
            {
                if (string.IsNullOrEmpty(acc.Session?.RefreshToken)) continue;
                var watcher = new AccountWatcher(acc);
                watcher.ConfirmationsChanged += watcher_ConfirmationsChanged;
                watcher.StatusChanged += watcher_StatusChanged;
                watchers.Add(watcher);
                watcher.Start();
            }
            updateLiveStatus();
        }

        private void restartWatchers()
        {
            stopWatchers();
            startWatchers();
        }

        private void stopWatchers()
        {
            foreach (var watcher in watchers)
                watcher.Dispose();
            watchers.Clear();
            updateLiveStatus();
        }

        private void watcher_StatusChanged(AccountWatcher watcher)
        {
            if (!watchers.Contains(watcher)) return;

            if (watcher.Failed && !manifest.PeriodicChecking && !timerTradesPopup.Enabled)
            {
                timerTradesPopup.Enabled = true;
                Notify(watcher.Account, "Live updates unavailable", "Steam refused the connection for " + watcher.Account.AccountName + " (" + watcher.Status + "). Checking periodically instead.");
            }
            updateLiveStatus();
        }

        private void updateLiveStatus()
        {
            if (watchers.Count == 0)
                liveStatus = "";
            else if (watchers.Any(w => w.Failed))
                liveStatus = "Live: " + watchers.First(w => w.Failed).Status;
            else if (watchers.All(w => w.Connected))
                liveStatus = "Live: connected";
            else
                liveStatus = "Live: " + watchers.First(w => !w.Connected).Status.ToLower();

            showStatus("");
        }

        private void showStatus(string text)
        {
            lblStatus.Text = text.Length > 0 ? text : liveStatus;
        }

        // Other methods

        private void Notify(SteamGuardAccount account, string title, string text)
        {
            notifiedAccount = account;
            trayIcon.BalloonTipTitle = title;
            trayIcon.BalloonTipText = text;
            trayIcon.ShowBalloonTip(10000);
        }

        private void CopyLoginToken()
        {
            string text = txtLoginToken.Text;
            if (String.IsNullOrEmpty(text))
                return;
            Clipboard.SetText(text);
        }

        /// <summary>
        /// Display a login form to the user to refresh their OAuth Token
        /// </summary>
        /// <param name="account">The account to refresh</param>
        private void PromptRefreshLogin(SteamGuardAccount account)
        {
            var loginForm = new LoginForm(LoginForm.LoginType.Refresh, account);
            loginForm.ShowDialog();
            expiredSessions.Remove(account.Session.SteamID);
            restartWatchers();
        }

        /// <summary>
        /// Load UI with the current account info, this is run every second
        /// </summary>
        private void loadAccountInfo()
        {
            if (currentAccount != null && steamTime != 0)
            {
                txtLoginToken.Text = currentAccount.GenerateSteamGuardCodeForTime(steamTime);
                lblAccount.Text = currentAccount.AccountName;
            }
        }

        /// <summary>
        /// Decrypts files and populates list UI with accounts
        /// </summary>
        private void loadAccountsList()
        {
            currentAccount = null;

            listAccounts.Items.Clear();
            listAccounts.SelectedIndex = -1;

            allAccounts = manifest.GetAllAccounts(passKey);

            if (allAccounts.Length > 0)
            {
                for (int i = 0; i < allAccounts.Length; i++)
                {
                    listAccounts.Items.Add(allAccounts[i].AccountName);
                }

                listAccounts.SelectedIndex = 0;
                listAccounts.Sorted = true;
            }
            loadTrayAccounts();
            menuDeactivateAuthenticator.Enabled = btnTradeConfirmations.Enabled = allAccounts.Length > 0;
        }

        private void loadTrayAccounts()
        {
            trayAccounts.DropDownItems.Clear();
            foreach (string name in listAccounts.Items)
            {
                var item = new ToolStripMenuItem(name);
                item.Checked = currentAccount != null && name == currentAccount.AccountName;
                item.Click += trayAccount_Click;
                trayAccounts.DropDownItems.Add(item);
            }
            trayAccounts.Enabled = trayAccounts.DropDownItems.Count > 0;
            Theme.Apply(trayAccounts.DropDown);
        }

        private void listAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    int to = listAccounts.SelectedIndex - (e.KeyCode == Keys.Up ? 1 : -1);
                    manifest.MoveEntry(listAccounts.SelectedIndex, to);
                    loadAccountsList();
                }
                return;
            }

            if (!IsKeyAChar(e.KeyCode) && !IsKeyADigit(e.KeyCode))
            {
                return;
            }

            txtAccSearch.Focus();
            txtAccSearch.Text = e.KeyCode.ToString();
            txtAccSearch.SelectionStart = 1;
        }

        private static bool IsKeyAChar(Keys key)
        {
            return key >= Keys.A && key <= Keys.Z;
        }

        private static bool IsKeyADigit(Keys key)
        {
            return (key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9);
        }

        private bool IsFilter(string f)
        {
            if (txtAccSearch.Text.StartsWith("~"))
            {
                try
                {
                    return Regex.IsMatch(f, txtAccSearch.Text);
                }
                catch (Exception)
                {
                    return true;
                }

            }
            else
            {
                return f.Contains(txtAccSearch.Text.ToLower());
            }
        }

        private string[] getAllNames()
        {
            string[] itemArray = new string[allAccounts.Length];
            for (int i = 0; i < itemArray.Length; i++)
            {
                itemArray[i] = allAccounts[i].AccountName;
            }
            return itemArray;
        }

        private void loadSettings()
        {
            timerTradesPopup.Enabled = manifest.PeriodicChecking;
            timerTradesPopup.Interval = manifest.PeriodicCheckingInterval * 1000;
            restartWatchers();
        }

        // Logic for version checking
        private Version newVersion = null;
        private Version currentVersion = null;
        private WebClient updateClient = null;
        private string updateUrl = null;
        private bool startupUpdateCheck = true;

        private void checkForUpdates()
        {
            if (updateClient != null) return;
            updateClient = new WebClient();
            updateClient.DownloadStringCompleted += UpdateClient_DownloadStringCompleted;
            updateClient.Headers.Add("Content-Type", "application/json");
            updateClient.Headers.Add("User-Agent", "Steam Desktop Authenticator");
            updateClient.DownloadStringAsync(new Uri("https://api.github.com/repos/Jessecar96/SteamDesktopAuthenticator/releases/latest"));
        }

        private void compareVersions()
        {
            if (newVersion > currentVersion)
            {
                labelUpdate.Text = "Download new version"; // Show the user a new version is available if they press no
                DialogResult updateDialog = MessageBox.Show(String.Format("A new version is available! Would you like to download it now?\nYou will update from version {0} to {1}", Application.ProductVersion, newVersion.ToString()), "New Version", MessageBoxButtons.YesNo);
                if (updateDialog == DialogResult.Yes)
                {
                    Process.Start(updateUrl);
                }
            }
            else
            {
                if (!startupUpdateCheck)
                {
                    MessageBox.Show(String.Format("You are using the latest version: {0}", Application.ProductVersion));
                }
            }

            newVersion = null; // Check the api again next time they check for updates
            updateClient = null; // Set to null to indicate it's done checking
            startupUpdateCheck = false; // Set when it's done checking on startup
        }

        private void UpdateClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                dynamic resultObject = JsonConvert.DeserializeObject(e.Result);
                newVersion = new Version(resultObject.tag_name.Value);
                currentVersion = new Version(Application.ProductVersion);
                updateUrl = resultObject.assets.First.browser_download_url.Value;
                compareVersions();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to check for updates.");
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                CopyLoginToken();
            }
        }
    }
}
