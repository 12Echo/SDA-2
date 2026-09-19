using System;
using System.Diagnostics;
using System.Windows.Forms;
using SteamAuth;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;

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
        private Dictionary<ulong, DateTime> nextCheck = new Dictionary<ulong, DateTime>();
        private HashSet<ulong> fallbackPolling = new HashSet<ulong>();
        private List<SteamGuardAccount> liveNeedsLogin = new List<SteamGuardAccount>();
        private string liveStatus = "";
        private ProfileCache profiles = new ProfileCache();
        private ConfirmationPopup popup;
        private Dictionary<ulong, TradeOfferInfo> tradeOffers = new Dictionary<ulong, TradeOfferInfo>();
        private HashSet<ulong> autoAcceptWarned = new HashSet<ulong>();
        private HashSet<ulong> rejectedSessions = new HashSet<ulong>();
        private int dragIndex = -1;
        private Point dragStart;
        private AccountItem dragItem;
        private List<AccountItem> dragOrder;
        private System.Windows.Forms.Timer dragTimer = new System.Windows.Forms.Timer { Interval = 60 };
        private bool locked;
        private bool unlocking;
        private bool unlockPrompted;
        private bool warningShown;
        private int cardHeight, searchTop, listTop;

        private long steamTime = 0;
        private long currentSteamChunk = 0;
        private int secondsLeft = 30;
        private int ticks = 0;
        private string passKey = null;
        private bool startSilent = false;

        private class AccountItem
        {
            public SteamGuardAccount Account;
            public string Name;

            public override string ToString()
            {
                return Name;
            }
        }

        public MainForm()
        {
            InitializeComponent();
            Theme.Apply(this);
            Language.Apply(this);
            Theme.Apply(menuStripTray);
            Theme.Apply(listMenu);
            Language.Apply(menuStripTray.Items);
            Language.Apply(listMenu.Items);
            Theme.ListImages(listAccounts, item => profiles.GetAvatar(((AccountItem)item).Account.Session.SteamID));
            Theme.ListBadges(listAccounts, item => badgeColor(((AccountItem)item).Account));
            profiles.Updated += profiles_Updated;
            profiles.Warning += (account, text) => Notify(account, "Account warning", text);
            dragTimer.Tick += dragTimer_Tick;
            Theme.Apply(menuGroups);
            Theme.Dropdown(btnGroup);
            Theme.ListTags(listAccounts, item => manifest?.GetEntry(((AccountItem)item).Account)?.Group);
            SystemEvents.TimeChanged += SystemEvents_Realign;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        // The clock moved or the machine woke up, so the offset to Steam's clock is stale
        private async void SystemEvents_Realign(object sender, EventArgs e)
        {
            await TimeAligner.AlignTimeAsync();
            Log.Write("Time re-aligned with Steam after a clock change or resume");
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Resume)
                SystemEvents_Realign(sender, e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Program.ShowMessage)
            {
                trayRestore_Click(this, EventArgs.Empty);
                Activate();
                return;
            }
            base.WndProc(ref m);
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
                MessageForm.Show("Unable to read your settings. Try restating SDA.", "Steam Desktop Authenticator 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Make sure we don't show that welcome dialog again
            this.manifest.FirstRun = false;
            this.manifest.Save();

            // The exe may have moved since the run key was written
            if (manifest.StartWithWindows)
                Startup.Apply(manifest);

            picAvatar.Region = new Region(Theme.RoundedRect(new Rectangle(0, 0, picAvatar.Width, picAvatar.Height), Theme.Radius));

            // Tick first time manually to sync time
            timerSteamGuard_Tick(new object(), EventArgs.Empty);

            if (manifest.Encrypted && passKey == null)
            {
                passKey = manifest.PromptForPassKey(PasskeyRecovery.Offer);
                // Recovery swaps the files out from under us, so look again before giving up
                manifest = Manifest.GetManifest();
                if (passKey == null && manifest.Encrypted)
                {
                    Application.Exit();
                    return;
                }
            }

            btnManageEncryption.Text = manifest.Encrypted ? "Manage Encryption" : "Setup Encryption";

            loadSettings();
            loadAccountsList();
            listAccounts.Focus();

            if (!startSilent)
                Backup.Remind(manifest, allAccounts);
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

        private void MainForm_Activated(object sender, EventArgs e)
        {
            if (locked && !unlockPrompted)
            {
                unlockPrompted = true;
                unlock();
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            unlockPrompted = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemEvents.TimeChanged -= SystemEvents_Realign;
            SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged;
            stopWatchers();
            // Application.Exit raises this while walking the open forms, so the popup must not be disposed from here
            if (e.CloseReason != CloseReason.ApplicationExitCall)
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
                    MessageForm.Show("Passkeys do not match.");
                    return;
                }

                if (newPassKey.Length == 0)
                {
                    newPassKey = null;
                }

                string action = newPassKey == null ? "remove" : "change";
                if (!manifest.ChangeEncryptionKey(curPassKey, newPassKey))
                {
                    MessageForm.Show("Unable to " + action + " passkey.");
                }
                else
                {
                    MessageForm.Show("Passkey successfully " + action + "d.");
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
            // Always ask GitHub again, the version noted at startup may be days old by now
            checkForUpdates();
        }

        private void lblSession_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (locked)
            {
                unlockPrompted = true;
                unlock();
                return;
            }
            if (currentAccount == null) return;
            PromptRefreshLogin(currentAccount);
            loadAccountInfo();
            listAccounts.Invalidate();
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
                MessageForm.Show("You cannot remove accounts from the manifest file while it is encrypted.", "Remove from manifest", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult res = MessageForm.Show("This will remove the selected account from the manifest file.\nUse this to move a maFile to another computer.\nThis will NOT delete your maFile.", "Remove from manifest", MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    manifest.RemoveAccount(currentAccount, false);
                    MessageForm.Show("Account removed from manifest.\nYou can now move its maFile to another computer and import it using the File menu.", "Remove from manifest");
                    loadAccountsList();
                }
            }
        }

        private void menuLoginAgain_Click(object sender, EventArgs e)
        {
            this.PromptRefreshLogin(currentAccount);
        }

        private void menuBackup_Click(object sender, EventArgs e)
        {
            if (locked) return;
            Backup.Offer(manifest, allAccounts);
        }

        private void menuLock_Click(object sender, EventArgs e)
        {
            lockNow();
        }

        private void menuGroup_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;
            var entry = manifest.GetEntry(currentAccount);
            if (entry == null) return;

            var groups = manifest.Groups();
            string hint = groups.Count > 0 ? " Existing groups: " + string.Join(", ", groups) + "." : "";
            var form = new InputForm("Group for " + displayName(currentAccount) + ", used to filter the list. Leave it blank for none." + hint);
            form.Text = Language.T("Group");
            form.txtBox.Text = entry.Group ?? "";
            form.txtBox.SelectAll();
            form.ShowDialog();
            if (form.Canceled) return;

            string group = form.txtBox.Text.Trim();
            entry.Group = group.Length > 0 ? group : null;
            manifest.Save();
            fillAccountsList();
            showGroup();
        }

        // The dropdown next to the search box narrows the list to one group and remembers the choice
        private void btnGroup_Click(object sender, EventArgs e)
        {
            menuGroups.Items.Clear();
            var all = new ToolStripMenuItem(Language.T("All accounts")) { Tag = "", Checked = string.IsNullOrEmpty(manifest.ListGroup) };
            all.Click += menuGroupItem_Click;
            menuGroups.Items.Add(all);
            var groups = manifest.Groups();
            if (groups.Count > 0)
                menuGroups.Items.Add(new ToolStripSeparator());
            foreach (string group in groups)
            {
                var item = new ToolStripMenuItem(group) { Tag = group, Checked = string.Equals(group, manifest.ListGroup, StringComparison.OrdinalIgnoreCase) };
                item.Click += menuGroupItem_Click;
                menuGroups.Items.Add(item);
            }
            Theme.StyleMenuItems(menuGroups.Items, false);
            menuGroups.Width = Math.Max(btnGroup.Width, LogicalToDeviceUnits(160));
            menuGroups.Show(btnGroup, new Point(btnGroup.Width - menuGroups.Width, btnGroup.Height + 4));
        }

        private void menuGroupItem_Click(object sender, EventArgs e)
        {
            manifest.ListGroup = (string)((ToolStripMenuItem)sender).Tag;
            manifest.Save();
            fillAccountsList();
            showGroup();
        }

        private void showGroup()
        {
            // A group that no longer exists falls back to everything
            if (!string.IsNullOrEmpty(manifest.ListGroup) && !manifest.Groups().Contains(manifest.ListGroup, StringComparer.OrdinalIgnoreCase))
            {
                manifest.ListGroup = "";
                manifest.Save();
                fillAccountsList();
            }
            btnGroup.Text = string.IsNullOrEmpty(manifest.ListGroup) ? Language.T("All accounts") : manifest.ListGroup;
            btnGroup.Enabled = manifest.Groups().Count > 0;
            listAccounts.Invalidate();
        }

        // Locking: the passkey is forgotten and everything derived from it leaves the screen, the tray and memory

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO info);

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        private static int IdleSeconds()
        {
            var info = new LASTINPUTINFO { cbSize = (uint)Marshal.SizeOf<LASTINPUTINFO>() };
            if (!GetLastInputInfo(ref info)) return 0;
            return (int)(unchecked((uint)Environment.TickCount - info.dwTime) / 1000);
        }

        private void lockNow()
        {
            if (locked || manifest == null || !manifest.Encrypted) return;
            locked = true;
            passKey = null;
            stopWatchers();
            if (popup != null && !popup.IsDisposed) popup.Close();
            allAccounts = new SteamGuardAccount[0];
            currentAccount = null;
            nextCheck.Clear();
            listAccounts.Items.Clear();
            loadTrayAccounts();

            txtLoginToken.Text = "";
            pbTimeout.Value = 0;
            picAvatar.Image = null;
            lblAccountTitle.Text = "";
            lblAccount.Text = "Locked";
            lblSession.Text = "Click to unlock";
            lblSession.LinkColor = Theme.Accent;
            lblSession.LinkArea = new LinkArea(0, lblSession.Text.Length);
            showWarning(null, Theme.Warning);
            menuDeactivateAuthenticator.Enabled = menuRecoveryKit.Enabled = btnTradeConfirmations.Enabled = false;
            menuLock.Enabled = trayLock.Enabled = false;
            Log.Write("Locked");
        }

        private void unlock()
        {
            if (!locked || unlocking) return;
            unlocking = true;
            try
            {
                string key = manifest.PromptForPassKey();
                if (key == null) return;
                passKey = key;
                locked = false;
                loadSettings();
                loadAccountsList();
                Log.Write("Unlocked");
            }
            finally
            {
                unlocking = false;
            }
        }

        private void menuRecoveryKit_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;
            RecoveryKit.Offer(currentAccount);
        }

        private void menuRename_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;
            var entry = manifest.GetEntry(currentAccount);
            if (entry == null) return;

            var form = new InputForm("Display name for " + currentAccount.AccountName + ". This only changes what SDA shows. Leave it blank to follow the Steam profile name.");
            form.Text = Language.T("Rename");
            form.txtBox.Text = displayName(currentAccount);
            form.txtBox.SelectAll();
            form.ShowDialog();
            if (form.Canceled) return;

            // Keeping the profile name as it was pre-filled means keep following the profile
            string name = form.txtBox.Text.Trim();
            string profileName = string.IsNullOrEmpty(entry.PersonaName) ? currentAccount.AccountName : entry.PersonaName;
            entry.DisplayName = name.Length > 0 && name != profileName ? name : null;
            manifest.Save();
            profiles_Updated();
        }

        private async void menuApproveQr_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;
            await ApproveQr(currentAccount);
        }

        // Scans the screens for a Steam login QR code and approves it for this account, like the mobile app does
        private async Task ApproveQr(SteamGuardAccount account)
        {
            if (account.Session.IsRefreshTokenExpired())
            {
                MessageForm.Show("Your session has expired. Login again from the Selected Account menu first.", "Approve login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            showStatus("Looking for a login QR code...");
            QrLoginRequest login = await Task.Run(() => QrLogin.FindOnScreen());
            if (login == null)
            {
                showStatus("");
                MessageForm.Show("No Steam login QR code is visible on your screens. Open the Steam sign in page or the Steam client so the QR code is on screen, then try again.", "Approve login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SteamKit2.Internal.CAuthentication_GetAuthSessionInfo_Response info;
            try
            {
                showStatus("Asking Steam about the login...");
                info = await QrLogin.GetInfoAsync(account, login.ClientId);
            }
            catch (Exception ex)
            {
                showStatus("");
                MessageForm.Show("Steam did not recognise that QR code. It may have expired, refresh the sign in page and try again.\n" + ex.Message, "Approve login", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            showStatus("");

            var answer = MessageForm.Show("Sign in to " + displayName(account) + " on " + QrLogin.Describe(info) + "?\n\nOnly approve logins you started yourself.", "Approve login", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            if (answer == DialogResult.Cancel) return;

            try
            {
                await QrLogin.ApproveAsync(account, login, answer == DialogResult.Yes);
                MessageForm.Show(answer == DialogResult.Yes ? "Login approved. The other device finishes signing in on its own." : "Login denied.", "Approve login", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageForm.Show("Could not send the answer to Steam: " + ex.Message, "Approve login", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void menuImportAccount_Click(object sender, EventArgs e)
        {
            ImportAccountForm currentImport_maFile_Form = new ImportAccountForm();
            currentImport_maFile_Form.ShowDialog();
            loadAccountsList();
        }

        private void menuSettings_Click(object sender, EventArgs e)
        {
            new SettingsForm(allAccounts).ShowDialog();
            manifest = Manifest.GetManifest(true);
            loadSettings();
            promptClientLogins();
        }

        private async void menuDeactivateAuthenticator_Click(object sender, EventArgs e)
        {
            if (currentAccount == null) return;

            // Check for a valid refresh token first
            if (currentAccount.Session.IsRefreshTokenExpired())
            {
                MessageForm.Show("Your session has expired. Use the login again button under the selected account menu.", "Deactivate Authenticator", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageForm.Show(ex.Message, "Deactivate Authenticator Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            DialogResult res = MessageForm.Show("Would you like to remove Steam Guard completely?\nYes - Remove Steam Guard completely.\nNo - Switch back to Email authentication.", "Deactivate Authenticator: " + currentAccount.AccountName, MessageBoxButtons.YesNoCancel);
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
                    MessageForm.Show("Confirmation codes do not match. Steam Guard not removed.");
                    return;
                }

                bool success = await currentAccount.DeactivateAuthenticator(scheme);
                if (success)
                {
                    MessageForm.Show(String.Format("Steam Guard {0}. maFile will be deleted after hitting okay. If you need to make a backup, now's the time.", (scheme == 2 ? "removed completely" : "switched to emails")));
                    this.manifest.RemoveAccount(currentAccount);
                    this.loadAccountsList();
                }
                else
                {
                    MessageForm.Show("Steam Guard failed to deactivate.");
                }
            }
            else
            {
                MessageForm.Show("Steam Guard was not removed. No action was taken.");
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

        private void menuStripTray_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            loadTrayCode();
            trayTradeConfirmations.Enabled = trayApproveQr.Enabled = currentAccount != null;
            trayCheckNow.Enabled = allAccounts != null && allAccounts.Length > 0;
        }

        private void listMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = currentAccount == null;
        }

        private async void trayCheckNow_Click(object sender, EventArgs e)
        {
            if (allAccounts == null) return;
            var accs = allAccounts.Where(a => manifest.GetEntry(a)?.Confirmations != ConfirmationMode.Off).ToArray();
            if (accs.Length == 0 && currentAccount != null)
                accs = new SteamGuardAccount[] { currentAccount };
            if (accs.Length == 0) return;

            await confirmationsSemaphore.WaitAsync();
            try
            {
                int shown = await checkConfirmations(accs, true);
                if (shown == 0)
                    Notify(null, "Nothing waiting", accs.Length == 1 ? "No confirmations are waiting for " + displayName(accs[0]) + "." : "No confirmations are waiting on your " + accs.Length + " accounts.");
            }
            finally
            {
                confirmationsSemaphore.Release();
            }
        }

        private void loadTrayCode()
        {
            string code = txtLoginToken.Text;
            trayCopySteamGuard.Text = code.Length > 0 ? "Copy login code    " + code : "Copy login code";
            trayCopySteamGuard.Enabled = code.Length > 0;
            trayCopySteamGuard.Tag = code.Length > 0 ? (object)secondsLeft : null;
            trayCopySteamGuard.Invalidate();
        }

        private void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (notifiedAccount == null) return;
            new ConfirmationFormWeb(notifiedAccount).Show();
        }

        private void trayAccount_Click(object sender, EventArgs e)
        {
            selectAccount((SteamGuardAccount)((ToolStripMenuItem)sender).Tag);
        }


        // Misc UI handlers
        private void listAccounts_SelectedValueChanged(object sender, EventArgs e)
        {
            var item = listAccounts.SelectedItem as AccountItem;
            if (item == null) return;

            currentAccount = item.Account;
            loadAccountInfo();
            loadTrayAccounts();
        }

        private void txtAccSearch_TextChanged(object sender, EventArgs e)
        {
            fillAccountsList();
        }

        private void profiles_Updated()
        {
            foreach (AccountItem item in listAccounts.Items)
                item.Name = displayName(item.Account);
            listAccounts.Invalidate();
            loadAccountInfo();
            loadTrayAccounts();
        }


        // Timers

        private async void timerSteamGuard_Tick(object sender, EventArgs e)
        {
            showStatus("Aligning time with Steam...");
            steamTime = await TimeAligner.GetSteamTimeAsync();
            showStatus("");

            // Re-align every hour so a machine left running does not drift into bad codes
            if (++ticks % 3600 == 0)
            {
                await TimeAligner.AlignTimeAsync();
                _ = profiles.RefreshAsync(manifest, allAccounts);
            }
            if (!locked && !unlocking && manifest != null && manifest.Encrypted && manifest.LockAfterMinutes > 0 && IdleSeconds() >= manifest.LockAfterMinutes * 60)
                lockNow();
            if (ticks % 86400 == 0)
                _ = noteNewerRelease();

            currentSteamChunk = steamTime / 30L;
            int secondsUntilChange = (int)(steamTime - (currentSteamChunk * 30L));

            secondsLeft = 30 - secondsUntilChange;
            loadAccountInfo();
            if (currentAccount != null)
            {
                pbTimeout.Value = secondsLeft;
            }
            if (menuStripTray.Visible)
            {
                loadTrayCode();
            }
        }

        private async void timerTradesPopup_Tick(object sender, EventArgs e)
        {
            if (allAccounts == null) return;

            DateTime now = DateTime.UtcNow;
            List<SteamGuardAccount> due = new List<SteamGuardAccount>();
            foreach (var acc in allAccounts)
            {
                var entry = manifest.GetEntry(acc);
                if (entry == null) continue;

                bool poll = entry.Confirmations == ConfirmationMode.Periodic
                    || (entry.Confirmations == ConfirmationMode.Live && fallbackPolling.Contains(acc.Session.SteamID));
                if (!poll) continue;

                DateTime at;
                if (!nextCheck.TryGetValue(acc.Session.SteamID, out at))
                    nextCheck[acc.Session.SteamID] = now.AddSeconds(2 * nextCheck.Count);
                else if (now >= at)
                    due.Add(acc);
            }

            if (due.Count == 0) return;
            if (!confirmationsSemaphore.Wait(0))
            {
                return; //Only one thread may access this critical section at once. Mutex is a bad choice here because it'll cause a pileup of threads.
            }

            try
            {
                foreach (var acc in due)
                    nextCheck[acc.Session.SteamID] = now.AddSeconds(Math.Max(5, manifest.GetEntry(acc).CheckInterval));

                await checkConfirmations(due.ToArray());
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

        private async Task<int> checkConfirmations(SteamGuardAccount[] accs, bool force = false)
        {
            int shown = 0;
            try
            {
                showStatus("Checking confirmations...");

                for (int i = 0; i < accs.Length; i++)
                {
                    var acc = accs[i];
                    var entry = manifest.GetEntry(acc);
                    if (entry == null) continue;
                    if (i > 0) await Task.Delay(1500);

                    if (acc.Session.IsRefreshTokenExpired())
                    {
                        if (expiredSessions.Add(acc.Session.SteamID) || force)
                            Notify(acc, "Session expired", "Login again from the Selected Account menu to keep checking confirmations for " + displayName(acc) + ".");
                        continue;
                    }

                    if (acc.Session.IsAccessTokenExpired())
                    {
                        showStatus("Refreshing session...");
                        try
                        {
                            await acc.Session.RefreshAccessToken();
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Refreshing the session for " + acc.AccountName, ex);
                            if (SessionData.IsTokenRejected(ex) && rejectedSessions.Add(acc.Session.SteamID))
                            {
                                Notify(acc, "Login no longer valid", "Steam rejected the saved login for " + displayName(acc) + ", the password may have changed. Login again from the Selected Account menu.");
                                loadAccountInfo();
                                listAccounts.Invalidate();
                            }
                            continue;
                        }
                        showStatus("Checking confirmations...");
                    }

                    try
                    {
                        List<Confirmation> autoAccept = new List<Confirmation>();
                        List<Confirmation> fresh = new List<Confirmation>();

                        foreach (var conf in await acc.FetchConfirmationsAsync())
                        {
                            if (await ShouldAutoAccept(acc, entry, conf))
                                autoAccept.Add(conf);
                            else if (notifiedConfirmations.Add(conf.ID) || force)
                                fresh.Add(conf);
                        }

                        if (autoAccept.Count > 0 && !await acc.AcceptMultipleConfirmations(autoAccept.ToArray()))
                        {
                            if (autoAcceptWarned.Add(acc.Session.SteamID))
                                Notify(acc, "Auto accept failed", "Steam refused to accept confirmations for " + displayName(acc) + ". " + ConfirmationFormWeb.TradeProtectionHint);
                        }

                        if (fresh.Count > 0)
                        {
                            shown += fresh.Count;
                            if (manifest.NotificationStyle == NotificationStyle.Popup)
                            {
                                if (popup == null || popup.IsDisposed)
                                    popup = new ConfirmationPopup();
                                popup.Queue(displayName(acc), acc, fresh);
                            }
                            else
                            {
                                Notify(acc, "New confirmations", fresh.Count + (fresh.Count == 1 ? " confirmation is" : " confirmations are") + " waiting for " + displayName(acc) + ".");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Checking confirmations for " + acc.AccountName, ex);
                    }
                }
            }
            finally
            {
                showStatus("");
            }
            return shown;
        }

        // Market listings are a plain switch, trades can be limited to safe ones; anything unreadable is left for the user
        private async Task<bool> ShouldAutoAccept(SteamGuardAccount acc, Manifest.ManifestEntry entry, Confirmation conf)
        {
            if (conf.ConfType == Confirmation.EMobileConfirmationType.MarketListing)
                return entry.AutoConfirmMarket;
            if (conf.ConfType != Confirmation.EMobileConfirmationType.Trade || !entry.AutoConfirmTrades)
                return false;
            if (!entry.AutoConfirmTradesReceiveOnly && !entry.AutoConfirmTradesPartnersOnly)
                return true;

            TradeOfferInfo offer;
            if (!tradeOffers.TryGetValue(conf.ID, out offer))
            {
                offer = await TradeOffers.ReadAsync(acc, conf);
                if (offer != null) tradeOffers[conf.ID] = offer;
            }
            if (offer == null) return false;

            if (entry.AutoConfirmTradesReceiveOnly && offer.Giving > 0) return false;
            if (entry.AutoConfirmTradesPartnersOnly && !entry.AutoConfirmTradePartners.Contains(offer.Partner)) return false;
            return true;
        }

        // Live notifications

        private void startWatchers()
        {
            if (allAccounts == null)
            {
                stopWatchers();
                return;
            }

            var live = allAccounts.Where(a => manifest.GetEntry(a)?.Confirmations == ConfirmationMode.Live).ToArray();
            var wanted = live.Where(a => !string.IsNullOrEmpty(a.Session?.ClientRefreshToken)).ToArray();

            liveNeedsLogin = live.Except(wanted).ToList();
            foreach (var acc in liveNeedsLogin)
                fallbackPolling.Add(acc.Session.SteamID);

            if (wanted.Length == watchers.Count && wanted.All(a => watchers.Any(w => w.Account == a)))
            {
                updateLiveStatus();
                return;
            }

            stopWatchers();
            foreach (var acc in wanted)
            {
                fallbackPolling.Remove(acc.Session.SteamID);
                var watcher = new AccountWatcher(acc);
                watcher.ConfirmationsChanged += watcher_ConfirmationsChanged;
                watcher.StatusChanged += watcher_StatusChanged;
                watchers.Add(watcher);
                watcher.Start(2 * (watchers.Count - 1));
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

            if (watcher.Failed && fallbackPolling.Add(watcher.Account.Session.SteamID))
            {
                Log.Write("Live connection for " + watcher.Account.AccountName + " refused: " + watcher.Status);
                Notify(watcher.Account, "Live updates unavailable", "Steam refused the connection for " + displayName(watcher.Account) + " (" + watcher.Status + "). Checking periodically instead.");
            }
            updateLiveStatus();
        }

        private void promptClientLogins()
        {
            foreach (var acc in liveNeedsLogin.ToArray())
            {
                var result = MessageForm.Show("Staying connected to Steam needs a Steam client session for " + displayName(acc) + ". Login again now to set it up?", "Steam Desktop Authenticator 2", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                    PromptRefreshLogin(acc);
            }
        }

        private void updateLiveStatus()
        {
            if (watchers.Count == 0 && liveNeedsLogin.Count == 0)
                liveStatus = "";
            else if (watchers.Any(w => w.Failed))
                liveStatus = "Live: " + watchers.First(w => w.Failed).Status;
            else if (liveNeedsLogin.Count > 0)
                liveStatus = "Live: login again to enable";
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

        private string displayName(SteamGuardAccount account)
        {
            return manifest == null ? account.AccountName : manifest.GetDisplayName(account);
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
            rejectedSessions.Remove(account.Session.SteamID);
            restartWatchers();
        }

        /// <summary>
        /// Load UI with the current account info, this is run every second
        /// </summary>
        private void loadAccountInfo()
        {
            if (currentAccount == null || steamTime == 0) return;

            txtLoginToken.Text = currentAccount.GenerateSteamGuardCodeForTime(steamTime);
            lblAccount.Text = displayName(currentAccount);
            lblAccountTitle.Text = currentAccount.AccountName;
            picAvatar.Image = profiles.GetAvatar(currentAccount.Session.SteamID);

            Color color;
            string session = sessionText(currentAccount, out color);
            if (lblSession.Text != session)
            {
                lblSession.Text = session;
                lblSession.LinkColor = color;
                lblSession.LinkArea = color == Theme.TextMuted ? new LinkArea(0, 0) : new LinkArea(0, session.Length);
            }

            showWarning(accountWarning(currentAccount, out color), color);
        }

        private string sessionText(SteamGuardAccount account, out Color color)
        {
            var expiry = account.Session.GetRefreshTokenExpiry();
            color = Theme.Danger;
            if (rejectedSessions.Contains(account.Session.SteamID))
                return "Steam rejected the login, login again";
            if (expiry == null || expiry <= DateTimeOffset.UtcNow)
                return "Session expired, login again";

            color = Theme.Warning;
            if (expiry < DateTimeOffset.UtcNow.AddDays(7))
            {
                int days = (int)Math.Ceiling((expiry.Value - DateTimeOffset.UtcNow).TotalDays);
                return "Session expires in " + days + (days == 1 ? " day, login again" : " days, login again");
            }

            color = Theme.TextMuted;
            return "Session active";
        }

        // Anything Steam holds against the account, bans first, then the things that pass on their own
        private string accountWarning(SteamGuardAccount account, out Color color)
        {
            var entry = manifest?.GetEntry(account);
            color = Theme.Danger;
            if (entry != null)
            {
                if (entry.TradeBan == "Banned") return "Trade banned";
                if (entry.VacBanned && entry.GameBans > 0) return "VAC banned and game banned";
                if (entry.VacBanned) return "VAC banned";
                if (entry.GameBans == 1) return "Game banned";
                if (entry.GameBans > 1) return entry.GameBans + " game bans on record";

                color = Theme.Warning;
                if (entry.TradeBan == "Probation") return "Trade ban probation";
                if (entry.LimitedAccount) return "Limited account, cannot trade or use the Market";
            }

            color = Theme.Warning;
            var holdsEnd = TradeHoldsEnd(account);
            if (holdsEnd > DateTimeOffset.UtcNow)
                return "Trade holds end in " + Remaining(holdsEnd - DateTimeOffset.UtcNow);
            return null;
        }

        // Steam holds trades made in the first week after an authenticator is added
        internal static DateTimeOffset TradeHoldsEnd(SteamGuardAccount account)
        {
            if (account.ServerTime <= 0) return DateTimeOffset.MinValue;
            return DateTimeOffset.FromUnixTimeSeconds(account.ServerTime).AddDays(7);
        }

        internal static string Remaining(TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                int days = (int)Math.Round(span.TotalDays, MidpointRounding.AwayFromZero);
                return days == 1 ? "1 day" : days + " days";
            }
            if (span.TotalHours >= 1)
            {
                int hours = (int)Math.Round(span.TotalHours, MidpointRounding.AwayFromZero);
                return hours == 1 ? "1 hour" : hours + " hours";
            }
            int minutes = Math.Max(1, (int)Math.Ceiling(span.TotalMinutes));
            return minutes == 1 ? "1 minute" : minutes + " minutes";
        }

        private Color? badgeColor(SteamGuardAccount account)
        {
            Color session, warning;
            sessionText(account, out session);
            string text = accountWarning(account, out warning);
            if (session == Theme.Danger || (text != null && warning == Theme.Danger)) return Theme.Danger;
            if (session == Theme.Warning || text != null) return Theme.Warning;
            return null;
        }

        // The warning row sits under the account card and pushes the list down while it is showing
        private void showWarning(string text, Color color)
        {
            bool show = text != null;
            if (show)
            {
                if (lblWarning.Text != text) lblWarning.Text = text;
                if (lblWarning.ForeColor != color)
                {
                    lblWarning.ForeColor = color;
                    lblWarning.Invalidate();
                }
            }
            // Visible reads false on every child while the window is hidden in the tray, so the row keeps its own flag
            // and the layout is set from the designer positions rather than nudged, otherwise the card grows every tick
            if (show == warningShown) return;
            if (cardHeight == 0)
            {
                cardHeight = groupAccount.Height;
                searchTop = panelSearch.Top;
                listTop = listAccounts.Top;
            }
            warningShown = show;

            int extra = show ? LogicalToDeviceUnits(26) : 0;
            int listBottom = listAccounts.Bottom;
            groupAccount.Height = cardHeight + extra;
            panelSearch.Top = btnGroup.Top = searchTop + extra;
            listAccounts.Top = listTop + extra;
            listAccounts.Height = listBottom - listAccounts.Top;
            lblWarning.Visible = show;
            groupAccount.Invalidate();
        }

        private void lblWarning_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int d = LogicalToDeviceUnits(8);
            using (var brush = new SolidBrush(lblWarning.ForeColor))
                e.Graphics.FillEllipse(brush, 0, (lblWarning.Height - d) / 2, d, d);
        }

        /// <summary>
        /// Decrypts files and populates list UI with accounts
        /// </summary>
        private void loadAccountsList()
        {
            ulong selected = currentAccount?.Session.SteamID ?? 0;
            currentAccount = null;

            allAccounts = manifest.GetAllAccounts(passKey);
            fillAccountsList(selected);

            menuDeactivateAuthenticator.Enabled = menuRecoveryKit.Enabled = btnTradeConfirmations.Enabled = allAccounts.Length > 0;
            btnManageEncryption.Enabled = manifest.Entries.Count > 0;
            btnManageEncryption.Text = manifest.Encrypted ? "Manage Encryption" : "Setup Encryption";
            menuLock.Enabled = trayLock.Enabled = manifest.Encrypted && !locked;
            showGroup();
            startWatchers();
            _ = profiles.RefreshAsync(manifest, allAccounts);
        }

        private void fillAccountsList(ulong keepSelected = 0)
        {
            if (keepSelected == 0 && currentAccount != null)
                keepSelected = currentAccount.Session.SteamID;

            listAccounts.BeginUpdate();
            listAccounts.Items.Clear();
            foreach (var account in allAccounts)
            {
                var item = new AccountItem { Account = account, Name = displayName(account) };
                if (IsFilter(item))
                    listAccounts.Items.Add(item);
            }
            listAccounts.EndUpdate();

            if (listAccounts.Items.Count > 0)
            {
                int index = 0;
                for (int i = 0; i < listAccounts.Items.Count; i++)
                {
                    if (((AccountItem)listAccounts.Items[i]).Account.Session.SteamID == keepSelected)
                        index = i;
                }
                listAccounts.SelectedIndex = index;
            }
            else
            {
                loadTrayAccounts();
            }
        }

        private void selectAccount(SteamGuardAccount account)
        {
            for (int i = 0; i < listAccounts.Items.Count; i++)
            {
                if (((AccountItem)listAccounts.Items[i]).Account == account)
                {
                    listAccounts.SelectedIndex = i;
                    return;
                }
            }
        }

        private void loadTrayAccounts()
        {
            trayAccounts.DropDownItems.Clear();
            foreach (AccountItem entry in listAccounts.Items)
            {
                var item = new ToolStripMenuItem(entry.Name);
                item.Tag = entry.Account;
                item.Checked = entry.Account == currentAccount;
                item.Click += trayAccount_Click;
                trayAccounts.DropDownItems.Add(item);
            }
            trayAccounts.Enabled = trayAccounts.DropDownItems.Count > 0;
            Theme.Apply(trayAccounts.DropDown);
        }

        private void listAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && dragItem != null)
            {
                cancelDrag();
                e.Handled = true;
                return;
            }

            if (e.Control && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                int to = listAccounts.SelectedIndex + (e.KeyCode == Keys.Up ? -1 : 1);
                if (listAccounts.SelectedIndex >= 0 && to >= 0 && to < listAccounts.Items.Count)
                    moveAccount((AccountItem)listAccounts.SelectedItem, (AccountItem)listAccounts.Items[to]);
                e.Handled = true;
                return;
            }

            if (e.Control || (!IsKeyAChar(e.KeyCode) && !IsKeyADigit(e.KeyCode)))
            {
                return;
            }

            txtAccSearch.Focus();
            txtAccSearch.Text = e.KeyCode.ToString();
            txtAccSearch.SelectionStart = 1;
        }

        // Reordering: the pressed item follows the mouse through the list and the manifest is updated on release

        private void listAccounts_MouseDown(object sender, MouseEventArgs e)
        {
            int index = listAccounts.IndexFromPoint(e.Location);
            if (e.Button == MouseButtons.Right)
            {
                if (index >= 0) listAccounts.SelectedIndex = index;
                return;
            }
            dragIndex = e.Button == MouseButtons.Left ? index : -1;
            dragStart = e.Location;
        }

        private void listAccounts_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (dragItem != null)
            {
                dragTo(e.Location);
                return;
            }

            if (dragIndex < 0 || listAccounts.Items.Count < 2) return;
            var drag = SystemInformation.DragSize;
            if (Math.Abs(e.X - dragStart.X) < drag.Width && Math.Abs(e.Y - dragStart.Y) < drag.Height) return;

            dragItem = (AccountItem)listAccounts.Items[dragIndex];
            dragOrder = listAccounts.Items.Cast<AccountItem>().ToList();
            dragIndex = -1;
            listAccounts.Cursor = Cursors.SizeAll;
            Theme.ListDragging(listAccounts, listAccounts.Items.IndexOf(dragItem));
            dragTimer.Start();
            dragTo(e.Location);
        }

        private void listAccounts_MouseUp(object sender, MouseEventArgs e)
        {
            dragIndex = -1;
            if (dragItem == null || e.Button != MouseButtons.Left) return;

            var item = dragItem;
            var order = dragOrder;
            int from = order.IndexOf(item);
            int to = listAccounts.Items.IndexOf(item);
            endDrag();
            if (to != from && to >= 0)
                moveAccount(item, order[to]);
        }

        private void listAccounts_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (dragItem != null && !listAccounts.Capture)
                cancelDrag();
        }

        // Keeps the list moving while the mouse rests above or below it
        private void dragTimer_Tick(object sender, EventArgs e)
        {
            if (dragItem == null) return;
            if (MouseButtons != MouseButtons.Left)
            {
                cancelDrag();
                return;
            }
            var point = listAccounts.PointToClient(Cursor.Position);
            if (point.Y < 0 || point.Y >= listAccounts.ClientSize.Height)
                dragTo(point);
        }

        private void dragTo(Point point)
        {
            int current = listAccounts.Items.IndexOf(dragItem);
            int target;
            if (point.Y < 0)
                target = Math.Max(0, Math.Min(current - 1, listAccounts.TopIndex - 1));
            else if (point.Y >= listAccounts.ClientSize.Height)
                target = Math.Min(listAccounts.Items.Count - 1, current + 1);
            else
            {
                target = listAccounts.IndexFromPoint(new Point(listAccounts.ClientSize.Width / 2, point.Y));
                if (target < 0) target = listAccounts.Items.Count - 1;
            }
            if (target == current || current < 0) return;

            listAccounts.BeginUpdate();
            listAccounts.Items.RemoveAt(current);
            listAccounts.Items.Insert(target, dragItem);
            listAccounts.SelectedIndex = target;
            listAccounts.EndUpdate();
            Theme.ListDragging(listAccounts, target);
        }

        private void cancelDrag()
        {
            if (dragItem == null) return;
            var item = dragItem;
            var order = dragOrder;
            endDrag();

            listAccounts.BeginUpdate();
            listAccounts.Items.Clear();
            foreach (var entry in order)
                listAccounts.Items.Add(entry);
            listAccounts.SelectedItem = item;
            listAccounts.EndUpdate();
        }

        private void endDrag()
        {
            dragTimer.Stop();
            dragItem = null;
            dragOrder = null;
            listAccounts.Cursor = Cursors.Default;
            Theme.ListDragging(listAccounts, -1);
        }

        // Puts an account where another one sits, in the manifest and in the list
        private void moveAccount(AccountItem item, AccountItem target)
        {
            if (item == null || target == null || item == target) return;
            int fromEntry = manifest.Entries.IndexOf(manifest.GetEntry(item.Account));
            int toEntry = manifest.Entries.IndexOf(manifest.GetEntry(target.Account));
            if (fromEntry < 0 || toEntry < 0) return;
            manifest.MoveEntry(fromEntry, toEntry);

            var order = manifest.Entries.Select(en => en.SteamID).ToList();
            allAccounts = allAccounts.OrderBy(a => order.IndexOf(a.Session.SteamID)).ToArray();
            fillAccountsList(item.Account.Session.SteamID);
        }

        private static bool IsKeyAChar(Keys key)
        {
            return key >= Keys.A && key <= Keys.Z;
        }

        private static bool IsKeyADigit(Keys key)
        {
            return (key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9);
        }

        private bool IsFilter(AccountItem item)
        {
            if (!string.IsNullOrEmpty(manifest.ListGroup))
            {
                var entry = manifest.GetEntry(item.Account);
                if (!string.Equals(entry?.Group, manifest.ListGroup, StringComparison.OrdinalIgnoreCase)) return false;
            }

            string filter = txtAccSearch.Text;
            if (filter.Length == 0) return true;

            if (filter.StartsWith("~"))
            {
                try
                {
                    return Regex.IsMatch(item.Name, filter.Substring(1)) || Regex.IsMatch(item.Account.AccountName, filter.Substring(1));
                }
                catch (Exception)
                {
                    return true;
                }
            }

            return item.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0
                || item.Account.AccountName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void loadSettings()
        {
            timerTradesPopup.Interval = 1000;
            timerTradesPopup.Enabled = true;
            nextCheck.Clear();
            restartWatchers();
        }

        // Update checks, silent at startup and spoken when the user asks
        private Release latestRelease;
        private bool startupUpdateCheck = true;
        private bool updating;

        private async void checkForUpdates()
        {
            if (updating) return;
            bool silent = startupUpdateCheck;
            startupUpdateCheck = false;
            if (silent && !manifest.CheckUpdates) return;

            try
            {
                latestRelease = await Updater.CheckAsync();
            }
            catch (Exception ex)
            {
                latestRelease = null;
                Log.Error("Update check", ex);
                if (!silent)
                    MessageForm.Show("Could not check for updates. Try again later.", "Updates", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (latestRelease.Version > Updater.Current)
            {
                labelUpdate.Text = "Update to " + latestRelease.Version;
                labelUpdate.LinkArea = new LinkArea(0, labelUpdate.Text.Length);
                await offerUpdate(latestRelease);
            }
            else if (!silent)
            {
                MessageForm.Show("You are using the latest version, " + Updater.Current + ".", "Updates", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // A copy left running for weeks still learns about new versions, through the link text only, no dialog
        private async Task noteNewerRelease()
        {
            if (updating || manifest == null || !manifest.CheckUpdates) return;
            try
            {
                latestRelease = await Updater.CheckAsync();
            }
            catch (Exception)
            {
                return;
            }
            if (latestRelease.Version > Updater.Current)
            {
                labelUpdate.Text = "Update to " + latestRelease.Version;
                labelUpdate.LinkArea = new LinkArea(0, labelUpdate.Text.Length);
            }
        }

        private async Task offerUpdate(Release release)
        {
            var answer = MessageForm.Show("Version " + release.Version + " is available, you have " + Updater.Current + ".\nDownload and install it now? SDA restarts when it is done and your accounts stay where they are.", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (answer != DialogResult.Yes) return;

            if (release.ZipUrl == null)
            {
                Startup.OpenUrl(release.PageUrl);
                return;
            }

            updating = true;
            try
            {
                var progress = new Progress<string>(text => showStatus(text));
                string files = await Updater.DownloadAsync(release, progress);
                Updater.Apply(files);
            }
            catch (Exception ex)
            {
                showStatus("");
                Log.Error("Update to " + release.Version, ex);
                MessageForm.Show("The update could not be installed: " + ex.Message + "\nYou can download it from the releases page instead.", "Update available", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Startup.OpenUrl(release.PageUrl);
                return;
            }
            finally
            {
                updating = false;
            }
            Application.Exit();
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
