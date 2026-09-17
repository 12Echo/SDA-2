using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Steam_Desktop_Authenticator
{
    static class Theme
    {
        public static readonly Color Background = Color.FromArgb(24, 27, 33);
        public static readonly Color Surface = Color.FromArgb(34, 38, 46);
        public static readonly Color Control = Color.FromArgb(44, 49, 59);
        public static readonly Color ControlHover = Color.FromArgb(54, 60, 72);
        public static readonly Color Border = Color.FromArgb(52, 57, 68);
        public static readonly Color Text = Color.FromArgb(230, 233, 239);
        public static readonly Color TextMuted = Color.FromArgb(139, 147, 161);
        public static readonly Color Accent = Color.FromArgb(76, 155, 232);
        public static readonly Color AccentHover = Color.FromArgb(96, 170, 240);
        public static readonly Color AccentPressed = Color.FromArgb(60, 135, 210);
        public static readonly Color Success = Color.FromArgb(63, 185, 80);
        public static readonly Color Danger = Color.FromArgb(229, 72, 77);
        public static readonly Color Warning = Color.FromArgb(229, 165, 58);

        static readonly ToolStripRenderer menuRenderer = new MenuRenderer();

        public static void Apply(Form form)
        {
            Style(form);
            if (form.IsHandleCreated)
                DarkTitleBar(form.Handle);
            form.HandleCreated += (s, e) => DarkTitleBar(form.Handle);
        }

        public static void Apply(ToolStrip strip)
        {
            strip.Renderer = menuRenderer;
            strip.BackColor = Background;
            strip.ForeColor = Text;
            StyleMenuItems(strip.Items);
        }

        public static void Primary(Button b)
        {
            b.BackColor = Accent;
            b.ForeColor = Color.White;
            b.FlatAppearance.BorderColor = Accent;
            b.FlatAppearance.MouseOverBackColor = AccentHover;
            b.FlatAppearance.MouseDownBackColor = AccentPressed;
        }

        public static void Secondary(Button b)
        {
            b.BackColor = Control;
            b.ForeColor = Text;
            b.FlatAppearance.BorderColor = Control;
            b.FlatAppearance.MouseOverBackColor = ControlHover;
            b.FlatAppearance.MouseDownBackColor = Border;
        }

        static void Style(System.Windows.Forms.Control c)
        {
            MapColors(c);

            switch (c)
            {
                case Button b:
                    bool primary = b.BackColor == Accent;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    b.UseVisualStyleBackColor = false;
                    b.Cursor = Cursors.Hand;
                    if (primary) Primary(b); else Secondary(b);
                    break;
                case LinkLabel l:
                    l.LinkColor = TextMuted;
                    l.ActiveLinkColor = Accent;
                    l.VisitedLinkColor = TextMuted;
                    l.LinkBehavior = LinkBehavior.HoverUnderline;
                    break;
                case TextBoxBase t:
                    t.BorderStyle = BorderStyle.None;
                    StyleInputWrapper(t);
                    break;
                case UpDownBase u:
                    u.BorderStyle = BorderStyle.None;
                    StyleInputWrapper(u);
                    break;
                case ListBox lb:
                    StyleList(lb);
                    break;
                case CheckBox cb:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.FlatAppearance.BorderSize = 1;
                    cb.FlatAppearance.BorderColor = TextMuted;
                    cb.FlatAppearance.CheckedBackColor = Accent;
                    break;
                case ToolStrip ts:
                    Apply(ts);
                    break;
            }

            foreach (System.Windows.Forms.Control child in c.Controls)
                Style(child);
        }

        static void MapColors(System.Windows.Forms.Control c)
        {
            Color back = Map(c.BackColor);
            if (back != c.BackColor) c.BackColor = back;
            Color fore = Map(c.ForeColor);
            if (fore != c.ForeColor) c.ForeColor = fore;
        }

        static Color Map(Color color)
        {
            if (color == SystemColors.Control) return Background;
            if (color == SystemColors.Window) return Surface;
            if (color == SystemColors.ControlText || color == SystemColors.WindowText) return Text;
            if (color == SystemColors.GrayText || color == SystemColors.ControlDarkDark) return TextMuted;
            if (color == SystemColors.Highlight) return Accent;
            if (color == SystemColors.HighlightText) return Color.White;
            return color;
        }

        static void StyleInputWrapper(System.Windows.Forms.Control input)
        {
            if (!(input.Parent is Panel wrapper) || wrapper.Controls.Count != 1)
                return;

            wrapper.BackColor = Surface;
            wrapper.Paint += (s, e) =>
            {
                using (var pen = new Pen(input.Focused ? Accent : Border))
                    e.Graphics.DrawLine(pen, 0, wrapper.Height - 1, wrapper.Width, wrapper.Height - 1);
            };
            input.GotFocus += (s, e) => wrapper.Invalidate();
            input.LostFocus += (s, e) => wrapper.Invalidate();
        }

        static void StyleList(ListBox list)
        {
            list.BorderStyle = BorderStyle.None;
            list.DrawMode = DrawMode.OwnerDrawFixed;
            list.ItemHeight = list.Font.Height + 16;
            list.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool selected = (e.State & DrawItemState.Selected) != 0;
                using (var brush = new SolidBrush(selected ? Accent : list.BackColor))
                    e.Graphics.FillRectangle(brush, e.Bounds);
                var bounds = new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 12, e.Bounds.Height);
                TextRenderer.DrawText(e.Graphics, list.Items[e.Index].ToString(), list.Font, bounds,
                    selected ? Color.White : list.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            };
        }

        static void StyleMenuItems(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                item.ForeColor = Text;
                if (item is ToolStripMenuItem menu)
                    StyleMenuItems(menu.DropDownItems);
                if (item is ToolStripComboBox combo)
                {
                    combo.FlatStyle = FlatStyle.Flat;
                    combo.BackColor = Control;
                    combo.ForeColor = Text;
                }
            }
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        static void DarkTitleBar(IntPtr hwnd)
        {
            int on = 1;
            if (DwmSetWindowAttribute(hwnd, 20, ref on, sizeof(int)) != 0)
                DwmSetWindowAttribute(hwnd, 19, ref on, sizeof(int));

            // Windows 11 only, ignored elsewhere
            int caption = ColorTranslator.ToWin32(Background);
            DwmSetWindowAttribute(hwnd, 35, ref caption, sizeof(int));
            int text = ColorTranslator.ToWin32(Text);
            DwmSetWindowAttribute(hwnd, 36, ref text, sizeof(int));
        }

        class MenuRenderer : ToolStripProfessionalRenderer
        {
            public MenuRenderer() : base(new MenuColors())
            {
                RoundedEdges = false;
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = Text;
                base.OnRenderArrow(e);
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? Text : TextMuted;
                base.OnRenderItemText(e);
            }
        }

        class MenuColors : ProfessionalColorTable
        {
            public MenuColors()
            {
                UseSystemColors = false;
            }

            public override Color MenuStripGradientBegin => Background;
            public override Color MenuStripGradientEnd => Background;
            public override Color MenuItemSelected => ControlHover;
            public override Color MenuItemSelectedGradientBegin => ControlHover;
            public override Color MenuItemSelectedGradientEnd => ControlHover;
            public override Color MenuItemPressedGradientBegin => Surface;
            public override Color MenuItemPressedGradientMiddle => Surface;
            public override Color MenuItemPressedGradientEnd => Surface;
            public override Color MenuItemBorder => ControlHover;
            public override Color MenuBorder => Border;
            public override Color ToolStripDropDownBackground => Surface;
            public override Color ImageMarginGradientBegin => Surface;
            public override Color ImageMarginGradientMiddle => Surface;
            public override Color ImageMarginGradientEnd => Surface;
            public override Color SeparatorDark => Border;
            public override Color SeparatorLight => Border;
            public override Color ToolStripBorder => Background;
        }
    }
}
