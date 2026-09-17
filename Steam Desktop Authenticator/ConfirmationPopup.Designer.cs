namespace Steam_Desktop_Authenticator
{
    partial class ConfirmationPopup
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
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblCounter = new System.Windows.Forms.Label();
            this.lblHeadline = new System.Windows.Forms.Label();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnDeny = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.timerHide = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // lblAccount
            //
            this.lblAccount.AutoEllipsis = true;
            this.lblAccount.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccount.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblAccount.Location = new System.Drawing.Point(16, 12);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(240, 16);
            this.lblAccount.TabIndex = 0;
            this.lblAccount.Text = "account";
            //
            // lblCounter
            //
            this.lblCounter.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCounter.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCounter.Location = new System.Drawing.Point(256, 12);
            this.lblCounter.Name = "lblCounter";
            this.lblCounter.Size = new System.Drawing.Size(64, 16);
            this.lblCounter.TabIndex = 1;
            this.lblCounter.Text = "+2 more";
            this.lblCounter.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            // lblHeadline
            //
            this.lblHeadline.AutoEllipsis = true;
            this.lblHeadline.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeadline.Location = new System.Drawing.Point(16, 32);
            this.lblHeadline.Name = "lblHeadline";
            this.lblHeadline.Size = new System.Drawing.Size(328, 22);
            this.lblHeadline.TabIndex = 2;
            this.lblHeadline.Text = "headline";
            //
            // lblSummary
            //
            this.lblSummary.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSummary.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblSummary.Location = new System.Drawing.Point(16, 56);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(328, 36);
            this.lblSummary.TabIndex = 3;
            this.lblSummary.Text = "summary";
            //
            // btnDeny
            //
            this.btnDeny.Location = new System.Drawing.Point(16, 100);
            this.btnDeny.Name = "btnDeny";
            this.btnDeny.Size = new System.Drawing.Size(110, 32);
            this.btnDeny.TabIndex = 4;
            this.btnDeny.Text = "Deny";
            this.btnDeny.UseVisualStyleBackColor = true;
            this.btnDeny.Click += new System.EventHandler(this.btnDeny_Click);
            //
            // btnAccept
            //
            this.btnAccept.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnAccept.Location = new System.Drawing.Point(134, 100);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(110, 32);
            this.btnAccept.TabIndex = 5;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = false;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            //
            // btnClose
            //
            this.btnClose.Location = new System.Drawing.Point(322, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(26, 22);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "✕";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // timerHide
            //
            this.timerHide.Interval = 7000;
            this.timerHide.Tick += new System.EventHandler(this.timerHide_Tick);
            //
            // ConfirmationPopup
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(360, 148);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnDeny);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.lblHeadline);
            this.Controls.Add(this.lblCounter);
            this.Controls.Add(this.lblAccount);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ConfirmationPopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Confirmation";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblAccount;
        private System.Windows.Forms.Label lblCounter;
        private System.Windows.Forms.Label lblHeadline;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnDeny;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Timer timerHide;
    }
}
