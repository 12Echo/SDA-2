using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    public class TimeoutBar : Control
    {
        private int maximum = 30;
        private int value = 30;

        public TimeoutBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            TabStop = false;
            Height = 4;
        }

        public int Maximum
        {
            get { return maximum; }
            set { maximum = Math.Max(1, value); Invalidate(); }
        }

        public int Value
        {
            get { return value; }
            set { this.value = Math.Max(0, Math.Min(maximum, value)); Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(Parent == null ? Theme.Surface : Parent.BackColor);

            int radius = Height / 2;
            using (var path = Theme.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), radius))
            using (var brush = new SolidBrush(Theme.Border))
                e.Graphics.FillPath(brush, path);

            int width = (int)(Width * (double)value / maximum);
            if (width < Height) return;
            using (var path = Theme.RoundedRect(new Rectangle(0, 0, width - 1, Height - 1), radius))
            using (var brush = new SolidBrush(value <= 5 ? Theme.Warning : Theme.Accent))
                e.Graphics.FillPath(brush, path);
        }
    }
}
