namespace Steam_Desktop_Authenticator
{
    partial class MessageForm
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
            this.stripe = new System.Windows.Forms.Panel();
            this.labelText = new System.Windows.Forms.Label();
            this.panelCode = new System.Windows.Forms.Panel();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnPrimary = new System.Windows.Forms.Button();
            this.btnSecondary = new System.Windows.Forms.Button();
            this.btnThird = new System.Windows.Forms.Button();
            this.panelCode.SuspendLayout();
            this.SuspendLayout();
            //
            // stripe
            //
            this.stripe.Dock = System.Windows.Forms.DockStyle.Left;
            this.stripe.Location = new System.Drawing.Point(0, 0);
            this.stripe.Name = "stripe";
            this.stripe.Size = new System.Drawing.Size(4, 200);
            this.stripe.TabIndex = 0;
            //
            // labelText
            //
            this.labelText.AutoSize = true;
            this.labelText.Location = new System.Drawing.Point(24, 20);
            this.labelText.MaximumSize = new System.Drawing.Size(372, 0);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(60, 17);
            this.labelText.TabIndex = 1;
            this.labelText.Text = "message";
            //
            // panelCode
            //
            this.panelCode.BackColor = System.Drawing.SystemColors.Window;
            this.panelCode.Controls.Add(this.txtCode);
            this.panelCode.Controls.Add(this.btnCopy);
            this.panelCode.Location = new System.Drawing.Point(24, 80);
            this.panelCode.Name = "panelCode";
            this.panelCode.Padding = new System.Windows.Forms.Padding(12, 9, 6, 6);
            this.panelCode.Size = new System.Drawing.Size(372, 46);
            this.panelCode.TabIndex = 2;
            //
            // txtCode
            //
            this.txtCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCode.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCode.ForeColor = System.Drawing.SystemColors.Highlight;
            this.txtCode.Location = new System.Drawing.Point(12, 9);
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(278, 22);
            this.txtCode.TabIndex = 0;
            this.txtCode.TabStop = false;
            //
            // btnCopy
            //
            this.btnCopy.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCopy.Location = new System.Drawing.Point(290, 9);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(76, 31);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            //
            // btnPrimary
            //
            this.btnPrimary.BackColor = System.Drawing.SystemColors.Highlight;
            this.btnPrimary.Location = new System.Drawing.Point(296, 146);
            this.btnPrimary.Name = "btnPrimary";
            this.btnPrimary.Size = new System.Drawing.Size(100, 34);
            this.btnPrimary.TabIndex = 3;
            this.btnPrimary.Text = "OK";
            this.btnPrimary.UseVisualStyleBackColor = false;
            this.btnPrimary.Click += new System.EventHandler(this.btnPrimary_Click);
            //
            // btnSecondary
            //
            this.btnSecondary.Location = new System.Drawing.Point(188, 146);
            this.btnSecondary.Name = "btnSecondary";
            this.btnSecondary.Size = new System.Drawing.Size(100, 34);
            this.btnSecondary.TabIndex = 4;
            this.btnSecondary.Text = "Cancel";
            this.btnSecondary.UseVisualStyleBackColor = true;
            this.btnSecondary.Click += new System.EventHandler(this.btnSecondary_Click);
            //
            // btnThird
            //
            this.btnThird.Location = new System.Drawing.Point(80, 146);
            this.btnThird.Name = "btnThird";
            this.btnThird.Size = new System.Drawing.Size(100, 34);
            this.btnThird.TabIndex = 5;
            this.btnThird.Text = "Cancel";
            this.btnThird.UseVisualStyleBackColor = true;
            this.btnThird.Click += new System.EventHandler(this.btnThird_Click);
            //
            // MessageForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 200);
            this.Controls.Add(this.btnThird);
            this.Controls.Add(this.btnSecondary);
            this.Controls.Add(this.btnPrimary);
            this.Controls.Add(this.panelCode);
            this.Controls.Add(this.labelText);
            this.Controls.Add(this.stripe);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Steam Desktop Authenticator 2";
            this.panelCode.ResumeLayout(false);
            this.panelCode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel stripe;
        private System.Windows.Forms.Label labelText;
        private System.Windows.Forms.Panel panelCode;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnPrimary;
        private System.Windows.Forms.Button btnSecondary;
        private System.Windows.Forms.Button btnThird;
    }
}
