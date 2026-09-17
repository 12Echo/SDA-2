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
            if (strip is ToolStripDropDownMenu menu)
                StyleDropDown(menu);
            StyleMenuItems(strip.Items, strip is MenuStrip);
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
                case RadioButton rb:
                    rb.FlatStyle = FlatStyle.Flat;
                    rb.FlatAppearance.BorderSize = 1;
                    rb.FlatAppearance.BorderColor = TextMuted;
                    rb.FlatAppearance.CheckedBackColor = Accent;
                    break;
                case ComboBox combo:
                    StyleCombo(combo);
                    StyleInputWrapper(combo);
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

        static void StyleCombo(ComboBox combo)
        {
            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = Surface;
            combo.ForeColor = Text;
            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.ItemHeight = combo.Font.Height + 8;
            combo.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                bool highlight = (e.State & DrawItemState.Selected) != 0 && (e.State & DrawItemState.ComboBoxEdit) == 0;
                using (var brush = new SolidBrush(highlight ? Accent : Surface))
                    e.Graphics.FillRectangle(brush, e.Bounds);
                var bounds = new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 6, e.Bounds.Height);
                TextRenderer.DrawText(e.Graphics, combo.Items[e.Index].ToString(), combo.Font, bounds,
                    highlight ? Color.White : Text,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            };
        }

        public static void StyleMenuItems(ToolStripItemCollection items, bool topLevel)
        {
            foreach (ToolStripItem item in items)
            {
                item.ForeColor = Text;
                item.Padding = topLevel ? new Padding(6, 4, 6, 4) : new Padding(10, 5, 24, 5);
                if (item is ToolStripMenuItem menu && menu.HasDropDownItems)
                {
                    menu.DropDown.Renderer = menuRenderer;
                    if (menu.DropDown is ToolStripDropDownMenu dropDown)
                        StyleDropDown(dropDown);
                    StyleMenuItems(menu.DropDownItems, false);
                }
            }
        }

        static void StyleDropDown(ToolStripDropDownMenu menu)
        {
            menu.ShowImageMargin = false;
            menu.ShowCheckMargin = false;
            menu.Padding = new Padding(1, 6, 1, 6);
            menu.BackColor = Surface;
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

        class MenuRenderer : ToolStripRenderer
        {
            protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
            {
                using (var brush = new SolidBrush(e.ToolStrip is ToolStripDropDown ? Surface : Background))
                    e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }

            protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
            {
                if (!(e.ToolStrip is ToolStripDropDown)) return;
                var bounds = e.AffectedBounds;
                bounds.Width--;
                bounds.Height--;
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, bounds);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                if (e.Item.Selected && e.Item.Enabled)
                {
                    var bounds = new Rectangle(Point.Empty, e.Item.Size);
                    if (e.Item.Owner is ToolStripDropDown)
                        bounds.Inflate(-4, 0);
                    using (var brush = new SolidBrush(ControlHover))
                        e.Graphics.FillRectangle(brush, bounds);
                }

                if (e.Item is ToolStripMenuItem menuItem && menuItem.Checked)
                {
                    using (var brush = new SolidBrush(Accent))
                        e.Graphics.FillRectangle(brush, 4, 6, 3, e.Item.Height - 12);
                }
            }

            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Enabled ? Text : TextMuted;
                base.OnRenderItemText(e);
            }

            protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
            {
            }

            protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
            {
                int y = e.Item.Height / 2;
                using (var pen = new Pen(Border))
                    e.Graphics.DrawLine(pen, 8, y, e.Item.Width - 8, y);
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.ArrowColor = e.Item.Enabled ? Text : TextMuted;
                base.OnRenderArrow(e);
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
            }
        }
    }
}
