namespace Steam_Desktop_Authenticator
{
    partial class InputForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputForm));
            this.labelText = new System.Windows.Forms.Label();
            this.panelInput = new System.Windows.Forms.Panel();
            this.txtBox = new System.Windows.Forms.TextBox();
            this.btnShow = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lnkExtra = new System.Windows.Forms.LinkLabel();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            //
            // labelText
            //
            this.labelText.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelText.Location = new System.Drawing.Point(16, 16);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(368, 96);
            this.labelText.TabIndex = 0;
            this.labelText.Text = "Sample Text~~~";
            //
            // panelInput
            //
            this.panelInput.BackColor = System.Drawing.SystemColors.Window;
            this.panelInput.Controls.Add(this.txtBox);
            this.panelInput.Controls.Add(this.btnShow);
            this.panelInput.Location = new System.Drawing.Point(16, 120);
            this.panelInput.Name = "panelInput";
            this.panelInput.Padding = new System.Windows.Forms.Padding(12, 9, 12, 8);
            this.panelInput.Size = new System.Drawing.Size(368, 40);
            this.panelInput.TabIndex = 1;
            //
            // txtBox
            //
            this.txtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBox.Location = new System.Drawing.Point(12, 9);
            this.txtBox.Name = "txtBox";
            this.txtBox.Size = new System.Drawing.Size(344, 20);
            this.txtBox.TabIndex = 0;
            //
            // btnShow
            //
            this.btnShow.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnShow.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShow.Location = new System.Drawing.Point(300, 9);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(56, 23);
            this.btnShow.TabIndex = 1;
            this.btnShow.TabStop = false;
            this.btnShow.Text = "Show";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Visible = false;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            //
            // btnAccept
            //
            this.btnAccept.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnAccept.Location = new System.Drawing.Point(272, 176);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(112, 34);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = false;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(152, 176);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 34);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // lnkExtra
            //
            this.lnkExtra.AutoSize = true;
            this.lnkExtra.Location = new System.Drawing.Point(16, 184);
            this.lnkExtra.Name = "lnkExtra";
            this.lnkExtra.Size = new System.Drawing.Size(100, 17);
            this.lnkExtra.TabIndex = 4;
            this.lnkExtra.TabStop = true;
            this.lnkExtra.Text = "Forgot passkey?";
            this.lnkExtra.Visible = false;
            this.lnkExtra.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkExtra_LinkClicked);
            //
            // InputForm
            //
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(400, 226);
            this.Controls.Add(this.lnkExtra);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.labelText);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Steam Desktop Authenticator 2";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InputForm_FormClosing);
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.Panel panelInput;
        public System.Windows.Forms.TextBox txtBox;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel lnkExtra;
    }
}
