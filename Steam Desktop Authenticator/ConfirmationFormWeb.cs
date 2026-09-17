using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamAuth;
using System.Linq;
using System.Runtime.InteropServices;

namespace Steam_Desktop_Authenticator
{
    public partial class ConfirmationFormWeb : Form, IMessageFilter
    {
        private SteamGuardAccount steamAccount;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public ConfirmationFormWeb(SteamGuardAccount steamAccount)
        {
            InitializeComponent();
            Theme.Apply(this);
            this.steamAccount = steamAccount;
            this.Text = String.Format("Confirmations - {0}", steamAccount.AccountName);
            Application.AddMessageFilter(this);
            this.FormClosed += (s, e) => Application.RemoveMessageFilter(this);

            // Line the refresh button up with the buttons on the cards below it, scrollbar or not
            this.splitContainer1.Panel2.ClientSizeChanged += (s, e) => PlaceRefresh();
            this.Load += (s, e) => PlaceRefresh();
        }

        private void PlaceRefresh()
        {
            int pad = LogicalToDeviceUnits(16);
            btnRefresh.Left = this.splitContainer1.Panel2.DisplayRectangle.Right - pad - btnRefresh.Width;
        }

        // The wheel goes to whichever control has focus, send it to the list when the cursor is over it
        public bool PreFilterMessage(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x020A;
            if (m.Msg != WM_MOUSEWHEEL) return false;

            var panel = this.splitContainer1.Panel2;
            if (!panel.IsHandleCreated || m.HWnd == panel.Handle) return false;
            if (!panel.ClientRectangle.Contains(panel.PointToClient(Cursor.Position))) return false;

            SendMessage(panel.Handle, m.Msg, m.WParam, m.LParam);
            return true;
        }
        private async Task LoadData()
        {
            this.splitContainer1.Panel2.Controls.Clear();

            // Check for a valid refresh token first
            if (steamAccount.Session.IsRefreshTokenExpired())
            {
                MessageForm.Show("Your session has expired. Use the login again button under the selected account menu.", "Trade Confirmations", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Check for a valid access token, refresh it if needed
            if (steamAccount.Session.IsAccessTokenExpired())
            {
                try
                {
                    await steamAccount.Session.RefreshAccessToken();
                }
                catch (Exception ex)
                {
                    MessageForm.Show(ex.Message, "Steam Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }
            }

            try
            {
                var confirmations = await steamAccount.FetchConfirmationsAsync();

                if (confirmations == null || confirmations.Length == 0)
                {
                    ShowEmpty();
                    return;
                }

                // Docked panels stack in reverse order of adding, so add the last one first
                foreach (var confirmation in confirmations.Reverse())
                {
                    this.splitContainer1.Panel2.Controls.Add(BuildCard(confirmation));
                }
            }
            catch (Exception ex)
            {
                Label errorLabel = new Label() { Text = "Something went wrong:\n" + ex.Message, AutoSize = true, ForeColor = Theme.Danger, Location = new Point(16, 24) };
                this.splitContainer1.Panel2.Controls.Add(errorLabel);
            }
        }

        private Panel BuildCard(Confirmation confirmation)
        {
            int pad = LogicalToDeviceUnits(16);
            int icon = LogicalToDeviceUnits(56);
            int buttonWidth = LogicalToDeviceUnits(96);
            int buttonHeight = LogicalToDeviceUnits(32);
            int gap = LogicalToDeviceUnits(8);
            int captionHeight = LogicalToDeviceUnits(16);
            int headlineHeight = LogicalToDeviceUnits(22);
            int summaryHeight = Font.Height * 3;
            int contentHeight = Math.Max(icon, captionHeight + headlineHeight + summaryHeight);

            Panel panel = new Panel()
            {
                Dock = DockStyle.Top,
                Width = this.splitContainer1.Panel2.DisplayRectangle.Width,
                Height = contentHeight + pad * 2 + gap,
                BackColor = Theme.Surface
            };
            Theme.Card(panel, gap);

            int textLeft = pad;
            if (!string.IsNullOrEmpty(confirmation.Icon))
            {
                PictureBox pictureBox = new PictureBox()
                {
                    Size = new Size(icon, icon),
                    Location = new Point(pad, pad),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Theme.Background
                };
                try
                {
                    pictureBox.Load(confirmation.Icon);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load avatar: " + ex.Message);
                }
                panel.Controls.Add(pictureBox);
                textLeft = pad + icon + pad;
            }

            int textWidth = panel.Width - textLeft - buttonWidth - pad * 2;

            Label typeLabel = new Label()
            {
                Text = TypeName(confirmation).ToUpper(),
                ForeColor = Theme.TextMuted,
                Font = new Font("Segoe UI", 8.25F),
                Location = new Point(textLeft, pad),
                Size = new Size(textWidth, captionHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panel.Controls.Add(typeLabel);

            Label nameLabel = new Label()
            {
                Text = confirmation.Headline,
                AutoEllipsis = true,
                Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold),
                Location = new Point(textLeft, pad + captionHeight),
                Size = new Size(textWidth, LogicalToDeviceUnits(22)),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panel.Controls.Add(nameLabel);

            Label summaryLabel = new Label()
            {
                Text = confirmation.Summary == null ? "" : String.Join("\n", confirmation.Summary),
                ForeColor = Theme.TextMuted,
                Location = new Point(textLeft, pad + captionHeight + headlineHeight),
                Size = new Size(textWidth, summaryHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            panel.Controls.Add(summaryLabel);

            Label detailsLabel = new Label()
            {
                Text = DescribeConfirmation(confirmation),
                ForeColor = Theme.TextMuted,
                Font = new Font("Segoe UI", 8.25F),
                Location = new Point(textLeft, pad + captionHeight + headlineHeight + summaryHeight),
                Size = new Size(textWidth, LogicalToDeviceUnits(18)),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Visible = false
            };
            panel.Controls.Add(detailsLabel);

            // Click anywhere on the card to see the whole summary
            bool expanded = false;
            EventHandler toggle = (s, e) =>
            {
                expanded = !expanded;
                int fullHeight = expanded
                    ? TextRenderer.MeasureText(summaryLabel.Text, summaryLabel.Font, new Size(summaryLabel.Width, int.MaxValue), TextFormatFlags.WordBreak).Height
                    : summaryHeight;
                summaryLabel.Height = Math.Max(fullHeight, expanded ? 0 : summaryHeight);
                detailsLabel.Top = summaryLabel.Bottom + LogicalToDeviceUnits(4);
                detailsLabel.Visible = expanded;
                int content = captionHeight + headlineHeight + summaryLabel.Height + (expanded ? detailsLabel.Height + LogicalToDeviceUnits(4) : 0);
                panel.Height = Math.Max(icon, content) + pad * 2 + gap;
            };
            foreach (Control c in new Control[] { panel, typeLabel, nameLabel, summaryLabel, detailsLabel })
            {
                c.Cursor = Cursors.Hand;
                c.Click += toggle;
            }
            panel.Tag = toggle;

            ConfirmationButton acceptButton = new ConfirmationButton()
            {
                Text = confirmation.Accept,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(panel.Width - pad - buttonWidth, pad),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Confirmation = confirmation
            };
            acceptButton.FlatAppearance.BorderSize = 0;
            Theme.Primary(acceptButton);
            acceptButton.Click += btnAccept_Click;
            panel.Controls.Add(acceptButton);

            ConfirmationButton cancelButton = new ConfirmationButton()
            {
                Text = confirmation.Cancel,
                Size = new Size(buttonWidth, buttonHeight),
                Location = new Point(panel.Width - pad - buttonWidth, pad + buttonHeight + gap),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Confirmation = confirmation
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            Theme.Secondary(cancelButton);
            cancelButton.Click += btnCancel_Click;
            panel.Controls.Add(cancelButton);

            return panel;
        }

        private static string DescribeConfirmation(Confirmation confirmation)
        {
            switch (confirmation.ConfType)
            {
                case Confirmation.EMobileConfirmationType.Trade:
                    return "Trade offer " + confirmation.Creator + "  ·  Confirmation " + confirmation.ID;
                case Confirmation.EMobileConfirmationType.MarketListing:
                    return "Listing " + confirmation.Creator + "  ·  Confirmation " + confirmation.ID;
                default:
                    return "Confirmation " + confirmation.ID;
            }
        }

        private async void btnAccept_Click(object sender, EventArgs e)
        {
            await HandleCard((ConfirmationButton)sender, true);
        }

        private async void btnCancel_Click(object sender, EventArgs e)
        {
            await HandleCard((ConfirmationButton)sender, false);
        }

        // Only the card that was acted on goes away, the rest of the list stays where it is
        private async Task HandleCard(ConfirmationButton button, bool accept)
        {
            var card = button.Parent;
            foreach (Control c in card.Controls)
                if (c is Button) c.Enabled = false;

            bool ok = accept
                ? await steamAccount.AcceptConfirmation(button.Confirmation)
                : await steamAccount.DenyConfirmation(button.Confirmation);

            if (!ok)
            {
                foreach (Control c in card.Controls)
                    if (c is Button) c.Enabled = true;
                MessageForm.Show("Steam did not " + (accept ? "accept" : "cancel") + " this confirmation. It may already be handled, press Refresh to check.", "Confirmations", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var list = card.Parent;
            list.Controls.Remove(card);
            card.Dispose();
            if (list.Controls.Count == 0)
                ShowEmpty();
        }

        private void ShowEmpty()
        {
            Label emptyLabel = new Label() { Text = "Nothing to confirm", AutoSize = true, ForeColor = Theme.TextMuted, Location = new Point(16, 24) };
            this.splitContainer1.Panel2.Controls.Add(emptyLabel);
        }

        internal static string TypeName(Confirmation confirmation)
        {
            switch (confirmation.ConfType)
            {
                case Confirmation.EMobileConfirmationType.Trade: return "Trade";
                case Confirmation.EMobileConfirmationType.MarketListing: return "Market listing";
                case Confirmation.EMobileConfirmationType.PhoneNumberChange: return "Phone number change";
                case Confirmation.EMobileConfirmationType.AccountRecovery: return "Account recovery";
                case Confirmation.EMobileConfirmationType.FeatureOptOut: return "Feature opt out";
                case Confirmation.EMobileConfirmationType.Test: return "Test";
                default: return "Type " + (int)confirmation.ConfType;
            }
        }


        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            this.btnRefresh.Enabled = false;
            this.btnRefresh.Text = "Refreshing...";

            await this.LoadData();

            this.btnRefresh.Enabled = true;
            this.btnRefresh.Text = "Refresh";
        }

        private async void ConfirmationFormWeb_Shown(object sender, EventArgs e)
        {
            this.btnRefresh.Enabled = false;
            this.btnRefresh.Text = "Refreshing...";

            await this.LoadData();

            this.btnRefresh.Enabled = true;
            this.btnRefresh.Text = "Refresh";
        }
    }
}
