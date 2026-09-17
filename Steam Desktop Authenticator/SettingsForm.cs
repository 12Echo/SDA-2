using System;
using System.Windows.Forms;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    public partial class SettingsForm : Form
    {
        Manifest manifest;
        SteamGuardAccount[] accounts;
        Manifest.ManifestEntry current;
        bool loading = false;

        public SettingsForm(SteamGuardAccount[] accounts)
        {
            InitializeComponent();
            Theme.Apply(this);

            manifest = Manifest.GetManifest(true);
            this.accounts = accounts ?? new SteamGuardAccount[0];

            foreach (var account in this.accounts)
                cmbAccount.Items.Add(account.AccountName);

            if (cmbAccount.Items.Count > 0)
                cmbAccount.SelectedIndex = 0;
            else
                LoadEntry(null);

            this.ActiveControl = btnSave;
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
            btnSave.Enabled = hasAccount;
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

        private void cmbAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            StoreEntry();
            int index = cmbAccount.SelectedIndex;
            LoadEntry(index >= 0 ? manifest.GetEntry(accounts[index]) : null);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StoreEntry();
            manifest.Save();
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
