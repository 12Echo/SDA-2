namespace Steam_Desktop_Authenticator
{
    partial class TradePopupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TradePopupForm));
            this.lblDesc = new System.Windows.Forms.Label();
            this.btnDeny = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblAccount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblDesc
            //
            this.lblDesc.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDesc.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblDesc.Location = new System.Drawing.Point(16, 38);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(232, 40);
            this.lblDesc.TabIndex = 1;
            this.lblDesc.Text = "trade description";
            //
            // btnDeny
            //
            this.btnDeny.Location = new System.Drawing.Point(16, 108);
            this.btnDeny.Name = "btnDeny";
            this.btnDeny.Size = new System.Drawing.Size(112, 34);
            this.btnDeny.TabIndex = 2;
            this.btnDeny.Text = "Deny";
            this.btnDeny.UseVisualStyleBackColor = true;
            this.btnDeny.Click += new System.EventHandler(this.btnDeny_Click);
            //
            // btnAccept
            //
            this.btnAccept.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnAccept.Location = new System.Drawing.Point(136, 108);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(112, 34);
            this.btnAccept.TabIndex = 3;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = false;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            //
            // lblStatus
            //
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStatus.Location = new System.Drawing.Point(16, 82);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(232, 18);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "status";
            //
            // lblAccount
            //
            this.lblAccount.AutoEllipsis = true;
            this.lblAccount.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccount.Location = new System.Drawing.Point(16, 12);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(232, 24);
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "account name";
            //
            // TradePopupForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 158);
            this.Controls.Add(this.lblAccount);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnDeny);
            this.Controls.Add(this.lblDesc);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TradePopupForm";
            this.Text = "New confirmation";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.TradePopupForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Button btnDeny;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblAccount;
    }
}
