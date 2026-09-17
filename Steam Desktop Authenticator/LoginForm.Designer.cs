namespace Steam_Desktop_Authenticator
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.label1 = new System.Windows.Forms.Label();
            this.panelUsername = new System.Windows.Forms.Panel();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.panelPassword = new System.Windows.Forms.Panel();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSteamLogin = new System.Windows.Forms.Button();
            this.labelLoginExplanation = new System.Windows.Forms.Label();
            this.panelUsername.SuspendLayout();
            this.panelPassword.SuspendLayout();
            this.SuspendLayout();
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label1.Location = new System.Drawing.Point(16, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            //
            // panelUsername
            //
            this.panelUsername.BackColor = System.Drawing.SystemColors.Window;
            this.panelUsername.Controls.Add(this.txtUsername);
            this.panelUsername.Location = new System.Drawing.Point(16, 110);
            this.panelUsername.Name = "panelUsername";
            this.panelUsername.Padding = new System.Windows.Forms.Padding(12, 9, 12, 8);
            this.panelUsername.Size = new System.Drawing.Size(328, 36);
            this.panelUsername.TabIndex = 1;
            //
            // txtUsername
            //
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUsername.Location = new System.Drawing.Point(12, 9);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(304, 18);
            this.txtUsername.TabIndex = 0;
            //
            // panelPassword
            //
            this.panelPassword.BackColor = System.Drawing.SystemColors.Window;
            this.panelPassword.Controls.Add(this.txtPassword);
            this.panelPassword.Location = new System.Drawing.Point(16, 178);
            this.panelPassword.Name = "panelPassword";
            this.panelPassword.Padding = new System.Windows.Forms.Padding(12, 9, 12, 8);
            this.panelPassword.Size = new System.Drawing.Size(328, 36);
            this.panelPassword.TabIndex = 3;
            //
            // txtPassword
            //
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPassword.Location = new System.Drawing.Point(12, 9);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(304, 18);
            this.txtPassword.TabIndex = 0;
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label2.Location = new System.Drawing.Point(16, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Password";
            //
            // btnSteamLogin
            //
            this.btnSteamLogin.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSteamLogin.Location = new System.Drawing.Point(16, 234);
            this.btnSteamLogin.Name = "btnSteamLogin";
            this.btnSteamLogin.Size = new System.Drawing.Size(328, 38);
            this.btnSteamLogin.TabIndex = 4;
            this.btnSteamLogin.Text = "Login";
            this.btnSteamLogin.UseVisualStyleBackColor = false;
            this.btnSteamLogin.Click += new System.EventHandler(this.btnSteamLogin_Click);
            //
            // labelLoginExplanation
            //
            this.labelLoginExplanation.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLoginExplanation.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelLoginExplanation.Location = new System.Drawing.Point(16, 16);
            this.labelLoginExplanation.Name = "labelLoginExplanation";
            this.labelLoginExplanation.Size = new System.Drawing.Size(328, 64);
            this.labelLoginExplanation.TabIndex = 5;
            this.labelLoginExplanation.Text = "This will activate Steam Desktop Authenticator 2 on your Steam account. This requir" +
    "es a phone number that can receive SMS.";
            //
            // LoginForm
            //
            this.AcceptButton = this.btnSteamLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 290);
            this.Controls.Add(this.labelLoginExplanation);
            this.Controls.Add(this.btnSteamLogin);
            this.Controls.Add(this.panelPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panelUsername);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Steam Login";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.panelUsername.ResumeLayout(false);
            this.panelUsername.PerformLayout();
            this.panelPassword.ResumeLayout(false);
            this.panelPassword.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Panel panelPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSteamLogin;
        private System.Windows.Forms.Label labelLoginExplanation;
    }
}
