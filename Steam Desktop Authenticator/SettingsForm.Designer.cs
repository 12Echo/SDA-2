namespace Steam_Desktop_Authenticator
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.lblAccountTitle = new System.Windows.Forms.Label();
            this.btnAccount = new System.Windows.Forms.Button();
            this.menuAccounts = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblConfirmTitle = new System.Windows.Forms.Label();
            this.radOff = new System.Windows.Forms.RadioButton();
            this.radPeriodic = new System.Windows.Forms.RadioButton();
            this.panelInterval = new System.Windows.Forms.Panel();
            this.numPeriodicInterval = new System.Windows.Forms.NumericUpDown();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.radLive = new System.Windows.Forms.RadioButton();
            this.lblLiveHint = new System.Windows.Forms.Label();
            this.lblAutoTitle = new System.Windows.Forms.Label();
            this.chkConfirmTrades = new System.Windows.Forms.CheckBox();
            this.chkConfirmMarket = new System.Windows.Forms.CheckBox();
            this.lblAutoHint = new System.Windows.Forms.Label();
            this.lblStartupTitle = new System.Windows.Forms.Label();
            this.chkStartWithWindows = new System.Windows.Forms.CheckBox();
            this.chkStartMinimized = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelInterval.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriodicInterval)).BeginInit();
            this.SuspendLayout();
            //
            // lblAccountTitle
            //
            this.lblAccountTitle.AutoSize = true;
            this.lblAccountTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccountTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAccountTitle.Location = new System.Drawing.Point(16, 14);
            this.lblAccountTitle.Name = "lblAccountTitle";
            this.lblAccountTitle.Size = new System.Drawing.Size(58, 13);
            this.lblAccountTitle.TabIndex = 0;
            this.lblAccountTitle.Text = "ACCOUNT";
            //
            // btnAccount
            //
            this.btnAccount.BackColor = System.Drawing.SystemColors.Window;
            this.btnAccount.Location = new System.Drawing.Point(16, 32);
            this.btnAccount.Name = "btnAccount";
            this.btnAccount.Size = new System.Drawing.Size(368, 38);
            this.btnAccount.TabIndex = 1;
            this.btnAccount.Text = "account";
            this.btnAccount.UseVisualStyleBackColor = false;
            this.btnAccount.Click += new System.EventHandler(this.btnAccount_Click);
            //
            // menuAccounts
            //
            this.menuAccounts.Name = "menuAccounts";
            this.menuAccounts.Size = new System.Drawing.Size(181, 26);
            //
            // lblConfirmTitle
            //
            this.lblConfirmTitle.AutoSize = true;
            this.lblConfirmTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirmTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblConfirmTitle.Location = new System.Drawing.Point(16, 90);
            this.lblConfirmTitle.Name = "lblConfirmTitle";
            this.lblConfirmTitle.Size = new System.Drawing.Size(96, 13);
            this.lblConfirmTitle.TabIndex = 2;
            this.lblConfirmTitle.Text = "CONFIRMATIONS";
            //
            // radOff
            //
            this.radOff.AutoSize = true;
            this.radOff.Checked = true;
            this.radOff.Location = new System.Drawing.Point(16, 110);
            this.radOff.Name = "radOff";
            this.radOff.Size = new System.Drawing.Size(200, 21);
            this.radOff.TabIndex = 3;
            this.radOff.TabStop = true;
            this.radOff.Text = "Do not check for confirmations";
            this.radOff.UseVisualStyleBackColor = true;
            this.radOff.CheckedChanged += new System.EventHandler(this.radMode_CheckedChanged);
            //
            // radPeriodic
            //
            this.radPeriodic.AutoSize = true;
            this.radPeriodic.Location = new System.Drawing.Point(16, 140);
            this.radPeriodic.Name = "radPeriodic";
            this.radPeriodic.Size = new System.Drawing.Size(100, 21);
            this.radPeriodic.TabIndex = 4;
            this.radPeriodic.Text = "Check every";
            this.radPeriodic.UseVisualStyleBackColor = true;
            this.radPeriodic.CheckedChanged += new System.EventHandler(this.radMode_CheckedChanged);
            //
            // panelInterval
            //
            this.panelInterval.BackColor = System.Drawing.SystemColors.Window;
            this.panelInterval.Controls.Add(this.numPeriodicInterval);
            this.panelInterval.Location = new System.Drawing.Point(124, 134);
            this.panelInterval.Name = "panelInterval";
            this.panelInterval.Padding = new System.Windows.Forms.Padding(8, 6, 4, 5);
            this.panelInterval.Size = new System.Drawing.Size(70, 32);
            this.panelInterval.TabIndex = 5;
            //
            // numPeriodicInterval
            //
            this.numPeriodicInterval.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numPeriodicInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPeriodicInterval.Location = new System.Drawing.Point(8, 6);
            this.numPeriodicInterval.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
            this.numPeriodicInterval.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numPeriodicInterval.Name = "numPeriodicInterval";
            this.numPeriodicInterval.Size = new System.Drawing.Size(58, 20);
            this.numPeriodicInterval.TabIndex = 0;
            this.numPeriodicInterval.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            //
            // lblSeconds
            //
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(202, 141);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(56, 17);
            this.lblSeconds.TabIndex = 6;
            this.lblSeconds.Text = "seconds";
            //
            // radLive
            //
            this.radLive.AutoSize = true;
            this.radLive.Location = new System.Drawing.Point(16, 176);
            this.radLive.Name = "radLive";
            this.radLive.Size = new System.Drawing.Size(180, 21);
            this.radLive.TabIndex = 7;
            this.radLive.Text = "Stay connected to Steam";
            this.radLive.UseVisualStyleBackColor = true;
            this.radLive.CheckedChanged += new System.EventHandler(this.radMode_CheckedChanged);
            //
            // lblLiveHint
            //
            this.lblLiveHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLiveHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblLiveHint.Location = new System.Drawing.Point(36, 198);
            this.lblLiveHint.Name = "lblLiveHint";
            this.lblLiveHint.Size = new System.Drawing.Size(348, 64);
            this.lblLiveHint.TabIndex = 8;
            this.lblLiveHint.Text = "Steam tells SDA the moment a confirmation appears, so there is no delay and nothi" +
    "ng to poll. Keeps one connection open for this account. If Steam refuses the con" +
    "nection, SDA checks every few seconds instead. You may be asked to log in again once " +
    "to set this up.";
            //
            // lblAutoTitle
            //
            this.lblAutoTitle.AutoSize = true;
            this.lblAutoTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAutoTitle.Location = new System.Drawing.Point(16, 278);
            this.lblAutoTitle.Name = "lblAutoTitle";
            this.lblAutoTitle.Size = new System.Drawing.Size(80, 13);
            this.lblAutoTitle.TabIndex = 9;
            this.lblAutoTitle.Text = "AUTO ACCEPT";
            //
            // chkConfirmTrades
            //
            this.chkConfirmTrades.AutoSize = true;
            this.chkConfirmTrades.Location = new System.Drawing.Point(16, 298);
            this.chkConfirmTrades.Name = "chkConfirmTrades";
            this.chkConfirmTrades.Size = new System.Drawing.Size(70, 21);
            this.chkConfirmTrades.TabIndex = 10;
            this.chkConfirmTrades.Text = "Trades";
            this.chkConfirmTrades.UseVisualStyleBackColor = true;
            this.chkConfirmTrades.CheckedChanged += new System.EventHandler(this.chkConfirmTrades_CheckedChanged);
            //
            // chkConfirmMarket
            //
            this.chkConfirmMarket.AutoSize = true;
            this.chkConfirmMarket.Location = new System.Drawing.Point(120, 298);
            this.chkConfirmMarket.Name = "chkConfirmMarket";
            this.chkConfirmMarket.Size = new System.Drawing.Size(120, 21);
            this.chkConfirmMarket.TabIndex = 11;
            this.chkConfirmMarket.Text = "Market listings";
            this.chkConfirmMarket.UseVisualStyleBackColor = true;
            this.chkConfirmMarket.CheckedChanged += new System.EventHandler(this.chkConfirmMarket_CheckedChanged);
            //
            // lblAutoHint
            //
            this.lblAutoHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAutoHint.Location = new System.Drawing.Point(16, 324);
            this.lblAutoHint.Name = "lblAutoHint";
            this.lblAutoHint.Size = new System.Drawing.Size(368, 32);
            this.lblAutoHint.TabIndex = 12;
            this.lblAutoHint.Text = "Accepts these without showing them to you. Only turn this on for bot accounts.";
            //
            // lblStartupTitle
            //
            this.lblStartupTitle.AutoSize = true;
            this.lblStartupTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStartupTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStartupTitle.Location = new System.Drawing.Point(16, 366);
            this.lblStartupTitle.Name = "lblStartupTitle";
            this.lblStartupTitle.Size = new System.Drawing.Size(56, 13);
            this.lblStartupTitle.TabIndex = 13;
            this.lblStartupTitle.Text = "STARTUP";
            //
            // chkStartWithWindows
            //
            this.chkStartWithWindows.AutoSize = true;
            this.chkStartWithWindows.Location = new System.Drawing.Point(16, 386);
            this.chkStartWithWindows.Name = "chkStartWithWindows";
            this.chkStartWithWindows.Size = new System.Drawing.Size(140, 21);
            this.chkStartWithWindows.TabIndex = 14;
            this.chkStartWithWindows.Text = "Start with Windows";
            this.chkStartWithWindows.UseVisualStyleBackColor = true;
            //
            // chkStartMinimized
            //
            this.chkStartMinimized.AutoSize = true;
            this.chkStartMinimized.Location = new System.Drawing.Point(190, 386);
            this.chkStartMinimized.Name = "chkStartMinimized";
            this.chkStartMinimized.Size = new System.Drawing.Size(150, 21);
            this.chkStartMinimized.TabIndex = 15;
            this.chkStartMinimized.Text = "Start minimized to tray";
            this.chkStartMinimized.UseVisualStyleBackColor = true;
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSave.Location = new System.Drawing.Point(16, 424);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(368, 38);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // SettingsForm
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 478);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkStartMinimized);
            this.Controls.Add(this.chkStartWithWindows);
            this.Controls.Add(this.lblStartupTitle);
            this.Controls.Add(this.lblAutoHint);
            this.Controls.Add(this.chkConfirmMarket);
            this.Controls.Add(this.chkConfirmTrades);
            this.Controls.Add(this.lblAutoTitle);
            this.Controls.Add(this.lblLiveHint);
            this.Controls.Add(this.radLive);
            this.Controls.Add(this.lblSeconds);
            this.Controls.Add(this.panelInterval);
            this.Controls.Add(this.radPeriodic);
            this.Controls.Add(this.radOff);
            this.Controls.Add(this.lblConfirmTitle);
            this.Controls.Add(this.btnAccount);
            this.Controls.Add(this.lblAccountTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.panelInterval.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numPeriodicInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAccountTitle;
        private System.Windows.Forms.Button btnAccount;
        private System.Windows.Forms.ContextMenuStrip menuAccounts;
        private System.Windows.Forms.Label lblConfirmTitle;
        private System.Windows.Forms.RadioButton radOff;
        private System.Windows.Forms.RadioButton radPeriodic;
        private System.Windows.Forms.Panel panelInterval;
        private System.Windows.Forms.NumericUpDown numPeriodicInterval;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.RadioButton radLive;
        private System.Windows.Forms.Label lblLiveHint;
        private System.Windows.Forms.Label lblAutoTitle;
        private System.Windows.Forms.CheckBox chkConfirmTrades;
        private System.Windows.Forms.CheckBox chkConfirmMarket;
        private System.Windows.Forms.Label lblAutoHint;
        private System.Windows.Forms.Label lblStartupTitle;
        private System.Windows.Forms.CheckBox chkStartWithWindows;
        private System.Windows.Forms.CheckBox chkStartMinimized;
        private System.Windows.Forms.Button btnSave;
    }
}
