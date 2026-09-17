namespace Steam_Desktop_Authenticator
{
    partial class WelcomeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeForm));
            this.label1 = new System.Windows.Forms.Label();
            this.btnImportConfig = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnJustStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(388, 64);
            this.label1.TabIndex = 0;
            this.label1.Text = "Welcome to\r\nSteam Desktop Authenticator";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnImportConfig
            //
            this.btnImportConfig.Location = new System.Drawing.Point(16, 210);
            this.btnImportConfig.Name = "btnImportConfig";
            this.btnImportConfig.Size = new System.Drawing.Size(388, 58);
            this.btnImportConfig.TabIndex = 3;
            this.btnImportConfig.Text = "Import accounts from an existing SDA install on this PC";
            this.btnImportConfig.UseVisualStyleBackColor = true;
            this.btnImportConfig.Click += new System.EventHandler(this.btnImportConfig_Click);
            //
            // label2
            //
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label2.Location = new System.Drawing.Point(16, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(388, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Select an option to get started";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnJustStart
            //
            this.btnJustStart.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnJustStart.Location = new System.Drawing.Point(16, 140);
            this.btnJustStart.Name = "btnJustStart";
            this.btnJustStart.Size = new System.Drawing.Size(388, 58);
            this.btnJustStart.TabIndex = 2;
            this.btnJustStart.Text = "This is my first time and\r\nI just want to sign into my Steam account";
            this.btnJustStart.UseVisualStyleBackColor = false;
            this.btnJustStart.Click += new System.EventHandler(this.btnJustStart_Click);
            //
            // WelcomeForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 292);
            this.Controls.Add(this.btnJustStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnImportConfig);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "WelcomeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Steam Desktop Authenticator";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnImportConfig;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnJustStart;
    }
}
