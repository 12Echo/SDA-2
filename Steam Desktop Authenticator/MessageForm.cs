using System;
using System.Drawing;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public partial class MessageForm : Form
    {
        private DialogResult primaryResult;
        private DialogResult secondaryResult;
        private DialogResult thirdResult;
        private bool hasSecondary;
        private bool hasThird;
        private bool hasCode;

        public static DialogResult Show(string text)
        {
            return Show(text, "Steam Desktop Authenticator", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            return Show(text, caption, buttons, MessageBoxIcon.None);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            using (var form = new MessageForm(text, caption, buttons, icon, null))
                return form.ShowDialog();
        }

        public static DialogResult ShowCode(string text, string caption, string code)
        {
            using (var form = new MessageForm(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information, code))
                return form.ShowDialog();
        }

        private MessageForm(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, string code)
        {
            InitializeComponent();
            Theme.Apply(this);

            Text = caption;
            labelText.Text = text;
            hasCode = code != null;
            txtCode.Text = code ?? "";
            panelCode.Visible = hasCode;
            stripe.BackColor = StripeColor(icon);

            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    SetButtons("OK", DialogResult.OK, "Cancel", DialogResult.Cancel);
                    break;
                case MessageBoxButtons.YesNo:
                    SetButtons("Yes", DialogResult.Yes, "No", DialogResult.No);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    SetButtons("Yes", DialogResult.Yes, "No", DialogResult.No, "Cancel", DialogResult.Cancel);
                    break;
                default:
                    SetButtons("OK", DialogResult.OK);
                    break;
            }
        }

        private void SetButtons(string primary, DialogResult primaryResult, string secondary = null, DialogResult secondaryResult = DialogResult.None, string third = null, DialogResult thirdResult = DialogResult.None)
        {
            btnPrimary.Text = primary;
            this.primaryResult = primaryResult;

            hasSecondary = secondary != null;
            btnSecondary.Visible = hasSecondary;
            btnSecondary.Text = secondary ?? "";
            this.secondaryResult = secondaryResult;

            hasThird = third != null;
            btnThird.Visible = hasThird;
            btnThird.Text = third ?? "";
            this.thirdResult = thirdResult;

            AcceptButton = btnPrimary;
            CancelButton = hasThird ? btnThird : (hasSecondary ? btnSecondary : btnPrimary);
        }

        private static Color StripeColor(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: return Theme.Danger;
                case MessageBoxIcon.Warning: return Theme.Warning;
                case MessageBoxIcon.Information: return Theme.Accent;
                default: return Theme.Border;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int pad = LogicalToDeviceUnits(20);
            int gap = LogicalToDeviceUnits(12);
            int left = LogicalToDeviceUnits(24);
            int width = ClientSize.Width - left - pad;

            labelText.MaximumSize = new Size(width, 0);
            labelText.Location = new Point(left, pad);
            int y = labelText.Bottom + gap;

            if (hasCode)
            {
                panelCode.Location = new Point(left, y);
                panelCode.Width = width;
                y = panelCode.Bottom + gap;
            }

            int buttonsTop = y + LogicalToDeviceUnits(4);
            int x = ClientSize.Width - pad;
            var buttons = new[] { btnPrimary, hasSecondary ? btnSecondary : null, hasThird ? btnThird : null };
            foreach (var button in buttons)
            {
                if (button == null) continue;
                x -= button.Width;
                button.Location = new Point(x, buttonsTop);
                x -= LogicalToDeviceUnits(8);
            }

            ClientSize = new Size(ClientSize.Width, buttonsTop + btnPrimary.Height + pad);
        }

        private void btnPrimary_Click(object sender, EventArgs e)
        {
            DialogResult = primaryResult;
            Close();
        }

        private void btnSecondary_Click(object sender, EventArgs e)
        {
            DialogResult = secondaryResult;
            Close();
        }

        private void btnThird_Click(object sender, EventArgs e)
        {
            DialogResult = thirdResult;
            Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtCode.Text);
            btnCopy.Text = "Copied";
        }
    }
}
