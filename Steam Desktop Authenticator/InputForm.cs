using System;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public partial class InputForm : Form
    {
        public bool Canceled = false;
        private bool userClosed = true;
        private readonly bool password;

        public InputForm(string label, bool password = false)
        {
            InitializeComponent();
            Theme.Apply(this);
            Language.Apply(this);
            this.labelText.Text = Language.T(label);
            this.password = password;

            btnShow.Visible = password;
            if (password)
                this.txtBox.PasswordChar = '*';
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int pad = LogicalToDeviceUnits(16);
            labelText.MaximumSize = new System.Drawing.Size(ClientSize.Width - pad * 2, 0);
            labelText.AutoSize = true;

            panelInput.Top = labelText.Bottom + LogicalToDeviceUnits(12);
            btnAccept.Top = btnCancel.Top = panelInput.Bottom + pad;
            ClientSize = new System.Drawing.Size(ClientSize.Width, btnAccept.Bottom + pad);
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            bool shown = txtBox.PasswordChar == '\0';
            txtBox.PasswordChar = shown ? '*' : '\0';
            btnShow.Text = Language.T(shown ? "Show" : "Hide");
            txtBox.Focus();
            txtBox.SelectionStart = txtBox.TextLength;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Canceled = false;
            this.userClosed = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Canceled = true;
            this.userClosed = false;
            this.Close();
        }

        private void InputForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.userClosed)
            {
                // Set Canceled = true when the user hits the X button.
                this.Canceled = true;
            }
        }
    }
}
