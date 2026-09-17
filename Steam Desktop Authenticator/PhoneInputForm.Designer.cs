namespace Steam_Desktop_Authenticator
{
    partial class PhoneInputForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhoneInputForm));
            this.panelCountryCode = new System.Windows.Forms.Panel();
            this.txtCountryCode = new System.Windows.Forms.MaskedTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panelPhoneNumber = new System.Windows.Forms.Panel();
            this.txtPhoneNumber = new System.Windows.Forms.MaskedTextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelCountryCode.SuspendLayout();
            this.panelPhoneNumber.SuspendLayout();
            this.SuspendLayout();
            //
            // panelCountryCode
            //
            this.panelCountryCode.BackColor = System.Drawing.SystemColors.Window;
            this.panelCountryCode.Controls.Add(this.txtCountryCode);
            this.panelCountryCode.Location = new System.Drawing.Point(16, 106);
            this.panelCountryCode.Name = "panelCountryCode";
            this.panelCountryCode.Padding = new System.Windows.Forms.Padding(12, 9, 12, 8);
            this.panelCountryCode.Size = new System.Drawing.Size(80, 40);
            this.panelCountryCode.TabIndex = 2;
            //
            // txtCountryCode
            //
            this.txtCountryCode.AsciiOnly = true;
            this.txtCountryCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCountryCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCountryCode.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCountryCode.Location = new System.Drawing.Point(12, 9);
            this.txtCountryCode.Mask = "AA";
            this.txtCountryCode.Name = "txtCountryCode";
            this.txtCountryCode.Size = new System.Drawing.Size(56, 20);
            this.txtCountryCode.TabIndex = 0;
            this.txtCountryCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCountryCode_KeyPress);
            this.txtCountryCode.Leave += new System.EventHandler(this.txtCountryCode_Leave);
            //
            // label1
            //
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(388, 60);
            this.label1.TabIndex = 0;
            this.label1.Text = "Your Steam account requires a phone number to add a mobile authenticator. The num" +
    "ber must be able to receive SMS. VoIP and virtual phone numbers are not supporte" +
    "d.";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(276, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Two letter country code of the phone number";
            //
            // label3
            //
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(210, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Phone number (+1 0000000000)";
            //
            // panelPhoneNumber
            //
            this.panelPhoneNumber.BackColor = System.Drawing.SystemColors.Window;
            this.panelPhoneNumber.Controls.Add(this.txtPhoneNumber);
            this.panelPhoneNumber.Location = new System.Drawing.Point(16, 180);
            this.panelPhoneNumber.Name = "panelPhoneNumber";
            this.panelPhoneNumber.Padding = new System.Windows.Forms.Padding(12, 9, 12, 8);
            this.panelPhoneNumber.Size = new System.Drawing.Size(240, 40);
            this.panelPhoneNumber.TabIndex = 4;
            //
            // txtPhoneNumber
            //
            this.txtPhoneNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPhoneNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPhoneNumber.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhoneNumber.Location = new System.Drawing.Point(12, 9);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(216, 20);
            this.txtPhoneNumber.TabIndex = 0;
            this.txtPhoneNumber.Text = "+1 ";
            this.txtPhoneNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPhoneNumber_KeyPress);
            //
            // btnSubmit
            //
            this.btnSubmit.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnSubmit.Location = new System.Drawing.Point(16, 240);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(112, 34);
            this.btnSubmit.TabIndex = 5;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(136, 240);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 34);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // PhoneInputForm
            //
            this.AcceptButton = this.btnSubmit;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(420, 290);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panelPhoneNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panelCountryCode);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PhoneInputForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phone Number";
            this.panelCountryCode.ResumeLayout(false);
            this.panelCountryCode.PerformLayout();
            this.panelPhoneNumber.ResumeLayout(false);
            this.panelPhoneNumber.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelCountryCode;
        private System.Windows.Forms.MaskedTextBox txtCountryCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panelPhoneNumber;
        private System.Windows.Forms.MaskedTextBox txtPhoneNumber;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnCancel;
    }
}
