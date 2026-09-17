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
            this.panelAccount = new System.Windows.Forms.Panel();
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
            this.chkReceiveOnly = new System.Windows.Forms.CheckBox();
            this.chkPartnersOnly = new System.Windows.Forms.CheckBox();
            this.panelPartners = new System.Windows.Forms.Panel();
            this.txtPartners = new System.Windows.Forms.TextBox();
            this.lblAutoHint = new System.Windows.Forms.Label();
            this.panelGlobal = new System.Windows.Forms.Panel();
            this.lblNotifyTitle = new System.Windows.Forms.Label();
            this.panelNotify = new System.Windows.Forms.Panel();
            this.radNotifyWindows = new System.Windows.Forms.RadioButton();
            this.radNotifyPopup = new System.Windows.Forms.RadioButton();
            this.lblStartupTitle = new System.Windows.Forms.Label();
            this.chkStartWithWindows = new System.Windows.Forms.CheckBox();
            this.chkStartMinimized = new System.Windows.Forms.CheckBox();
            this.lblLanguageTitle = new System.Windows.Forms.Label();
            this.btnLanguage = new System.Windows.Forms.Button();
            this.menuLanguages = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lblLanguageHint = new System.Windows.Forms.Label();
            this.lblUpdatesTitle = new System.Windows.Forms.Label();
            this.chkCheckUpdates = new System.Windows.Forms.CheckBox();
            this.lblUpdatesHint = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelAccount.SuspendLayout();
            this.panelInterval.SuspendLayout();
            this.panelPartners.SuspendLayout();
            this.panelGlobal.SuspendLayout();
            this.panelNotify.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriodicInterval)).BeginInit();
            this.SuspendLayout();
            //
            // panelAccount
            //
            this.panelAccount.BackColor = System.Drawing.SystemColors.Window;
            this.panelAccount.Controls.Add(this.lblAccountTitle);
            this.panelAccount.Controls.Add(this.btnAccount);
            this.panelAccount.Controls.Add(this.lblConfirmTitle);
            this.panelAccount.Controls.Add(this.radOff);
            this.panelAccount.Controls.Add(this.radPeriodic);
            this.panelAccount.Controls.Add(this.panelInterval);
            this.panelAccount.Controls.Add(this.lblSeconds);
            this.panelAccount.Controls.Add(this.radLive);
            this.panelAccount.Controls.Add(this.lblLiveHint);
            this.panelAccount.Controls.Add(this.lblAutoTitle);
            this.panelAccount.Controls.Add(this.chkConfirmTrades);
            this.panelAccount.Controls.Add(this.chkConfirmMarket);
            this.panelAccount.Controls.Add(this.chkReceiveOnly);
            this.panelAccount.Controls.Add(this.chkPartnersOnly);
            this.panelAccount.Controls.Add(this.panelPartners);
            this.panelAccount.Controls.Add(this.lblAutoHint);
            this.panelAccount.Location = new System.Drawing.Point(16, 16);
            this.panelAccount.Name = "panelAccount";
            this.panelAccount.Size = new System.Drawing.Size(376, 500);
            this.panelAccount.TabIndex = 0;
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
            this.btnAccount.Location = new System.Drawing.Point(16, 34);
            this.btnAccount.Name = "btnAccount";
            this.btnAccount.Size = new System.Drawing.Size(344, 36);
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
            this.lblConfirmTitle.Location = new System.Drawing.Point(16, 92);
            this.lblConfirmTitle.Name = "lblConfirmTitle";
            this.lblConfirmTitle.Size = new System.Drawing.Size(96, 13);
            this.lblConfirmTitle.TabIndex = 2;
            this.lblConfirmTitle.Text = "CONFIRMATIONS";
            //
            // radOff
            //
            this.radOff.AutoSize = true;
            this.radOff.Checked = true;
            this.radOff.Location = new System.Drawing.Point(16, 112);
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
            this.radPeriodic.Location = new System.Drawing.Point(16, 142);
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
            this.panelInterval.Location = new System.Drawing.Point(124, 136);
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
            this.lblSeconds.Location = new System.Drawing.Point(202, 143);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(56, 17);
            this.lblSeconds.TabIndex = 6;
            this.lblSeconds.Text = "seconds";
            //
            // radLive
            //
            this.radLive.AutoSize = true;
            this.radLive.Location = new System.Drawing.Point(16, 178);
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
            this.lblLiveHint.Location = new System.Drawing.Point(36, 200);
            this.lblLiveHint.Name = "lblLiveHint";
            this.lblLiveHint.Size = new System.Drawing.Size(324, 48);
            this.lblLiveHint.TabIndex = 8;
            this.lblLiveHint.Text = "Steam tells SDA the moment a confirmation appears, so nothing is polled. One conn" +
    "ection stays open for this account. If Steam refuses it, SDA checks every few se" +
    "conds instead.";
            //
            // lblAutoTitle
            //
            this.lblAutoTitle.AutoSize = true;
            this.lblAutoTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAutoTitle.Location = new System.Drawing.Point(16, 266);
            this.lblAutoTitle.Name = "lblAutoTitle";
            this.lblAutoTitle.Size = new System.Drawing.Size(80, 13);
            this.lblAutoTitle.TabIndex = 9;
            this.lblAutoTitle.Text = "AUTO ACCEPT";
            //
            // chkConfirmTrades
            //
            this.chkConfirmTrades.AutoSize = true;
            this.chkConfirmTrades.Location = new System.Drawing.Point(16, 286);
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
            this.chkConfirmMarket.Location = new System.Drawing.Point(120, 286);
            this.chkConfirmMarket.Name = "chkConfirmMarket";
            this.chkConfirmMarket.Size = new System.Drawing.Size(120, 21);
            this.chkConfirmMarket.TabIndex = 11;
            this.chkConfirmMarket.Text = "Market listings";
            this.chkConfirmMarket.UseVisualStyleBackColor = true;
            this.chkConfirmMarket.CheckedChanged += new System.EventHandler(this.chkConfirmMarket_CheckedChanged);
            //
            // chkReceiveOnly
            //
            this.chkReceiveOnly.AutoSize = true;
            this.chkReceiveOnly.Location = new System.Drawing.Point(36, 314);
            this.chkReceiveOnly.Name = "chkReceiveOnly";
            this.chkReceiveOnly.Size = new System.Drawing.Size(220, 21);
            this.chkReceiveOnly.TabIndex = 12;
            this.chkReceiveOnly.Text = "Only trades where I give nothing";
            this.chkReceiveOnly.UseVisualStyleBackColor = true;
            this.chkReceiveOnly.CheckedChanged += new System.EventHandler(this.chkTradeRule_CheckedChanged);
            //
            // chkPartnersOnly
            //
            this.chkPartnersOnly.AutoSize = true;
            this.chkPartnersOnly.Location = new System.Drawing.Point(36, 340);
            this.chkPartnersOnly.Name = "chkPartnersOnly";
            this.chkPartnersOnly.Size = new System.Drawing.Size(220, 21);
            this.chkPartnersOnly.TabIndex = 13;
            this.chkPartnersOnly.Text = "Only trades with these partners";
            this.chkPartnersOnly.UseVisualStyleBackColor = true;
            this.chkPartnersOnly.CheckedChanged += new System.EventHandler(this.chkTradeRule_CheckedChanged);
            //
            // panelPartners
            //
            this.panelPartners.BackColor = System.Drawing.SystemColors.Window;
            this.panelPartners.Controls.Add(this.txtPartners);
            this.panelPartners.Location = new System.Drawing.Point(36, 366);
            this.panelPartners.Name = "panelPartners";
            this.panelPartners.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.panelPartners.Size = new System.Drawing.Size(324, 58);
            this.panelPartners.TabIndex = 14;
            //
            // txtPartners
            //
            this.txtPartners.AcceptsReturn = true;
            this.txtPartners.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPartners.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPartners.Location = new System.Drawing.Point(12, 8);
            this.txtPartners.Multiline = true;
            this.txtPartners.Name = "txtPartners";
            this.txtPartners.PlaceholderText = "SteamID64 or profile link, one per line";
            this.txtPartners.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPartners.Size = new System.Drawing.Size(300, 42);
            this.txtPartners.TabIndex = 0;
            //
            // lblAutoHint
            //
            this.lblAutoHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAutoHint.Location = new System.Drawing.Point(16, 436);
            this.lblAutoHint.Name = "lblAutoHint";
            this.lblAutoHint.Size = new System.Drawing.Size(344, 48);
            this.lblAutoHint.TabIndex = 15;
            this.lblAutoHint.Text = "Accepted without showing them to you. Trades that fail a rule, or that SDA cannot" +
    " read, are left for you to review.";
            //
            // panelGlobal
            //
            this.panelGlobal.BackColor = System.Drawing.SystemColors.Window;
            this.panelGlobal.Controls.Add(this.lblNotifyTitle);
            this.panelGlobal.Controls.Add(this.panelNotify);
            this.panelGlobal.Controls.Add(this.lblStartupTitle);
            this.panelGlobal.Controls.Add(this.chkStartWithWindows);
            this.panelGlobal.Controls.Add(this.chkStartMinimized);
            this.panelGlobal.Controls.Add(this.lblLanguageTitle);
            this.panelGlobal.Controls.Add(this.btnLanguage);
            this.panelGlobal.Controls.Add(this.lblLanguageHint);
            this.panelGlobal.Controls.Add(this.lblUpdatesTitle);
            this.panelGlobal.Controls.Add(this.chkCheckUpdates);
            this.panelGlobal.Controls.Add(this.lblUpdatesHint);
            this.panelGlobal.Location = new System.Drawing.Point(408, 16);
            this.panelGlobal.Name = "panelGlobal";
            this.panelGlobal.Size = new System.Drawing.Size(344, 500);
            this.panelGlobal.TabIndex = 1;
            //
            // lblNotifyTitle
            //
            this.lblNotifyTitle.AutoSize = true;
            this.lblNotifyTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotifyTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblNotifyTitle.Location = new System.Drawing.Point(16, 14);
            this.lblNotifyTitle.Name = "lblNotifyTitle";
            this.lblNotifyTitle.Size = new System.Drawing.Size(90, 13);
            this.lblNotifyTitle.TabIndex = 0;
            this.lblNotifyTitle.Text = "NOTIFICATIONS";
            //
            // panelNotify
            //
            this.panelNotify.Controls.Add(this.radNotifyWindows);
            this.panelNotify.Controls.Add(this.radNotifyPopup);
            this.panelNotify.Location = new System.Drawing.Point(16, 34);
            this.panelNotify.Name = "panelNotify";
            this.panelNotify.Size = new System.Drawing.Size(312, 52);
            this.panelNotify.TabIndex = 1;
            //
            // radNotifyWindows
            //
            this.radNotifyWindows.AutoSize = true;
            this.radNotifyWindows.Checked = true;
            this.radNotifyWindows.Location = new System.Drawing.Point(0, 0);
            this.radNotifyWindows.Name = "radNotifyWindows";
            this.radNotifyWindows.Size = new System.Drawing.Size(160, 21);
            this.radNotifyWindows.TabIndex = 0;
            this.radNotifyWindows.TabStop = true;
            this.radNotifyWindows.Text = "Windows notification";
            this.radNotifyWindows.UseVisualStyleBackColor = true;
            //
            // radNotifyPopup
            //
            this.radNotifyPopup.AutoSize = true;
            this.radNotifyPopup.Location = new System.Drawing.Point(0, 26);
            this.radNotifyPopup.Name = "radNotifyPopup";
            this.radNotifyPopup.Size = new System.Drawing.Size(240, 21);
            this.radNotifyPopup.TabIndex = 1;
            this.radNotifyPopup.Text = "Popup with quick accept and deny";
            this.radNotifyPopup.UseVisualStyleBackColor = true;
            //
            // lblStartupTitle
            //
            this.lblStartupTitle.AutoSize = true;
            this.lblStartupTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStartupTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStartupTitle.Location = new System.Drawing.Point(16, 106);
            this.lblStartupTitle.Name = "lblStartupTitle";
            this.lblStartupTitle.Size = new System.Drawing.Size(56, 13);
            this.lblStartupTitle.TabIndex = 2;
            this.lblStartupTitle.Text = "STARTUP";
            //
            // chkStartWithWindows
            //
            this.chkStartWithWindows.AutoSize = true;
            this.chkStartWithWindows.Location = new System.Drawing.Point(16, 126);
            this.chkStartWithWindows.Name = "chkStartWithWindows";
            this.chkStartWithWindows.Size = new System.Drawing.Size(140, 21);
            this.chkStartWithWindows.TabIndex = 3;
            this.chkStartWithWindows.Text = "Start with Windows";
            this.chkStartWithWindows.UseVisualStyleBackColor = true;
            //
            // chkStartMinimized
            //
            this.chkStartMinimized.AutoSize = true;
            this.chkStartMinimized.Location = new System.Drawing.Point(16, 152);
            this.chkStartMinimized.Name = "chkStartMinimized";
            this.chkStartMinimized.Size = new System.Drawing.Size(150, 21);
            this.chkStartMinimized.TabIndex = 4;
            this.chkStartMinimized.Text = "Start minimized to tray";
            this.chkStartMinimized.UseVisualStyleBackColor = true;
            //
            // lblLanguageTitle
            //
            this.lblLanguageTitle.AutoSize = true;
            this.lblLanguageTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLanguageTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblLanguageTitle.Location = new System.Drawing.Point(16, 192);
            this.lblLanguageTitle.Name = "lblLanguageTitle";
            this.lblLanguageTitle.Size = new System.Drawing.Size(66, 13);
            this.lblLanguageTitle.TabIndex = 5;
            this.lblLanguageTitle.Text = "LANGUAGE";
            //
            // btnLanguage
            //
            this.btnLanguage.BackColor = System.Drawing.SystemColors.Window;
            this.btnLanguage.Location = new System.Drawing.Point(16, 212);
            this.btnLanguage.Name = "btnLanguage";
            this.btnLanguage.Size = new System.Drawing.Size(312, 36);
            this.btnLanguage.TabIndex = 6;
            this.btnLanguage.Text = "English";
            this.btnLanguage.UseVisualStyleBackColor = false;
            this.btnLanguage.Click += new System.EventHandler(this.btnLanguage_Click);
            //
            // menuLanguages
            //
            this.menuLanguages.Name = "menuLanguages";
            this.menuLanguages.Size = new System.Drawing.Size(181, 26);
            //
            // lblLanguageHint
            //
            this.lblLanguageHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLanguageHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblLanguageHint.Location = new System.Drawing.Point(16, 254);
            this.lblLanguageHint.Name = "lblLanguageHint";
            this.lblLanguageHint.Size = new System.Drawing.Size(312, 32);
            this.lblLanguageHint.TabIndex = 7;
            this.lblLanguageHint.Text = "Translations are json files in the languages folder next to SDA. Copy template.js" +
    "on to add one.";
            //
            // lblUpdatesTitle
            //
            this.lblUpdatesTitle.AutoSize = true;
            this.lblUpdatesTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatesTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblUpdatesTitle.Location = new System.Drawing.Point(16, 304);
            this.lblUpdatesTitle.Name = "lblUpdatesTitle";
            this.lblUpdatesTitle.Size = new System.Drawing.Size(56, 13);
            this.lblUpdatesTitle.TabIndex = 8;
            this.lblUpdatesTitle.Text = "UPDATES";
            //
            // chkCheckUpdates
            //
            this.chkCheckUpdates.AutoSize = true;
            this.chkCheckUpdates.Checked = true;
            this.chkCheckUpdates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCheckUpdates.Location = new System.Drawing.Point(16, 324);
            this.chkCheckUpdates.Name = "chkCheckUpdates";
            this.chkCheckUpdates.Size = new System.Drawing.Size(220, 21);
            this.chkCheckUpdates.TabIndex = 9;
            this.chkCheckUpdates.Text = "Check for updates when SDA starts";
            this.chkCheckUpdates.UseVisualStyleBackColor = true;
            //
            // lblUpdatesHint
            //
            this.lblUpdatesHint.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUpdatesHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblUpdatesHint.Location = new System.Drawing.Point(36, 348);
            this.lblUpdatesHint.Name = "lblUpdatesHint";
            this.lblUpdatesHint.Size = new System.Drawing.Size(292, 32);
            this.lblUpdatesHint.TabIndex = 10;
            this.lblUpdatesHint.Text = "Updates come from the SDA 2 GitHub releases and install in one click. Your accounts" +
    " are never touched.";
            //
            // btnSave
            //
            this.btnSave.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSave.Location = new System.Drawing.Point(632, 532);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 36);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(504, 532);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(120, 36);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // SettingsForm
            //
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(768, 584);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.panelGlobal);
            this.Controls.Add(this.panelAccount);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.panelAccount.ResumeLayout(false);
            this.panelAccount.PerformLayout();
            this.panelInterval.ResumeLayout(false);
            this.panelPartners.ResumeLayout(false);
            this.panelPartners.PerformLayout();
            this.panelGlobal.ResumeLayout(false);
            this.panelGlobal.PerformLayout();
            this.panelNotify.ResumeLayout(false);
            this.panelNotify.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriodicInterval)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelAccount;
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
        private System.Windows.Forms.CheckBox chkReceiveOnly;
        private System.Windows.Forms.CheckBox chkPartnersOnly;
        private System.Windows.Forms.Panel panelPartners;
        private System.Windows.Forms.TextBox txtPartners;
        private System.Windows.Forms.Label lblAutoHint;
        private System.Windows.Forms.Panel panelGlobal;
        private System.Windows.Forms.Label lblNotifyTitle;
        private System.Windows.Forms.Panel panelNotify;
        private System.Windows.Forms.RadioButton radNotifyWindows;
        private System.Windows.Forms.RadioButton radNotifyPopup;
        private System.Windows.Forms.Label lblStartupTitle;
        private System.Windows.Forms.CheckBox chkStartWithWindows;
        private System.Windows.Forms.CheckBox chkStartMinimized;
        private System.Windows.Forms.Label lblLanguageTitle;
        private System.Windows.Forms.Button btnLanguage;
        private System.Windows.Forms.ContextMenuStrip menuLanguages;
        private System.Windows.Forms.Label lblLanguageHint;
        private System.Windows.Forms.Label lblUpdatesTitle;
        private System.Windows.Forms.CheckBox chkCheckUpdates;
        private System.Windows.Forms.Label lblUpdatesHint;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
