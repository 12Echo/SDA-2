using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteamAuth;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Steam_Desktop_Authenticator
{
    public partial class ConfirmationFormWeb : Form, IMessageFilter
    {
        private SteamGuardAccount steamAccount;
        private SteamGuardAccount[] accounts;
        private Func<SteamGuardAccount, string> nameOf;
        private readonly ContextMenuStrip menuPick = new ContextMenuStrip();
        private readonly Dictionary<ulong, string> details = new Dictionary<ulong, string>();

        internal const string TradeProtectionHint = "If Steam asked you to acknowledge its trade protection notice, open your inventory in a browser, go to Trade Offers, accept the notice there and try again.";

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public ConfirmationFormWeb(SteamGuardAccount steamAccount)
            : this(steamAccount, null, null)
        {
        }

        // With the other accounts handed in, the header dropdown switches between them without closing the window
        public ConfirmationFormWeb(SteamGuardAccount steamAccount, SteamGuardAccount[] accounts, Func<SteamGuardAccount, string> nameOf)
        {
            InitializeComponent();
            Theme.Apply(this);
            Language.Apply(this);
            Theme.Apply(menuPick);
            Theme.Dropdown(btnAccount);
            this.accounts = accounts ?? new SteamGuardAccount[] { steamAccount };
            this.nameOf = nameOf ?? (a => a.AccountName);
            ShowAccount(steamAccount);
            btnAccount.Enabled = this.accounts.Length > 1;
            Application.AddMessageFilter(this);
            this.FormClosed += (s, e) => Application.RemoveMessageFilter(this);

            // Line the refresh button up with the buttons on the cards below it, scrollbar or not
            this.splitContainer1.Panel2.ClientSizeChanged += (s, e) => PlaceRefresh();
            this.Load += (s, e) => PlaceRefresh();
        }

        private void ShowAccount(SteamGuardAccount account)
        {
            steamAccount = account;
            btnAccount.Text = nameOf(account);
            this.Text = String.Format("Confirmations - {0}", account.AccountName);
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            menuPick.Items.Clear();
            foreach (var account in accounts)
            {
                var item = new ToolStripMenuItem(nameOf(account)) { Tag = account, Checked = account == steamAccount };
                item.Click += async (s2, e2) =>
                {
                    var picked = (SteamGuardAccount)((ToolStripMenuItem)s2).Tag;
                    if (picked == steamAccount) return;
                    ShowAccount(picked);
                    await Refresh(true);
                };
                menuPick.Items.Add(item);
            }
            Theme.StyleMenuItems(menuPick.Items, false);
            menuPick.Width = Math.Max(btnAccount.Width, LogicalToDeviceUnits(200));
            menuPick.Show(btnAccount, new Point(0, btnAccount.Height + 4));
        }

        private void PlaceRefresh()
        {
            int pad = LogicalToDeviceUnits(16);
            int gap = LogicalToDeviceUnits(4);
            btnRefresh.Left = this.splitContainer1.Panel2.DisplayRectangle.Right - pad - btnRefresh.Width;
            btnCancelAll.Left = btnRefresh.Left - gap - btnCancelAll.Width;
            btnAcceptAll.Left = btnCancelAll.Left - gap - btnAcceptAll.Width;
        }

        // The header buttons act on the ticked cards, or on every card when none is ticked
        private List<ConfirmationButton> Cards(bool tickedOnly)
        {
            var found = new List<ConfirmationButton>();
            foreach (Control card in this.splitContainer1.Panel2.Controls)
            {
                var accept = card.Controls.OfType<ConfirmationButton>().FirstOrDefault();
                var tick = card.Controls.OfType<CheckBox>().FirstOrDefault();
                if (accept == null || tick == null) continue;
                if (!tickedOnly || tick.Checked) found.Add(accept);
            }
            return found;
        }

        private void UpdateBatchButtons()
        {
            int total = Cards(false).Count;
            int ticked = Cards(true).Count;
            btnAcceptAll.Enabled = btnCancelAll.Enabled = total > 0;
            btnAcceptAll.Text = ticked > 0 ? Language.T("Accept") + " " + ticked : Language.T("Accept all");
            btnCancelAll.Text = ticked > 0 ? Language.T("Cancel") + " " + ticked : Language.T("Cancel all");
        }

        private async void btnAcceptAll_Click(object sender, EventArgs e)
        {
            await HandleBatch(true);
        }

        private async void btnCancelAll_Click(object sender, EventArgs e)
        {
            await HandleBatch(false);
        }

        private async Task HandleBatch(bool accept)
        {
            var picked = Cards(true);
            bool all = picked.Count == 0;
            if (all) picked = Cards(false);
            if (picked.Count == 0) return;

            string what = picked.Count == 1 ? "this confirmation" : (all ? "all " : "the ") + picked.Count + (all ? " confirmations" : " ticked confirmations");
            var answer = MessageForm.Show((accept ? "Accept " : "Cancel ") + what + "?", "Confirmations", MessageBoxButtons.YesNo, accept ? MessageBoxIcon.Warning : MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            btnAcceptAll.Enabled = btnCancelAll.Enabled = btnRefresh.Enabled = false;
            var confirmations = picked.Select(b => b.Confirmation).ToArray();
            var account = steamAccount;
            int failed = await Handle(account, confirmations, accept);

            if (failed > 0)
            {
                string text = "Steam did not " + (accept ? "accept" : "cancel") + (failed == confirmations.Length ? " them" : " " + failed + " of them") + ". Some may already be handled, the list is refreshed now.";
                if (accept && confirmations.Any(c => c.ConfType == Confirmation.EMobileConfirmationType.Trade))
                    text += "\n\n" + TradeProtectionHint;
                MessageForm.Show(text, "Confirmations", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            btnRefresh.Enabled = true;
            if (account == steamAccount)
                await LoadData();
        }

        // The multi call is one request, but Steam has been refusing it while honouring single ones, so those are the fallback.
        // Returns how many stayed unhandled.
        internal static async Task<int> Handle(SteamGuardAccount account, Confirmation[] confirmations, bool accept)
        {
            if (!TimeAligner.Aligned) await TimeAligner.AlignTimeAsync();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (confirmations.Length > 1)
            {
                try
                {
                    bool ok = accept
                        ? await account.AcceptMultipleConfirmations(confirmations)
                        : await account.DenyMultipleConfirmations(confirmations);
                    if (ok) return 0;
                    Log.Write("Steam refused a batch " + (accept ? "accept" : "cancel") + " of " + confirmations.Length + " for " + account.AccountName + " after " + watch.ElapsedMilliseconds + " ms, trying one by one");
                }
                catch (Exception ex)
                {
                    Log.Error("Batch " + (accept ? "accept" : "cancel") + " for " + account.AccountName, ex);
                }
            }

            int failed = 0;
            for (int i = 0; i < confirmations.Length; i++)
            {
                if (i > 0) await Task.Delay(400);
                var one = System.Diagnostics.Stopwatch.StartNew();
                bool ok;
                try
                {
                    ok = accept
                        ? await account.AcceptConfirmation(confirmations[i])
                        : await account.DenyConfirmation(confirmations[i]);
                }
                catch (Exception ex)
                {
                    Log.Error((accept ? "Accept" : "Cancel") + " " + confirmations[i].ID + " for " + account.AccountName, ex);
                    ok = false;
                }
                if (!ok)
                {
                    failed++;
                    Log.Write("Steam refused to " + (accept ? "accept" : "cancel") + " confirmation " + confirmations[i].ID + " for " + account.AccountName);
                }
                else if (one.ElapsedMilliseconds > 3000)
                    Log.Write((accept ? "Accept" : "Cancel") + " of " + confirmations[i].ID + " for " + account.AccountName + " took " + one.ElapsedMilliseconds + " ms");
            }
            return failed;
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
            if (!TimeAligner.Aligned) await TimeAligner.AlignTimeAsync();

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
                UpdateBatchButtons();
            }
            catch (Exception ex)
            {
                Log.Error("Loading confirmations for " + steamAccount.AccountName, ex);
                Label errorLabel = new Label() { Text = "Something went wrong:\n" + ex.Message, AutoSize = true, ForeColor = Theme.Danger, Location = new Point(16, 24) };
                this.splitContainer1.Panel2.Controls.Add(errorLabel);
                UpdateBatchButtons();
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

            int tickWidth = LogicalToDeviceUnits(20);
            CheckBox tick = new CheckBox()
            {
                AutoSize = false,
                Size = new Size(tickWidth, tickWidth),
                Location = new Point(pad, pad + (contentHeight - tickWidth) / 2),
                Anchor = AnchorStyles.Left,
                TabStop = false
            };
            Theme.Toggle(tick);
            tick.CheckedChanged += (s, e) => UpdateBatchButtons();
            panel.Controls.Add(tick);

            int textLeft = pad + tickWidth + gap;
            if (!string.IsNullOrEmpty(confirmation.Icon))
            {
                PictureBox pictureBox = new PictureBox()
                {
                    Size = new Size(icon, icon),
                    Location = new Point(textLeft, pad),
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
                textLeft += icon + pad;
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

            Label idLabel = new Label()
            {
                Text = DescribeConfirmation(confirmation),
                AutoEllipsis = true,
                ForeColor = Theme.TextMuted,
                Font = new Font("Segoe UI", 8.25F),
                Location = new Point(textLeft, pad + captionHeight + headlineHeight + summaryHeight),
                Size = new Size(textWidth, LogicalToDeviceUnits(18)),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Visible = false
            };
            panel.Controls.Add(idLabel);

            // The details sit below the buttons and use the full width of the card, item names are long
            Panel detailsPanel = new Panel()
            {
                Location = new Point(textLeft, idLabel.Top),
                Size = new Size(panel.Width - pad - textLeft, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Theme.Surface,
                Visible = false
            };
            panel.Controls.Add(detailsPanel);

            // Click anywhere on the card to see the whole summary and what the confirmation actually contains
            bool expanded = false;
            bool loaded = false;
            int small = LogicalToDeviceUnits(6);
            Action layout = () =>
            {
                int fullHeight = expanded
                    ? TextRenderer.MeasureText(summaryLabel.Text, summaryLabel.Font, new Size(summaryLabel.Width, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix).Height + LogicalToDeviceUnits(4)
                    : summaryHeight;
                summaryLabel.Height = Math.Max(fullHeight, expanded ? 0 : summaryHeight);

                int y = summaryLabel.Bottom + small;
                detailsPanel.Visible = expanded && detailsPanel.Height > 0;
                if (detailsPanel.Visible)
                {
                    detailsPanel.Top = Math.Max(y, pad + buttonHeight * 2 + gap + small);
                    y = detailsPanel.Bottom + small;
                }
                idLabel.Top = y;
                idLabel.Visible = expanded;

                int content = (expanded ? idLabel.Bottom : summaryLabel.Bottom) - pad;
                panel.Height = Math.Max(icon, content) + pad * 2 + gap;
                panel.Invalidate();
            };
            EventHandler toggle = null;
            toggle = async (s, e) =>
            {
                expanded = !expanded;
                layout();
                if (expanded && !loaded)
                {
                    loaded = true;
                    await ShowDetails(confirmation, detailsPanel, layout, toggle);
                }
            };
            foreach (Control c in new Control[] { panel, typeLabel, nameLabel, summaryLabel, idLabel, detailsPanel })
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

        // Fills the card's detail area from the confirmation page: item lists for a trade, the page text for anything else
        private async Task ShowDetails(Confirmation confirmation, Panel host, Action layout, EventHandler click)
        {
            var account = steamAccount;
            var loading = SmallLabel(host, "Loading...", 0, click);
            host.Height = loading.Bottom;
            layout();

            string html;
            if (!details.TryGetValue(confirmation.ID, out html))
            {
                html = await TradeOffers.DetailsAsync(account, confirmation);
                if (html != null) details[confirmation.ID] = html;
            }
            if (host.IsDisposed) return;

            host.SuspendLayout();
            host.Controls.Clear();
            int y = 0;

            TradeOfferItems items = confirmation.ConfType == Confirmation.EMobileConfirmationType.Trade
                ? TradeOffers.ParseItems(html, account.Session.SteamID) : null;
            if (items != null)
            {
                y = ItemList(host, "YOU GIVE", items.Mine, y, click);
                y = ItemList(host, "YOU RECEIVE", items.Theirs, y + LogicalToDeviceUnits(6), click);
            }
            else if (html == null)
            {
                y = SmallLabel(host, "Steam did not return the details of this confirmation.", y, click).Bottom;
            }
            else
            {
                var lines = TradeOffers.TextLines(html);
                if (confirmation.ConfType == Confirmation.EMobileConfirmationType.Trade)
                {
                    KeepPage(confirmation, html);
                    lines = lines.Where(l => !Repeats(l, confirmation) && !Regex.IsMatch(l, @"^[\d\s.,:]+$")).ToList();
                }
                string text = lines.Count > 0 ? string.Join("\n", lines) : Language.T("Nothing more to show.");
                var label = new Label()
                {
                    ForeColor = Theme.Text,
                    Location = new Point(0, y),
                    MaximumSize = new Size(host.Width, 0),
                    AutoSize = true,
                    UseMnemonic = false,
                    Cursor = Cursors.Hand
                };
                label.Text = text;
                label.Click += click;
                host.Controls.Add(label);
                y = label.Bottom;
            }

            host.Height = y;
            host.ResumeLayout();
            layout();
        }

        private static bool Repeats(string line, Confirmation confirmation)
        {
            if (string.Equals(line, confirmation.Headline, StringComparison.OrdinalIgnoreCase)) return true;
            return confirmation.Summary != null && confirmation.Summary.Any(s => string.Equals(s, line, StringComparison.OrdinalIgnoreCase));
        }

        // The item parser was written against Steam's trade offer markup, so a trade page it cannot read is saved next to the log
        private static void KeepPage(Confirmation confirmation, string html)
        {
            try
            {
                string file = System.IO.Path.Combine(Manifest.GetExecutableDir(), "details-" + confirmation.ID + ".html");
                if (!System.IO.File.Exists(file))
                    System.IO.File.WriteAllText(file, html);
                Log.Write("Confirmation " + confirmation.ID + " is a trade but its page had no item lists, page saved as " + System.IO.Path.GetFileName(file));
            }
            catch (Exception ex)
            {
                Log.Error("Saving confirmation page", ex);
            }
        }

        private Label SmallLabel(Control host, string text, int y, EventHandler click)
        {
            var label = new Label()
            {
                Text = Language.T(text),
                ForeColor = Theme.TextMuted,
                Font = new Font("Segoe UI", 8.25F),
                Location = new Point(0, y),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            label.Click += click;
            host.Controls.Add(label);
            return label;
        }

        // One side of a trade: a caption, then a row per item with its icon, name and type
        private int ItemList(Panel host, string caption, List<TradeItem> items, int y, EventHandler click)
        {
            y = SmallLabel(host, caption, y, click).Bottom + LogicalToDeviceUnits(2);
            if (items.Count == 0)
            {
                var nothing = new Label()
                {
                    Text = Language.T("Nothing"),
                    ForeColor = Theme.TextMuted,
                    Location = new Point(0, y),
                    AutoSize = true,
                    Cursor = Cursors.Hand
                };
                nothing.Click += click;
                host.Controls.Add(nothing);
                return nothing.Bottom;
            }

            int iconSize = LogicalToDeviceUnits(36);
            int rowHeight = iconSize + LogicalToDeviceUnits(6);
            int textLeft = iconSize + LogicalToDeviceUnits(10);
            foreach (var item in items)
            {
                var iconBox = new PictureBox()
                {
                    Size = new Size(iconSize, iconSize),
                    Location = new Point(0, y),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    BackColor = Theme.Background
                };
                Theme.Rounded(iconBox);
                host.Controls.Add(iconBox);
                LoadIcon(iconBox, item.IconUrl);

                var name = new Label()
                {
                    Text = item.Amount > 1 ? item.Amount + " x " + Language.T("Item") : Language.T("Item"),
                    ForeColor = Theme.Text,
                    AutoEllipsis = true,
                    UseMnemonic = false,
                    Location = new Point(textLeft, y + LogicalToDeviceUnits(1)),
                    Size = new Size(host.Width - textLeft, LogicalToDeviceUnits(18)),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                name.Click += click;
                host.Controls.Add(name);

                var kind = new Label()
                {
                    Text = "",
                    ForeColor = Theme.TextMuted,
                    Font = new Font("Segoe UI", 8.25F),
                    AutoEllipsis = true,
                    UseMnemonic = false,
                    Location = new Point(textLeft, name.Bottom),
                    Size = new Size(host.Width - textLeft, LogicalToDeviceUnits(16)),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                kind.Click += click;
                host.Controls.Add(kind);

                if (item.ClassId != null)
                    _ = NameItem(item, name, kind, iconBox);
                y += rowHeight;
            }
            return y - LogicalToDeviceUnits(6);
        }

        private static async Task NameItem(TradeItem item, Label name, Label kind, PictureBox iconBox)
        {
            var info = await Economy.GetAsync(item.AppId, item.ClassId, item.InstanceId);
            if (name.IsDisposed) return;
            if (info == null)
            {
                name.Text = (item.Amount > 1 ? item.Amount + " x " : "") + Language.T("Unknown item") + " " + item.ClassId;
                return;
            }
            name.Text = (item.Amount > 1 ? item.Amount + " x " : "") + info.Name;
            kind.Text = info.Type ?? "";
            if (iconBox.Image == null && item.IconUrl == null)
                LoadIcon(iconBox, info.IconUrl);
        }

        private static void LoadIcon(PictureBox box, string url)
        {
            if (string.IsNullOrEmpty(url)) return;
            try
            {
                box.LoadAsync(url);
            }
            catch (Exception ex)
            {
                Log.Error("Item icon " + url, ex);
            }
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

            bool ok = await Handle(steamAccount, new[] { button.Confirmation }, accept) == 0;

            // A refresh or an account switch may have thrown the card away while Steam was answering
            if (card.IsDisposed || card.Parent == null)
                return;

            if (!ok)
            {
                foreach (Control c in card.Controls)
                    if (c is Button) c.Enabled = true;
                string text = "Steam did not " + (accept ? "accept" : "cancel") + " this confirmation. It may already be handled, press Refresh to check.";
                if (accept && button.Confirmation.ConfType == Confirmation.EMobileConfirmationType.Trade)
                    text += "\n\n" + TradeProtectionHint;
                MessageForm.Show(text, "Confirmations", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var list = card.Parent;
            list.Controls.Remove(card);
            card.Dispose();
            if (list.Controls.Count == 0)
                ShowEmpty();
            else
                UpdateBatchButtons();
        }

        private void ShowEmpty()
        {
            Label emptyLabel = new Label() { Text = "Nothing to confirm", AutoSize = true, ForeColor = Theme.TextMuted, Location = new Point(16, 24) };
            this.splitContainer1.Panel2.Controls.Add(emptyLabel);
            UpdateBatchButtons();
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
            await Refresh(true);
        }

        private async void ConfirmationFormWeb_Shown(object sender, EventArgs e)
        {
            await Refresh(true);
        }

        private async Task Refresh(bool busy)
        {
            this.btnRefresh.Enabled = false;
            this.btnRefresh.Text = Language.T("Refreshing...");

            await this.LoadData();

            this.btnRefresh.Enabled = true;
            this.btnRefresh.Text = Language.T("Refresh");
        }
    }
}
