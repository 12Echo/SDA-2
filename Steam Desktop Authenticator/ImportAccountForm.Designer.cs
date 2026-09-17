namespace Steam_Desktop_Authenticator
{
    partial class ImportAccountForm
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportAccountForm));
            this.labelText = new System.Windows.Forms.Label();
            this.panelKey = new System.Windows.Forms.Panel();
            this.txtBox = new System.Windows.Forms.TextBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panelKey.SuspendLayout();
            this.SuspendLayout();
            //
            // labelText
            //
            this.labelText.AutoSize = true;
            this.labelText.Location = new System.Drawing.Point(16, 16);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(300, 17);
            this.labelText.TabIndex = 0;
            this.labelText.Text = "Encryption passkey, if the .maFile is encrypted";
            //
            // panelKey
            //
            this.panelKey.BackColor = System.Drawing.SystemColors.Window;
            this.panelKey.Controls.Add(this.txtBox);
            this.panelKey.Location = new System.Drawing.Point(16, 40);
            this.panelKey.Name = "panelKey";
            this.panelKey.Padding = new System.Windows.Forms.Padding(12, 9, 12, 8);
            this.panelKey.Size = new System.Drawing.Size(348, 36);
            this.panelKey.TabIndex = 1;
            //
            // txtBox
            //
            this.txtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBox.Location = new System.Drawing.Point(12, 9);
            this.txtBox.Name = "txtBox";
            this.txtBox.PasswordChar = '*';
            this.txtBox.Size = new System.Drawing.Size(324, 18);
            this.txtBox.TabIndex = 0;
            //
            // btnImport
            //
            this.btnImport.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnImport.Location = new System.Drawing.Point(136, 140);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(228, 34);
            this.btnImport.TabIndex = 3;
            this.btnImport.Text = "Select .maFile to import";
            this.btnImport.UseVisualStyleBackColor = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(16, 140);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 34);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // label1
            //
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.GrayText;
            this.label1.Location = new System.Drawing.Point(16, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 40);
            this.label1.TabIndex = 2;
            this.label1.Text = "If you import an encrypted .maFile, its manifest.json must be in the same folder.";
            //
            // ImportAccountForm
            //
            this.AcceptButton = this.btnImport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(380, 190);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.panelKey);
            this.Controls.Add(this.labelText);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportAccountForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import Account";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Import_maFile_Form_FormClosing);
            this.panelKey.ResumeLayout(false);
            this.panelKey.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.Panel panelKey;
        private System.Windows.Forms.TextBox txtBox;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
    }
}
