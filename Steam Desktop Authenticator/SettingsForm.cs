using System;
using System.Drawing;
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

        public SettingsForm(SteamGuardAccount[] accounts)
        {
            InitializeComponent();
            Theme.Apply(this);
            Theme.Apply(menuAccounts);
            Theme.Dropdown(btnAccount);

            manifest = Manifest.GetManifest(true);
            this.accounts = accounts ?? new SteamGuardAccount[0];

            chkStartWithWindows.Checked = manifest.StartWithWindows;
            chkStartMinimized.Checked = manifest.StartMinimized;

            foreach (var account in this.accounts)
            {
                var item = new ToolStripMenuItem(account.AccountName);
                item.Click += menuAccount_Click;
                menuAccounts.Items.Add(item);
            }
            Theme.StyleMenuItems(menuAccounts.Items, false);

            SelectAccount(this.accounts.Length > 0 ? 0 : -1);
            this.ActiveControl = btnSave;
        }

        private void SelectAccount(int index)
        {
            StoreEntry();
            selected = index;

            for (int i = 0; i < menuAccounts.Items.Count; i++)
                ((ToolStripMenuItem)menuAccounts.Items[i]).Checked = i == index;

            btnAccount.Text = index >= 0 ? accounts[index].AccountName : "No accounts";
            btnAccount.Enabled = accounts.Length > 0;
            LoadEntry(index >= 0 ? manifest.GetEntry(accounts[index]) : null);
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
        }

        private void SetControlsEnabledState()
        {
            bool hasAccount = current != null;
            radOff.Enabled = radPeriodic.Enabled = radLive.Enabled = hasAccount;
            numPeriodicInterval.Enabled = hasAccount && radPeriodic.Checked;
            chkConfirmTrades.Enabled = chkConfirmMarket.Enabled = hasAccount && !radOff.Checked;
            btnSave.Enabled = true;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            StoreEntry();
            manifest.StartWithWindows = chkStartWithWindows.Checked;
            manifest.StartMinimized = chkStartMinimized.Checked;
            manifest.Save();
            Startup.Apply(manifest);
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
            if (chkConfirmTrades.Checked)
                ShowWarning(chkConfirmTrades);
        }
    }
}
