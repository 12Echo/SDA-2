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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.chkLive = new System.Windows.Forms.CheckBox();
            this.chkPeriodicChecking = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.panelInterval = new System.Windows.Forms.Panel();
            this.numPeriodicInterval = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.chkCheckAll = new System.Windows.Forms.CheckBox();
            this.chkConfirmMarket = new System.Windows.Forms.CheckBox();
            this.chkConfirmTrades = new System.Windows.Forms.CheckBox();
            this.panelInterval.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPeriodicInterval)).BeginInit();
            this.SuspendLayout();
            //
            // chkLive
            //
            this.chkLive.AutoSize = true;
            this.chkLive.Location = new System.Drawing.Point(16, 16);
            this.chkLive.Name = "chkLive";
            this.chkLive.Size = new System.Drawing.Size(270, 38);
            this.chkLive.TabIndex = 0;
            this.chkLive.Text = "Stay connected to Steam and check instantly\r\nwhen a confirmation appears";
            this.chkLive.UseVisualStyleBackColor = true;
            this.chkLive.CheckedChanged += new System.EventHandler(this.chkLive_CheckedChanged);
            //
            // chkPeriodicChecking
            //
            this.chkPeriodicChecking.AutoSize = true;
            this.chkPeriodicChecking.Location = new System.Drawing.Point(16, 66);
            this.chkPeriodicChecking.Name = "chkPeriodicChecking";
            this.chkPeriodicChecking.Size = new System.Drawing.Size(270, 38);
            this.chkPeriodicChecking.TabIndex = 1;
            this.chkPeriodicChecking.Text = "Periodically check for new confirmations\r\nand show a notification when they arrive";
            this.chkPeriodicChecking.UseVisualStyleBackColor = true;
            this.chkPeriodicChecking.CheckedChanged += new System.EventHandler(this.chkPeriodicChecking_CheckedChanged);
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSave.Location = new System.Drawing.Point(16, 254);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(308, 38);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // panelInterval
            //
            this.panelInterval.BackColor = System.Drawing.SystemColors.Window;
            this.panelInterval.Controls.Add(this.numPeriodicInterval);
            this.panelInterval.Location = new System.Drawing.Point(16, 116);
            this.panelInterval.Name = "panelInterval";
            this.panelInterval.Padding = new System.Windows.Forms.Padding(8, 7, 4, 6);
            this.panelInterval.Size = new System.Drawing.Size(72, 34);
            this.panelInterval.TabIndex = 2;
            //
            // numPeriodicInterval
            //
            this.numPeriodicInterval.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numPeriodicInterval.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numPeriodicInterval.Location = new System.Drawing.Point(8, 7);
            this.numPeriodicInterval.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numPeriodicInterval.Name = "numPeriodicInterval";
            this.numPeriodicInterval.Size = new System.Drawing.Size(60, 20);
            this.numPeriodicInterval.TabIndex = 0;
            this.numPeriodicInterval.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(96, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Seconds between confirmation checks";
            //
            // chkCheckAll
            //
            this.chkCheckAll.AutoSize = true;
            this.chkCheckAll.Location = new System.Drawing.Point(16, 162);
            this.chkCheckAll.Name = "chkCheckAll";
            this.chkCheckAll.Size = new System.Drawing.Size(240, 21);
            this.chkCheckAll.TabIndex = 4;
            this.chkCheckAll.Text = "Check all accounts for confirmations";
            this.chkCheckAll.UseVisualStyleBackColor = true;
            //
            // chkConfirmMarket
            //
            this.chkConfirmMarket.AutoSize = true;
            this.chkConfirmMarket.Location = new System.Drawing.Point(16, 190);
            this.chkConfirmMarket.Name = "chkConfirmMarket";
            this.chkConfirmMarket.Size = new System.Drawing.Size(230, 21);
            this.chkConfirmMarket.TabIndex = 5;
            this.chkConfirmMarket.Text = "Auto-confirm market transactions";
            this.chkConfirmMarket.UseVisualStyleBackColor = true;
            this.chkConfirmMarket.CheckedChanged += new System.EventHandler(this.chkConfirmMarket_CheckedChanged);
            //
            // chkConfirmTrades
            //
            this.chkConfirmTrades.AutoSize = true;
            this.chkConfirmTrades.Location = new System.Drawing.Point(16, 218);
            this.chkConfirmTrades.Name = "chkConfirmTrades";
            this.chkConfirmTrades.Size = new System.Drawing.Size(150, 21);
            this.chkConfirmTrades.TabIndex = 6;
            this.chkConfirmTrades.Text = "Auto-confirm trades";
            this.chkConfirmTrades.UseVisualStyleBackColor = true;
            this.chkConfirmTrades.CheckedChanged += new System.EventHandler(this.chkConfirmTrades_CheckedChanged);
            //
            // SettingsForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 308);
            this.Controls.Add(this.chkConfirmTrades);
            this.Controls.Add(this.chkConfirmMarket);
            this.Controls.Add(this.chkCheckAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelInterval);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkPeriodicChecking);
            this.Controls.Add(this.chkLive);
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

        private System.Windows.Forms.CheckBox chkLive;
        private System.Windows.Forms.CheckBox chkPeriodicChecking;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panelInterval;
        private System.Windows.Forms.NumericUpDown numPeriodicInterval;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkCheckAll;
        private System.Windows.Forms.CheckBox chkConfirmMarket;
        private System.Windows.Forms.CheckBox chkConfirmTrades;
    }
}
