using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SteamAuth;

namespace Steam_Desktop_Authenticator
{
    public partial class ConfirmationPopup : Form
    {
        private class Pending
        {
            public string Label;
            public SteamGuardAccount Account;
            public Confirmation Confirmation;
        }

        private readonly Queue<Pending> queue = new Queue<Pending>();
        private Pending current;
        private bool busy;

        private bool dragging;
        private Point dragStart;
        private int homeLeft;

        public ConfirmationPopup()
        {
            InitializeComponent();
            Theme.Apply(this);
            Theme.Secondary(btnClose);
            btnClose.BackColor = Theme.Surface;

            foreach (Control c in new Control[] { this, lblAccount, lblCounter, lblHeadline, lblSummary })
            {
                c.MouseDown += swipe_MouseDown;
                c.MouseMove += swipe_MouseMove;
                c.MouseUp += swipe_MouseUp;
            }
        }

        // Click to dismiss, drag right to swipe away, right click to clear everything
        private void swipe_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnClose_Click(sender, e);
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            dragging = true;
            dragStart = Cursor.Position;
            homeLeft = Left;
        }

        private void swipe_MouseMove(object sender, MouseEventArgs e)
        {
            if (!dragging) return;
            int dx = Cursor.Position.X - dragStart.X;
            Left = homeLeft + Math.Max(0, dx);
        }

        private void swipe_MouseUp(object sender, MouseEventArgs e)
        {
            if (!dragging || e.Button != MouseButtons.Left) return;
            dragging = false;
            int dx = Cursor.Position.X - dragStart.X;
            Left = homeLeft;
            if (busy) return;

            bool swiped = dx > LogicalToDeviceUnits(60);
            bool clicked = Math.Abs(dx) < LogicalToDeviceUnits(4);
            if (swiped || clicked)
                ShowNext();
        }

        // Never take focus away from whatever the user is doing
        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x08000000 | 0x00000080; // WS_EX_NOACTIVATE, WS_EX_TOOLWINDOW
                return cp;
            }
        }

        public void Queue(string label, SteamGuardAccount account, IEnumerable<Confirmation> confirmations)
        {
            foreach (var confirmation in confirmations)
                queue.Enqueue(new Pending { Label = label, Account = account, Confirmation = confirmation });

            if (current == null)
                ShowNext();
            else
                UpdateCounter();
        }

        private void ShowNext()
        {
            if (queue.Count == 0)
            {
                current = null;
                Hide();
                return;
            }

            current = queue.Dequeue();
            lblAccount.Text = current.Label;
            lblHeadline.Text = current.Confirmation.Headline;
            lblSummary.Text = current.Confirmation.Summary == null ? "" : string.Join("  ·  ", current.Confirmation.Summary);
            btnAccept.Text = string.IsNullOrEmpty(current.Confirmation.Accept) ? "Accept" : current.Confirmation.Accept;
            btnDeny.Text = string.IsNullOrEmpty(current.Confirmation.Cancel) ? "Deny" : current.Confirmation.Cancel;
            SetBusy(false);
            UpdateCounter();

            var area = Screen.PrimaryScreen.WorkingArea;
            int margin = LogicalToDeviceUnits(16);
            Location = new Point(area.Right - Width - margin, area.Bottom - Height - margin);

            timerHide.Stop();
            timerHide.Start();
            if (!Visible)
                Show();
        }

        private void UpdateCounter()
        {
            lblCounter.Text = queue.Count > 0 ? "+" + queue.Count + " more" : "";
        }

        private void SetBusy(bool value)
        {
            busy = value;
            btnAccept.Enabled = btnDeny.Enabled = !value;
        }

        private async void btnAccept_Click(object sender, EventArgs e)
        {
            if (busy || current == null) return;
            SetBusy(true);
            bool ok = await current.Account.AcceptConfirmation(current.Confirmation);
            Finish(ok, "Steam did not accept it. Open the confirmations list to try again.");
        }

        private async void btnDeny_Click(object sender, EventArgs e)
        {
            if (busy || current == null) return;
            SetBusy(true);
            bool ok = await current.Account.DenyConfirmation(current.Confirmation);
            Finish(ok, "Steam did not deny it. Open the confirmations list to try again.");
        }

        private void Finish(bool ok, string failure)
        {
            if (ok)
            {
                ShowNext();
                return;
            }

            lblSummary.Text = failure;
            lblSummary.ForeColor = Theme.Warning;
            SetBusy(false);
            timerHide.Stop();
            timerHide.Start();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            queue.Clear();
            current = null;
            Hide();
        }

        private void timerHide_Tick(object sender, EventArgs e)
        {
            if (busy || ClientRectangle.Contains(PointToClient(Cursor.Position)))
                return;

            timerHide.Stop();
            ShowNext();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Region = new Region(Theme.RoundedRect(new Rectangle(0, 0, Width, Height), Theme.Radius + 2));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (var pen = new Pen(Theme.Border))
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}
