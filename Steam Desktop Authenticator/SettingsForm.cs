using System;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public partial class SettingsForm : Form
    {
        Manifest manifest;
        bool fullyLoaded = false;

        public SettingsForm()
        {
            InitializeComponent();
            Theme.Apply(this);

            // Get latest manifest
            manifest = Manifest.GetManifest(true);

            chkLive.Checked = manifest.LiveNotifications;
            chkPeriodicChecking.Checked = manifest.PeriodicChecking;
            numPeriodicInterval.Value = manifest.PeriodicCheckingInterval;
            chkCheckAll.Checked = manifest.CheckAllAccounts;
            chkConfirmMarket.Checked = manifest.AutoConfirmMarketTransactions;
            chkConfirmTrades.Checked = manifest.AutoConfirmTrades;

            SetControlsEnabledState();

            fullyLoaded = true;
        }

        private void SetControlsEnabledState()
        {
            numPeriodicInterval.Enabled = chkPeriodicChecking.Checked;
            chkCheckAll.Enabled = chkConfirmMarket.Enabled = chkConfirmTrades.Enabled = chkPeriodicChecking.Checked || chkLive.Checked;
        }

        private void ShowWarning(CheckBox affectedBox)
        {
            if (!fullyLoaded) return;

            var result = MessageBox.Show("Warning: enabling this will severely reduce the security of your items! Use of this option is at your own risk. Would you like to continue?", "Warning!", MessageBoxButtons.YesNo);
            if (result == DialogResult.No)
            {
                affectedBox.Checked = false;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            manifest.LiveNotifications = chkLive.Checked;
            manifest.PeriodicChecking = chkPeriodicChecking.Checked;
            manifest.PeriodicCheckingInterval = (int)numPeriodicInterval.Value;
            manifest.CheckAllAccounts = chkCheckAll.Checked;
            manifest.AutoConfirmMarketTransactions = chkConfirmMarket.Checked;
            manifest.AutoConfirmTrades = chkConfirmTrades.Checked;
            manifest.Save();
            this.Close();
        }

        private void chkPeriodicChecking_CheckedChanged(object sender, EventArgs e)
        {
            SetControlsEnabledState();
        }

        private void chkLive_CheckedChanged(object sender, EventArgs e)
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
