namespace Steam_Desktop_Authenticator
{

    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnSteamLogin = new System.Windows.Forms.Button();
            this.panelCode = new System.Windows.Forms.Panel();
            this.lblCodeTitle = new System.Windows.Forms.Label();
            this.btnCopy = new System.Windows.Forms.Button();
            this.pbTimeout = new Steam_Desktop_Authenticator.TimeoutBar();
            this.txtLoginToken = new System.Windows.Forms.TextBox();
            this.listAccounts = new System.Windows.Forms.ListBox();
            this.timerSteamGuard = new System.Windows.Forms.Timer(this.components);
            this.btnTradeConfirmations = new System.Windows.Forms.Button();
            this.btnManageEncryption = new System.Windows.Forms.Button();
            this.groupAccount = new System.Windows.Forms.Panel();
            this.picAvatar = new System.Windows.Forms.PictureBox();
            this.lblAccountTitle = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblSession = new System.Windows.Forms.LinkLabel();
            this.lblWarning = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelUpdate = new System.Windows.Forms.LinkLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImportAccount = new System.Windows.Forms.ToolStripMenuItem();
            this.menuBackup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLock = new System.Windows.Forms.ToolStripMenuItem();
            this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.accountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuLoginAgain = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRename = new System.Windows.Forms.ToolStripMenuItem();
            this.menuGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuApproveQr = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRecoveryKit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuRemoveAccountFromManifest = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeactivateAuthenticator = new System.Windows.Forms.ToolStripMenuItem();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menuStripTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayRestore = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.trayAccounts = new System.Windows.Forms.ToolStripMenuItem();
            this.trayTradeConfirmations = new System.Windows.Forms.ToolStripMenuItem();
            this.trayCopySteamGuard = new System.Windows.Forms.ToolStripMenuItem();
            this.trayCheckNow = new System.Windows.Forms.ToolStripMenuItem();
            this.trayApproveQr = new System.Windows.Forms.ToolStripMenuItem();
            this.trayLock = new System.Windows.Forms.ToolStripMenuItem();
            this.listMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.listRename = new System.Windows.Forms.ToolStripMenuItem();
            this.listGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.listLoginAgain = new System.Windows.Forms.ToolStripMenuItem();
            this.listConfirmations = new System.Windows.Forms.ToolStripMenuItem();
            this.listApproveQr = new System.Windows.Forms.ToolStripMenuItem();
            this.listRecoveryKit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.listRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.trayQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.timerTradesPopup = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.txtAccSearch = new System.Windows.Forms.TextBox();
            this.panelButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnGroup = new System.Windows.Forms.Button();
            this.menuGroups = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.panelCode.SuspendLayout();
            this.groupAccount.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.menuStripTray.SuspendLayout();
            this.listMenu.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // btnSteamLogin
            //
            this.btnSteamLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSteamLogin.Location = new System.Drawing.Point(0, 0);
            this.btnSteamLogin.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.btnSteamLogin.Name = "btnSteamLogin";
            this.btnSteamLogin.Size = new System.Drawing.Size(160, 34);
            this.btnSteamLogin.TabIndex = 1;
            this.btnSteamLogin.Text = "Setup New Account";
            this.btnSteamLogin.UseVisualStyleBackColor = true;
            this.btnSteamLogin.Click += new System.EventHandler(this.btnSteamLogin_Click);
            //
            // panelCode
            //
            this.panelCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCode.BackColor = System.Drawing.SystemColors.Window;
            this.panelCode.Controls.Add(this.lblCodeTitle);
            this.panelCode.Controls.Add(this.btnCopy);
            this.panelCode.Controls.Add(this.txtLoginToken);
            this.panelCode.Controls.Add(this.pbTimeout);
            this.panelCode.Location = new System.Drawing.Point(16, 84);
            this.panelCode.Name = "panelCode";
            this.panelCode.Size = new System.Drawing.Size(328, 118);
            this.panelCode.TabIndex = 2;
            //
            // lblCodeTitle
            //
            this.lblCodeTitle.AutoSize = true;
            this.lblCodeTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCodeTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCodeTitle.Location = new System.Drawing.Point(16, 12);
            this.lblCodeTitle.Name = "lblCodeTitle";
            this.lblCodeTitle.Size = new System.Drawing.Size(70, 13);
            this.lblCodeTitle.TabIndex = 3;
            this.lblCodeTitle.Text = "LOGIN CODE";
            //
            // btnCopy
            //
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(244, 40);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(68, 34);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            //
            // pbTimeout
            //
            this.pbTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbTimeout.Location = new System.Drawing.Point(16, 100);
            this.pbTimeout.Maximum = 30;
            this.pbTimeout.Name = "pbTimeout";
            this.pbTimeout.Size = new System.Drawing.Size(296, 4);
            this.pbTimeout.TabIndex = 1;
            this.pbTimeout.Value = 30;
            //
            // txtLoginToken
            //
            this.txtLoginToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLoginToken.BackColor = System.Drawing.SystemColors.Window;
            this.txtLoginToken.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLoginToken.Font = new System.Drawing.Font("Segoe UI", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLoginToken.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtLoginToken.Location = new System.Drawing.Point(12, 32);
            this.txtLoginToken.Name = "txtLoginToken";
            this.txtLoginToken.ReadOnly = true;
            this.txtLoginToken.Size = new System.Drawing.Size(222, 49);
            this.txtLoginToken.TabIndex = 0;
            this.txtLoginToken.TabStop = false;
            this.txtLoginToken.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // listAccounts
            //
            this.listAccounts.ContextMenuStrip = this.listMenu;
            this.listAccounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listAccounts.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listAccounts.FormattingEnabled = true;
            this.listAccounts.IntegralHeight = false;
            this.listAccounts.Location = new System.Drawing.Point(16, 348);
            this.listAccounts.Name = "listAccounts";
            this.listAccounts.Size = new System.Drawing.Size(328, 140);
            this.listAccounts.TabIndex = 3;
            this.listAccounts.SelectedValueChanged += new System.EventHandler(this.listAccounts_SelectedValueChanged);
            this.listAccounts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listAccounts_KeyDown);
            this.listAccounts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listAccounts_MouseDown);
            this.listAccounts.MouseMove += new System.Windows.Forms.MouseEventHandler(this.listAccounts_MouseMove);
            this.listAccounts.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listAccounts_MouseUp);
            this.listAccounts.MouseCaptureChanged += new System.EventHandler(this.listAccounts_MouseCaptureChanged);
            //
            // timerSteamGuard
            //
            this.timerSteamGuard.Enabled = true;
            this.timerSteamGuard.Interval = 1000;
            this.timerSteamGuard.Tick += new System.EventHandler(this.timerSteamGuard_Tick);
            //
            // btnTradeConfirmations
            //
            this.btnTradeConfirmations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTradeConfirmations.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnTradeConfirmations.Enabled = false;
            this.btnTradeConfirmations.Location = new System.Drawing.Point(166, 20);
            this.btnTradeConfirmations.Name = "btnTradeConfirmations";
            this.btnTradeConfirmations.Size = new System.Drawing.Size(146, 34);
            this.btnTradeConfirmations.TabIndex = 4;
            this.btnTradeConfirmations.Text = "View Confirmations";
            this.btnTradeConfirmations.UseVisualStyleBackColor = false;
            this.btnTradeConfirmations.Click += new System.EventHandler(this.btnTradeConfirmations_Click);
            //
            // btnManageEncryption
            //
            this.btnManageEncryption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnManageEncryption.Location = new System.Drawing.Point(168, 0);
            this.btnManageEncryption.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.btnManageEncryption.Name = "btnManageEncryption";
            this.btnManageEncryption.Size = new System.Drawing.Size(160, 34);
            this.btnManageEncryption.TabIndex = 6;
            this.btnManageEncryption.Text = "Manage Encryption";
            this.btnManageEncryption.UseVisualStyleBackColor = true;
            this.btnManageEncryption.Click += new System.EventHandler(this.btnManageEncryption_Click);
            //
            // groupAccount
            //
            this.groupAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupAccount.BackColor = System.Drawing.SystemColors.Window;
            this.groupAccount.Controls.Add(this.picAvatar);
            this.groupAccount.Controls.Add(this.lblAccountTitle);
            this.groupAccount.Controls.Add(this.lblAccount);
            this.groupAccount.Controls.Add(this.lblSession);
            this.groupAccount.Controls.Add(this.lblWarning);
            this.groupAccount.Controls.Add(this.btnTradeConfirmations);
            this.groupAccount.Location = new System.Drawing.Point(16, 212);
            this.groupAccount.Name = "groupAccount";
            this.groupAccount.Size = new System.Drawing.Size(328, 74);
            this.groupAccount.TabIndex = 7;
            //
            // picAvatar
            //
            this.picAvatar.BackColor = System.Drawing.SystemColors.Control;
            this.picAvatar.Location = new System.Drawing.Point(16, 15);
            this.picAvatar.Name = "picAvatar";
            this.picAvatar.Size = new System.Drawing.Size(44, 44);
            this.picAvatar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picAvatar.TabIndex = 8;
            this.picAvatar.TabStop = false;
            //
            // lblAccountTitle
            //
            this.lblAccountTitle.AutoEllipsis = true;
            this.lblAccountTitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccountTitle.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAccountTitle.Location = new System.Drawing.Point(72, 10);
            this.lblAccountTitle.Name = "lblAccountTitle";
            this.lblAccountTitle.Size = new System.Drawing.Size(88, 15);
            this.lblAccountTitle.TabIndex = 5;
            this.lblAccountTitle.Text = "ACCOUNT";
            //
            // lblAccount
            //
            this.lblAccount.AutoEllipsis = true;
            this.lblAccount.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccount.Location = new System.Drawing.Point(71, 25);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(90, 22);
            this.lblAccount.TabIndex = 6;
            this.lblAccount.Text = "No account";
            this.lblAccount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSession
            //
            this.lblSession.AutoEllipsis = true;
            this.lblSession.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSession.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
            this.lblSession.Location = new System.Drawing.Point(72, 48);
            this.lblSession.Name = "lblSession";
            this.lblSession.Size = new System.Drawing.Size(88, 16);
            this.lblSession.TabIndex = 9;
            this.lblSession.Text = "Session active";
            this.lblSession.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSession_LinkClicked);
            //
            // lblWarning
            //
            this.lblWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWarning.AutoEllipsis = true;
            this.lblWarning.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWarning.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblWarning.Location = new System.Drawing.Point(16, 70);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblWarning.Size = new System.Drawing.Size(296, 22);
            this.lblWarning.TabIndex = 10;
            this.lblWarning.Text = "Trade holds end in 6 days";
            this.lblWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblWarning.Visible = false;
            this.lblWarning.Paint += new System.Windows.Forms.PaintEventHandler(this.lblWarning_Paint);
            //
            // labelVersion
            //
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVersion.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelVersion.Location = new System.Drawing.Point(244, 500);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(100, 18);
            this.labelVersion.TabIndex = 8;
            this.labelVersion.Text = "v0.0.0";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // labelUpdate
            //
            this.labelUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelUpdate.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUpdate.Location = new System.Drawing.Point(16, 500);
            this.labelUpdate.Name = "labelUpdate";
            this.labelUpdate.Size = new System.Drawing.Size(160, 18);
            this.labelUpdate.TabIndex = 9;
            this.labelUpdate.TabStop = true;
            this.labelUpdate.Text = "Check for updates";
            this.labelUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelUpdate_LinkClicked);
            //
            // menuStrip
            //
            this.menuStrip.BackColor = System.Drawing.SystemColors.Control;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.accountToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(8, 4, 0, 4);
            this.menuStrip.Size = new System.Drawing.Size(360, 32);
            this.menuStrip.TabIndex = 10;
            this.menuStrip.Text = "menuStrip1";
            //
            // fileToolStripMenuItem
            //
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuImportAccount,
            this.menuBackup,
            this.toolStripSeparator1,
            this.menuSettings,
            this.menuLock,
            this.menuQuit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 24);
            this.fileToolStripMenuItem.Text = "File";
            //
            // menuImportAccount
            //
            this.menuImportAccount.Name = "menuImportAccount";
            this.menuImportAccount.Size = new System.Drawing.Size(180, 24);
            this.menuImportAccount.Text = "Import Account";
            this.menuImportAccount.Click += new System.EventHandler(this.menuImportAccount_Click);
            //
            // menuBackup
            //
            this.menuBackup.Name = "menuBackup";
            this.menuBackup.Size = new System.Drawing.Size(180, 24);
            this.menuBackup.Text = "Back up all accounts...";
            this.menuBackup.Click += new System.EventHandler(this.menuBackup_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(177, 6);
            //
            // menuSettings
            //
            this.menuSettings.Name = "menuSettings";
            this.menuSettings.Size = new System.Drawing.Size(180, 24);
            this.menuSettings.Text = "Settings";
            this.menuSettings.Click += new System.EventHandler(this.menuSettings_Click);
            //
            // menuLock
            //
            this.menuLock.Name = "menuLock";
            this.menuLock.Size = new System.Drawing.Size(180, 24);
            this.menuLock.Text = "Lock";
            this.menuLock.Click += new System.EventHandler(this.menuLock_Click);
            //
            // menuQuit
            //
            this.menuQuit.Name = "menuQuit";
            this.menuQuit.Size = new System.Drawing.Size(180, 24);
            this.menuQuit.Text = "Quit";
            this.menuQuit.Click += new System.EventHandler(this.menuQuit_Click);
            //
            // accountToolStripMenuItem
            //
            this.accountToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRename,
            this.menuGroup,
            this.menuLoginAgain,
            this.menuApproveQr,
            this.menuRecoveryKit,
            this.toolStripSeparator4,
            this.menuRemoveAccountFromManifest,
            this.menuDeactivateAuthenticator});
            this.accountToolStripMenuItem.Name = "accountToolStripMenuItem";
            this.accountToolStripMenuItem.Size = new System.Drawing.Size(126, 24);
            this.accountToolStripMenuItem.Text = "Selected Account";
            //
            // menuLoginAgain
            //
            this.menuLoginAgain.Name = "menuLoginAgain";
            this.menuLoginAgain.Size = new System.Drawing.Size(230, 24);
            this.menuLoginAgain.Text = "Login again";
            this.menuLoginAgain.Click += new System.EventHandler(this.menuLoginAgain_Click);
            //
            // menuRename
            //
            this.menuRename.Name = "menuRename";
            this.menuRename.Size = new System.Drawing.Size(230, 24);
            this.menuRename.Text = "Rename...";
            this.menuRename.Click += new System.EventHandler(this.menuRename_Click);
            //
            // menuGroup
            //
            this.menuGroup.Name = "menuGroup";
            this.menuGroup.Size = new System.Drawing.Size(230, 24);
            this.menuGroup.Text = "Group...";
            this.menuGroup.Click += new System.EventHandler(this.menuGroup_Click);
            //
            // menuApproveQr
            //
            this.menuApproveQr.Name = "menuApproveQr";
            this.menuApproveQr.Size = new System.Drawing.Size(230, 24);
            this.menuApproveQr.Text = "Approve login QR on screen";
            this.menuApproveQr.Click += new System.EventHandler(this.menuApproveQr_Click);
            //
            // menuRecoveryKit
            //
            this.menuRecoveryKit.Name = "menuRecoveryKit";
            this.menuRecoveryKit.Size = new System.Drawing.Size(230, 24);
            this.menuRecoveryKit.Text = "Save recovery kit...";
            this.menuRecoveryKit.Click += new System.EventHandler(this.menuRecoveryKit_Click);
            //
            // toolStripSeparator4
            //
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(227, 6);
            //
            // menuRemoveAccountFromManifest
            //
            this.menuRemoveAccountFromManifest.Name = "menuRemoveAccountFromManifest";
            this.menuRemoveAccountFromManifest.Size = new System.Drawing.Size(230, 24);
            this.menuRemoveAccountFromManifest.Text = "Remove from manifest";
            this.menuRemoveAccountFromManifest.Click += new System.EventHandler(this.menuRemoveAccountFromManifest_Click);
            //
            // menuDeactivateAuthenticator
            //
            this.menuDeactivateAuthenticator.Name = "menuDeactivateAuthenticator";
            this.menuDeactivateAuthenticator.Size = new System.Drawing.Size(230, 24);
            this.menuDeactivateAuthenticator.Text = "Deactivate Authenticator";
            this.menuDeactivateAuthenticator.Click += new System.EventHandler(this.menuDeactivateAuthenticator_Click);
            //
            // trayIcon
            //
            this.trayIcon.ContextMenuStrip = this.menuStripTray;
            this.trayIcon.Text = "Steam Desktop Authenticator 2";
            this.trayIcon.Visible = true;
            this.trayIcon.BalloonTipClicked += new System.EventHandler(this.trayIcon_BalloonTipClicked);
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
            //
            // menuStripTray
            //
            this.menuStripTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayRestore,
            this.toolStripSeparator2,
            this.trayAccounts,
            this.trayTradeConfirmations,
            this.trayCheckNow,
            this.trayCopySteamGuard,
            this.trayApproveQr,
            this.trayLock,
            this.toolStripSeparator3,
            this.trayQuit});
            this.menuStripTray.Name = "contextMenuStripTray";
            this.menuStripTray.Size = new System.Drawing.Size(216, 131);
            this.menuStripTray.Opening += new System.ComponentModel.CancelEventHandler(this.menuStripTray_Opening);
            //
            // trayRestore
            //
            this.trayRestore.Name = "trayRestore";
            this.trayRestore.Size = new System.Drawing.Size(215, 22);
            this.trayRestore.Text = "Restore";
            this.trayRestore.Click += new System.EventHandler(this.trayRestore_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(212, 6);
            //
            // trayAccounts
            //
            this.trayAccounts.Name = "trayAccounts";
            this.trayAccounts.Size = new System.Drawing.Size(215, 22);
            this.trayAccounts.Text = "Account";
            //
            // trayTradeConfirmations
            //
            this.trayTradeConfirmations.Name = "trayTradeConfirmations";
            this.trayTradeConfirmations.Size = new System.Drawing.Size(215, 22);
            this.trayTradeConfirmations.Text = "View confirmations";
            this.trayTradeConfirmations.Click += new System.EventHandler(this.trayTradeConfirmations_Click);
            //
            // trayCopySteamGuard
            //
            this.trayCopySteamGuard.Name = "trayCopySteamGuard";
            this.trayCopySteamGuard.Size = new System.Drawing.Size(215, 22);
            this.trayCopySteamGuard.Text = "Copy login code";
            this.trayCopySteamGuard.Click += new System.EventHandler(this.trayCopySteamGuard_Click);
            //
            // trayCheckNow
            //
            this.trayCheckNow.Name = "trayCheckNow";
            this.trayCheckNow.Size = new System.Drawing.Size(215, 22);
            this.trayCheckNow.Text = "Check for confirmations now";
            this.trayCheckNow.Click += new System.EventHandler(this.trayCheckNow_Click);
            //
            // trayApproveQr
            //
            this.trayApproveQr.Name = "trayApproveQr";
            this.trayApproveQr.Size = new System.Drawing.Size(215, 22);
            this.trayApproveQr.Text = "Approve login QR on screen";
            this.trayApproveQr.Click += new System.EventHandler(this.menuApproveQr_Click);
            //
            // trayLock
            //
            this.trayLock.Name = "trayLock";
            this.trayLock.Size = new System.Drawing.Size(215, 22);
            this.trayLock.Text = "Lock";
            this.trayLock.Click += new System.EventHandler(this.menuLock_Click);
            //
            // listMenu
            //
            this.listMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.listRename,
            this.listGroup,
            this.listLoginAgain,
            this.listConfirmations,
            this.listApproveQr,
            this.listRecoveryKit,
            this.toolStripSeparator5,
            this.listRemove});
            this.listMenu.Name = "listMenu";
            this.listMenu.Size = new System.Drawing.Size(216, 131);
            this.listMenu.Opening += new System.ComponentModel.CancelEventHandler(this.listMenu_Opening);
            //
            // listRename
            //
            this.listRename.Name = "listRename";
            this.listRename.Size = new System.Drawing.Size(215, 22);
            this.listRename.Text = "Rename...";
            this.listRename.Click += new System.EventHandler(this.menuRename_Click);
            //
            // listGroup
            //
            this.listGroup.Name = "listGroup";
            this.listGroup.Size = new System.Drawing.Size(215, 22);
            this.listGroup.Text = "Group...";
            this.listGroup.Click += new System.EventHandler(this.menuGroup_Click);
            //
            // listLoginAgain
            //
            this.listLoginAgain.Name = "listLoginAgain";
            this.listLoginAgain.Size = new System.Drawing.Size(215, 22);
            this.listLoginAgain.Text = "Login again";
            this.listLoginAgain.Click += new System.EventHandler(this.menuLoginAgain_Click);
            //
            // listConfirmations
            //
            this.listConfirmations.Name = "listConfirmations";
            this.listConfirmations.Size = new System.Drawing.Size(215, 22);
            this.listConfirmations.Text = "View confirmations";
            this.listConfirmations.Click += new System.EventHandler(this.btnTradeConfirmations_Click);
            //
            // listApproveQr
            //
            this.listApproveQr.Name = "listApproveQr";
            this.listApproveQr.Size = new System.Drawing.Size(215, 22);
            this.listApproveQr.Text = "Approve login QR on screen";
            this.listApproveQr.Click += new System.EventHandler(this.menuApproveQr_Click);
            //
            // listRecoveryKit
            //
            this.listRecoveryKit.Name = "listRecoveryKit";
            this.listRecoveryKit.Size = new System.Drawing.Size(215, 22);
            this.listRecoveryKit.Text = "Save recovery kit...";
            this.listRecoveryKit.Click += new System.EventHandler(this.menuRecoveryKit_Click);
            //
            // toolStripSeparator5
            //
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(212, 6);
            //
            // listRemove
            //
            this.listRemove.Name = "listRemove";
            this.listRemove.Size = new System.Drawing.Size(215, 22);
            this.listRemove.Text = "Remove from manifest";
            this.listRemove.Click += new System.EventHandler(this.menuRemoveAccountFromManifest_Click);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(212, 6);
            //
            // trayQuit
            //
            this.trayQuit.Name = "trayQuit";
            this.trayQuit.Size = new System.Drawing.Size(215, 22);
            this.trayQuit.Text = "Quit";
            this.trayQuit.Click += new System.EventHandler(this.trayQuit_Click);
            //
            // timerTradesPopup
            //
            this.timerTradesPopup.Enabled = true;
            this.timerTradesPopup.Interval = 5000;
            this.timerTradesPopup.Tick += new System.EventHandler(this.timerTradesPopup_Tick);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.SystemColors.Control;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStatus.Location = new System.Drawing.Point(160, 6);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(184, 20);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // panelSearch
            //
            this.panelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSearch.BackColor = System.Drawing.SystemColors.Window;
            this.panelSearch.Controls.Add(this.txtAccSearch);
            this.panelSearch.Location = new System.Drawing.Point(16, 302);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Padding = new System.Windows.Forms.Padding(12, 9, 12, 8);
            this.panelSearch.Size = new System.Drawing.Size(192, 36);
            this.panelSearch.TabIndex = 12;
            //
            // txtAccSearch
            //
            this.txtAccSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtAccSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAccSearch.Location = new System.Drawing.Point(12, 9);
            this.txtAccSearch.Name = "txtAccSearch";
            this.txtAccSearch.PlaceholderText = "Search accounts";
            this.txtAccSearch.Size = new System.Drawing.Size(168, 18);
            this.txtAccSearch.TabIndex = 0;
            this.txtAccSearch.TextChanged += new System.EventHandler(this.txtAccSearch_TextChanged);
            //
            // btnGroup
            //
            this.btnGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGroup.Location = new System.Drawing.Point(216, 302);
            this.btnGroup.Name = "btnGroup";
            this.btnGroup.Size = new System.Drawing.Size(128, 36);
            this.btnGroup.TabIndex = 15;
            this.btnGroup.Text = "All accounts";
            this.btnGroup.UseVisualStyleBackColor = true;
            this.btnGroup.Click += new System.EventHandler(this.btnGroup_Click);
            //
            // menuGroups
            //
            this.menuGroups.Name = "menuGroups";
            this.menuGroups.Size = new System.Drawing.Size(181, 26);
            //
            // panelButtons
            //
            this.panelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelButtons.ColumnCount = 2;
            this.panelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelButtons.Controls.Add(this.btnSteamLogin, 0, 0);
            this.panelButtons.Controls.Add(this.btnManageEncryption, 1, 0);
            this.panelButtons.Location = new System.Drawing.Point(16, 40);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.RowCount = 1;
            this.panelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelButtons.Size = new System.Drawing.Size(328, 34);
            this.panelButtons.TabIndex = 14;
            //
            // MainForm
            //
            this.AcceptButton = this.btnCopy;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 528);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnGroup);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.panelSearch);
            this.Controls.Add(this.labelUpdate);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.groupAccount);
            this.Controls.Add(this.listAccounts);
            this.Controls.Add(this.panelCode);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(376, 520);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Steam Desktop Authenticator 2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.panelCode.ResumeLayout(false);
            this.panelCode.PerformLayout();
            this.groupAccount.ResumeLayout(false);
            this.groupAccount.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picAvatar)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.menuStripTray.ResumeLayout(false);
            this.listMenu.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSteamLogin;
        private System.Windows.Forms.Panel panelCode;
        private System.Windows.Forms.Label lblCodeTitle;
        private System.Windows.Forms.TextBox txtLoginToken;
        private System.Windows.Forms.ListBox listAccounts;
        private System.Windows.Forms.Timer timerSteamGuard;
        private TimeoutBar pbTimeout;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnTradeConfirmations;
        private System.Windows.Forms.Button btnManageEncryption;
        private System.Windows.Forms.Panel groupAccount;
        private System.Windows.Forms.PictureBox picAvatar;
        private System.Windows.Forms.Label lblAccountTitle;
        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.LinkLabel lblSession;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.LinkLabel labelUpdate;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuImportAccount;
        private System.Windows.Forms.ToolStripMenuItem menuBackup;
        private System.Windows.Forms.ToolStripMenuItem menuLock;
        private System.Windows.Forms.ToolStripMenuItem menuGroup;
        private System.Windows.Forms.ToolStripMenuItem listGroup;
        private System.Windows.Forms.ToolStripMenuItem trayLock;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.ContextMenuStrip menuGroups;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuQuit;
        private System.Windows.Forms.ToolStripMenuItem accountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuLoginAgain;
        private System.Windows.Forms.ToolStripMenuItem menuRename;
        private System.Windows.Forms.ToolStripMenuItem menuApproveQr;
        private System.Windows.Forms.ToolStripMenuItem menuRecoveryKit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveAccountFromManifest;
        private System.Windows.Forms.ToolStripMenuItem menuDeactivateAuthenticator;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip menuStripTray;
        private System.Windows.Forms.ToolStripMenuItem trayRestore;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem trayAccounts;
        private System.Windows.Forms.ToolStripMenuItem trayTradeConfirmations;
        private System.Windows.Forms.ToolStripMenuItem trayCopySteamGuard;
        private System.Windows.Forms.ToolStripMenuItem trayCheckNow;
        private System.Windows.Forms.ToolStripMenuItem trayApproveQr;
        private System.Windows.Forms.ContextMenuStrip listMenu;
        private System.Windows.Forms.ToolStripMenuItem listRename;
        private System.Windows.Forms.ToolStripMenuItem listLoginAgain;
        private System.Windows.Forms.ToolStripMenuItem listConfirmations;
        private System.Windows.Forms.ToolStripMenuItem listApproveQr;
        private System.Windows.Forms.ToolStripMenuItem listRecoveryKit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem listRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem trayQuit;
        private System.Windows.Forms.Timer timerTradesPopup;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel panelSearch;
        private System.Windows.Forms.TextBox txtAccSearch;
        private System.Windows.Forms.TableLayoutPanel panelButtons;
    }
}
