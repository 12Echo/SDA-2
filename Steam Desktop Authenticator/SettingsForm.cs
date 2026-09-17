using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    public partial class SettingsForm : Form
    {
        Manifest manifest;
        SteamGuardAccount[] accounts;
        Manifest.ManifestEntry current;
        int selected = -1;
        bool loading = false;
        string language;

        public SettingsForm(SteamGuardAccount[] accounts)
        {
            InitializeComponent();
            Theme.Apply(this);
            Language.Apply(this);
            Theme.Apply(menuAccounts);
            Theme.Apply(menuLanguages);
            Theme.Dropdown(btnAccount);
            Theme.Dropdown(btnLanguage);

            manifest = Manifest.GetManifest(true);
            this.accounts = accounts ?? new SteamGuardAccount[0];

            chkStartWithWindows.Checked = manifest.StartWithWindows;
            chkStartMinimized.Checked = manifest.StartMinimized;
            chkCheckUpdates.Checked = manifest.CheckUpdates;
            radNotifyPopup.Checked = manifest.NotificationStyle == NotificationStyle.Popup;
            radNotifyWindows.Checked = !radNotifyPopup.Checked;

            foreach (var account in this.accounts)
            {
                var item = new ToolStripMenuItem(manifest.GetDisplayName(account));
                item.Click += menuAccount_Click;
                menuAccounts.Items.Add(item);
            }
            Theme.StyleMenuItems(menuAccounts.Items, false);

            var english = new ToolStripMenuItem("English") { Tag = "" };
            english.Click += menuLanguage_Click;
            menuLanguages.Items.Add(english);
            foreach (var available in Language.Available())
            {
                var item = new ToolStripMenuItem(available.Value) { Tag = available.Key };
                item.Click += menuLanguage_Click;
                menuLanguages.Items.Add(item);
            }
            Theme.StyleMenuItems(menuLanguages.Items, false);
            SelectLanguage(manifest.Language ?? "");

            SelectAccount(this.accounts.Length > 0 ? 0 : -1);
            this.ActiveControl = btnSave;
        }

        private void SelectAccount(int index)
        {
            StoreEntry();
            selected = index;

            for (int i = 0; i < menuAccounts.Items.Count; i++)
                ((ToolStripMenuItem)menuAccounts.Items[i]).Checked = i == index;

            btnAccount.Text = index >= 0 ? manifest.GetDisplayName(accounts[index]) : Language.T("No accounts");
            btnAccount.Enabled = accounts.Length > 0;
            LoadEntry(index >= 0 ? manifest.GetEntry(accounts[index]) : null);
        }

        private void SelectLanguage(string code)
        {
            language = code;
            foreach (ToolStripMenuItem item in menuLanguages.Items)
            {
                item.Checked = (string)item.Tag == code;
                if (item.Checked) btnLanguage.Text = item.Text;
            }
        }

        private void LoadEntry(Manifest.ManifestEntry entry)
        {
            loading = true;
            current = entry;

            if (entry != null)
            {
                radOff.Checked = entry.Confirmations == ConfirmationMode.Off;
                radPeriodic.Checked = entry.Confirmations == ConfirmationMode.Periodic;
                radLive.Checked = entry.Confirmations == ConfirmationMode.Live;
                numPeriodicInterval.Value = Math.Max(numPeriodicInterval.Minimum, Math.Min(numPeriodicInterval.Maximum, entry.CheckInterval));
                chkConfirmTrades.Checked = entry.AutoConfirmTrades;
                chkConfirmMarket.Checked = entry.AutoConfirmMarket;
                chkReceiveOnly.Checked = entry.AutoConfirmTradesReceiveOnly;
                chkPartnersOnly.Checked = entry.AutoConfirmTradesPartnersOnly;
                txtPartners.Text = string.Join(Environment.NewLine, entry.AutoConfirmTradePartners ?? new List<ulong>());
            }

            loading = false;
            SetControlsEnabledState();
        }

        private void StoreEntry()
        {
            if (current == null) return;

            current.Confirmations = radLive.Checked ? ConfirmationMode.Live : radPeriodic.Checked ? ConfirmationMode.Periodic : ConfirmationMode.Off;
            current.CheckInterval = (int)numPeriodicInterval.Value;
            current.AutoConfirmTrades = chkConfirmTrades.Checked;
            current.AutoConfirmMarket = chkConfirmMarket.Checked;
            current.AutoConfirmTradesReceiveOnly = chkReceiveOnly.Checked;
            current.AutoConfirmTradesPartnersOnly = chkPartnersOnly.Checked;
            current.AutoConfirmTradePartners = ParsePartners(txtPartners.Text);
        }

        // Accepts SteamID64s separated by anything, and profile links that contain one
        private static List<ulong> ParsePartners(string text)
        {
            var ids = new List<ulong>();
            foreach (var match in System.Text.RegularExpressions.Regex.Matches(text ?? "", @"7656\d{13}").Cast<System.Text.RegularExpressions.Match>())
            {
                ulong id = ulong.Parse(match.Value);
                if (!ids.Contains(id)) ids.Add(id);
            }
            return ids;
        }

        private void SetControlsEnabledState()
        {
            bool hasAccount = current != null;
            bool checking = hasAccount && !radOff.Checked;
            radOff.Enabled = radPeriodic.Enabled = radLive.Enabled = hasAccount;
            numPeriodicInterval.Enabled = hasAccount && radPeriodic.Checked;
            chkConfirmTrades.Enabled = chkConfirmMarket.Enabled = checking;
            chkReceiveOnly.Enabled = chkPartnersOnly.Enabled = checking && chkConfirmTrades.Checked;
            txtPartners.Enabled = chkPartnersOnly.Enabled && chkPartnersOnly.Checked;
            radNotifyWindows.Enabled = radNotifyPopup.Enabled = checking || OtherAccountsChecking();
            btnSave.Enabled = true;
        }

        // Notification style is shared, so it stays editable while any other account checks confirmations
        private bool OtherAccountsChecking()
        {
            for (int i = 0; i < accounts.Length; i++)
            {
                if (i == selected) continue;
                var entry = manifest.GetEntry(accounts[i]);
                if (entry != null && entry.Confirmations != ConfirmationMode.Off) return true;
            }
            return false;
        }

        private void ShowWarning(CheckBox affectedBox)
        {
            if (loading) return;

            var result = MessageForm.Show("Warning: enabling this will severely reduce the security of your items! Use of this option is at your own risk. Would you like to continue?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                affectedBox.Checked = false;
            }
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            menuAccounts.Width = btnAccount.Width;
            menuAccounts.Show(btnAccount, new Point(0, btnAccount.Height + 4));
        }

        private void menuAccount_Click(object sender, EventArgs e)
        {
            SelectAccount(menuAccounts.Items.IndexOf((ToolStripItem)sender));
        }

        private void btnLanguage_Click(object sender, EventArgs e)
        {
            menuLanguages.Width = btnLanguage.Width;
            menuLanguages.Show(btnLanguage, new Point(0, btnLanguage.Height + 4));
        }

        private void menuLanguage_Click(object sender, EventArgs e)
        {
            SelectLanguage((string)((ToolStripMenuItem)sender).Tag);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StoreEntry();
            bool languageChanged = (manifest.Language ?? "") != language;
            manifest.StartWithWindows = chkStartWithWindows.Checked;
            manifest.StartMinimized = chkStartMinimized.Checked;
            manifest.CheckUpdates = chkCheckUpdates.Checked;
            manifest.NotificationStyle = radNotifyPopup.Checked ? NotificationStyle.Popup : NotificationStyle.Windows;
            manifest.Language = language;
            manifest.Save();
            Startup.Apply(manifest);
            if (languageChanged)
                MessageForm.Show("The new language is used the next time SDA starts.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radMode_CheckedChanged(object sender, EventArgs e)
        {
            SetControlsEnabledState();
        }

        private void chkConfirmMarket_CheckedChanged(object sender, EventArgs e)
        {
            if (chkConfirmMarket.Checked)
                ShowWarning(chkConfirmMarket);
        }

        private void chkConfirmTrades_CheckedChanged(object sender, EventArgs e)
        {
            if (chkConfirmTrades.Checked && !chkReceiveOnly.Checked && !chkPartnersOnly.Checked)
                ShowWarning(chkConfirmTrades);
            SetControlsEnabledState();
        }

        private void chkTradeRule_CheckedChanged(object sender, EventArgs e)
        {
            SetControlsEnabledState();
        }
    }
}
