using System;
using System.Drawing;
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
            e.Graphics.Clear(Theme.Border);
            int width = (int)(Width * (double)value / maximum);
            using (var brush = new SolidBrush(value <= 5 ? Theme.Warning : Theme.Accent))
                e.Graphics.FillRectangle(brush, 0, 0, width, Height);
        }
    }
}
