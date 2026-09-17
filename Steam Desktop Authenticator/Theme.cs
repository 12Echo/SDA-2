using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
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
        public static readonly Color TextDisabled = Color.FromArgb(88, 95, 108);
        public static readonly Color Accent = Color.FromArgb(76, 155, 232);
        public static readonly Color AccentHover = Color.FromArgb(96, 170, 240);
        public static readonly Color AccentPressed = Color.FromArgb(60, 135, 210);
        public static readonly Color Danger = Color.FromArgb(229, 72, 77);
        public static readonly Color Warning = Color.FromArgb(229, 165, 58);

        public const int Radius = 6;

        static readonly ToolStripRenderer menuRenderer = new MenuRenderer();
        static readonly ConditionalWeakTable<Button, ButtonState> buttons = new ConditionalWeakTable<Button, ButtonState>();
        static readonly ConditionalWeakTable<ListBox, Func<object, Image>> listImages = new ConditionalWeakTable<ListBox, Func<object, Image>>();
        static readonly ConditionalWeakTable<ListBox, Func<object, Color?>> listBadges = new ConditionalWeakTable<ListBox, Func<object, Color?>>();
        static readonly ConditionalWeakTable<ListBox, int[]> listDropMarkers = new ConditionalWeakTable<ListBox, int[]>();
        static readonly ConditionalWeakTable<ButtonBase, bool[]> toggleHover = new ConditionalWeakTable<ButtonBase, bool[]>();

        public static void ListImages(ListBox list, Func<object, Image> imageFor)
        {
            listImages.AddOrUpdate(list, imageFor);
            list.ItemHeight = list.Font.Height + 22;
        }

        public static void ListBadges(ListBox list, Func<object, Color?> badgeFor)
        {
            listBadges.AddOrUpdate(list, badgeFor);
        }

        // Insertion line shown while an item is being dragged, -1 hides it
        public static void ListDropMarker(ListBox list, int index)
        {
            int[] marker;
            if (!listDropMarkers.TryGetValue(list, out marker))
            {
                marker = new int[] { -1 };
                listDropMarkers.Add(list, marker);
            }
            if (marker[0] == index) return;
            marker[0] = index;
            list.Invalidate();
        }

        class ButtonState
        {
            public bool Hover;
            public bool Down;
            public bool Dropdown;
        }

        public static void Apply(Form form)
        {
            Style(form);
            if (form.IsHandleCreated)
                StyleWindow(form.Handle);
            form.HandleCreated += (s, e) => StyleWindow(form.Handle);
        }

        static void StyleWindow(IntPtr hwnd)
        {
            DarkTitleBar(hwnd);
            // Buttons are painted here, so the system focus rectangle only gets in the way
            SendMessage(hwnd, 0x0128, (IntPtr)0x00010001, IntPtr.Zero); // WM_UPDATEUISTATE, UIS_SET | UISF_HIDEFOCUS
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

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
            StyleButton(b);
            b.BackColor = Accent;
            b.ForeColor = Color.White;
            b.FlatAppearance.MouseOverBackColor = AccentHover;
            b.FlatAppearance.MouseDownBackColor = AccentPressed;
            b.Invalidate();
        }

        public static void Secondary(Button b)
        {
            StyleButton(b);
            b.BackColor = Control;
            b.ForeColor = Text;
            b.FlatAppearance.MouseOverBackColor = ControlHover;
            b.FlatAppearance.MouseDownBackColor = Border;
            b.Invalidate();
        }

        // A button that looks like an input and opens a menu, used instead of ComboBox which cannot be themed
        public static void Dropdown(Button b)
        {
            Secondary(b);
            ButtonState state;
            buttons.TryGetValue(b, out state);
            state.Dropdown = true;
            b.BackColor = Surface;
            b.FlatAppearance.MouseOverBackColor = Control;
            b.FlatAppearance.MouseDownBackColor = Control;
            b.Invalidate();
        }

        public static void Card(Panel panel, int bottomGap = 0)
        {
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                using (var brush = new SolidBrush(ParentColor(panel)))
                    e.Graphics.FillRectangle(brush, panel.ClientRectangle);
                var bounds = new Rectangle(0, 0, panel.Width, panel.Height - bottomGap);
                using (var path = RoundedRect(bounds, Radius))
                using (var brush = new SolidBrush(panel.BackColor))
                    e.Graphics.FillPath(brush, path);
            };
        }

        static void Style(System.Windows.Forms.Control c)
        {
            MapColors(c);

            switch (c)
            {
                case Button b:
                    if (b.BackColor == Accent) Primary(b); else Secondary(b);
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
                    StyleToggle(cb, () => cb.Checked, false);
                    break;
                case RadioButton rb:
                    StyleToggle(rb, () => rb.Checked, true);
                    break;
                case ComboBox combo:
                    StyleCombo(combo);
                    StyleInputWrapper(combo);
                    break;
                case ToolStrip ts:
                    Apply(ts);
                    break;
                case SplitContainer split:
                    DarkScrollbars(split.Panel1);
                    DarkScrollbars(split.Panel2);
                    break;
                case TableLayoutPanel _:
                    break;
                case Panel p:
                    if (p.BackColor == Surface && !IsInputWrapper(p))
                        Card(p);
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

        static Color ParentColor(System.Windows.Forms.Control c)
        {
            return c.Parent == null ? Background : c.Parent.BackColor;
        }

        static void StyleButton(Button b)
        {
            ButtonState state;
            if (buttons.TryGetValue(b, out state)) return;
            state = new ButtonState();
            buttons.Add(b, state);

            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.UseVisualStyleBackColor = false;
            b.Cursor = Cursors.Hand;
            b.MouseEnter += (s, e) => { state.Hover = true; b.Invalidate(); };
            b.MouseLeave += (s, e) => { state.Hover = false; state.Down = false; b.Invalidate(); };
            b.MouseDown += (s, e) => { state.Down = true; b.Invalidate(); };
            b.MouseUp += (s, e) => { state.Down = false; b.Invalidate(); };
            b.Paint += (s, e) => PaintButton(b, state, e.Graphics);
        }

        static void PaintButton(Button b, ButtonState state, Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            using (var brush = new SolidBrush(ParentColor(b)))
                g.FillRectangle(brush, b.ClientRectangle);

            Color fill = b.BackColor;
            if (!b.Enabled) fill = Control;
            else if (state.Down) fill = b.FlatAppearance.MouseDownBackColor;
            else if (state.Hover) fill = b.FlatAppearance.MouseOverBackColor;

            // Fills cover whole pixels, strokes sit on pixel centers, otherwise the edge row blends and looks like a frame
            using (var path = RoundedRect(new Rectangle(0, 0, b.Width, b.Height), Radius))
            using (var brush = new SolidBrush(fill))
                g.FillPath(brush, path);
            if (state.Dropdown)
                using (var path = RoundedRect(new Rectangle(0, 0, b.Width - 1, b.Height - 1), Radius))
                using (var pen = new Pen(b.Focused ? Accent : Border))
                    g.DrawPath(pen, path);

            Color textColor = b.Enabled ? b.ForeColor : TextMuted;
            if (state.Dropdown)
            {
                int pad = b.LogicalToDeviceUnits(12);
                var text = new Rectangle(pad, 0, b.Width - pad * 2 - b.LogicalToDeviceUnits(16), b.Height);
                TextRenderer.DrawText(g, b.Text, b.Font, text, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                int cx = b.Width - pad - b.LogicalToDeviceUnits(4);
                int cy = b.Height / 2 - 1;
                using (var pen = new Pen(textColor, 1.5f))
                {
                    g.DrawLine(pen, cx - 4, cy - 2, cx, cy + 2);
                    g.DrawLine(pen, cx, cy + 2, cx + 4, cy - 2);
                }
                return;
            }

            TextRenderer.DrawText(g, b.Text, b.Font, b.ClientRectangle, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
        }

        // A panel holding one input, plus any small buttons that sit inside the same box
        static bool IsInputWrapper(Panel panel)
        {
            int inputs = 0;
            foreach (System.Windows.Forms.Control c in panel.Controls)
            {
                if (c is TextBoxBase || c is UpDownBase || c is ComboBox) inputs++;
                else if (!(c is Button)) return false;
            }
            return inputs == 1;
        }

        // Check boxes and radios get the same treatment as buttons: painted here, so they match the rest
        static void StyleToggle(ButtonBase toggle, Func<bool> isChecked, bool round)
        {
            bool[] hover;
            if (toggleHover.TryGetValue(toggle, out hover)) return;
            hover = new bool[1];
            toggleHover.Add(toggle, hover);

            toggle.FlatStyle = FlatStyle.Flat;
            toggle.FlatAppearance.BorderSize = 0;
            toggle.Cursor = Cursors.Hand;
            // The painted glyph is a little wider than the system one, so give the auto size room for it
            toggle.Padding = new Padding(0, 0, 12, 0);
            toggle.MouseEnter += (s, e) => { hover[0] = true; toggle.Invalidate(); };
            toggle.MouseLeave += (s, e) => { hover[0] = false; toggle.Invalidate(); };
            toggle.Paint += (s, e) => PaintToggle(toggle, e.Graphics, isChecked(), round, hover[0]);
        }

        static void PaintToggle(ButtonBase toggle, Graphics g, bool isChecked, bool round, bool hover)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            using (var brush = new SolidBrush(ParentColor(toggle)))
                g.FillRectangle(brush, toggle.ClientRectangle);

            int size = toggle.LogicalToDeviceUnits(16);
            int gap = toggle.LogicalToDeviceUnits(8);
            var box = new Rectangle(0, (toggle.Height - size) / 2, size, size);

            // Disabled toggles keep their value but drop to the border tone so they read as off limits
            Color fill = isChecked ? (toggle.Enabled ? (hover ? AccentHover : Accent) : Border) : (hover && toggle.Enabled ? Control : Color.Transparent);
            Color edge = isChecked ? fill : (!toggle.Enabled ? Border : hover ? Text : TextMuted);
            Color mark = toggle.Enabled ? Color.White : TextDisabled;

            using (var brush = new SolidBrush(fill))
            using (var pen = new Pen(edge, 1.5f))
            {
                if (round)
                {
                    if (fill != Color.Transparent) g.FillEllipse(brush, box);
                    g.DrawEllipse(pen, box.X + 1, box.Y + 1, box.Width - 2, box.Height - 2);
                    if (isChecked)
                    {
                        int dot = toggle.LogicalToDeviceUnits(6);
                        using (var white = new SolidBrush(mark))
                            g.FillEllipse(white, box.X + (size - dot) / 2, box.Y + (size - dot) / 2, dot, dot);
                    }
                }
                else
                {
                    using (var path = RoundedRect(box, toggle.LogicalToDeviceUnits(4)))
                    using (var inner = RoundedRect(new Rectangle(box.X + 1, box.Y + 1, box.Width - 2, box.Height - 2), toggle.LogicalToDeviceUnits(3)))
                    {
                        if (fill != Color.Transparent) g.FillPath(brush, path);
                        g.DrawPath(pen, inner);
                    }
                    if (isChecked)
                    {
                        using (var white = new Pen(mark, 2f))
                        {
                            white.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                            white.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                            white.LineJoin = LineJoin.Round;
                            float x = box.X, y = box.Y, u = size / 16f;
                            g.DrawLines(white, new PointF[] { new PointF(x + 4 * u, y + 8.5f * u), new PointF(x + 7 * u, y + 11.5f * u), new PointF(x + 12 * u, y + 5 * u) });
                        }
                    }
                }
            }

            var text = new Rectangle(size + gap, 0, toggle.Width - size - gap, toggle.Height);
            TextRenderer.DrawText(g, toggle.Text, toggle.Font, text, toggle.Enabled ? toggle.ForeColor : TextDisabled,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        static void StyleInputWrapper(System.Windows.Forms.Control input)
        {
            if (!(input.Parent is Panel wrapper) || !IsInputWrapper(wrapper))
                return;

            wrapper.BackColor = Surface;
            wrapper.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                using (var brush = new SolidBrush(ParentColor(wrapper)))
                    e.Graphics.FillRectangle(brush, wrapper.ClientRectangle);
                using (var path = RoundedRect(new Rectangle(0, 0, wrapper.Width, wrapper.Height), Radius))
                using (var brush = new SolidBrush(Surface))
                    e.Graphics.FillPath(brush, path);
                using (var path = RoundedRect(new Rectangle(0, 0, wrapper.Width - 1, wrapper.Height - 1), Radius))
                using (var pen = new Pen(input.Focused ? Accent : input.Enabled ? Border : Control))
                    e.Graphics.DrawPath(pen, path);
            };
            input.GotFocus += (s, e) => wrapper.Invalidate();
            input.LostFocus += (s, e) => wrapper.Invalidate();
            input.EnabledChanged += (s, e) =>
            {
                input.ForeColor = input.Enabled ? Text : TextDisabled;
                wrapper.Invalidate();
            };
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
                using (var brush = new SolidBrush(list.BackColor))
                    e.Graphics.FillRectangle(brush, e.Bounds);
                if (selected)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    var bounds = new Rectangle(e.Bounds.X + 4, e.Bounds.Y + 2, e.Bounds.Width - 9, e.Bounds.Height - 5);
                    using (var path = RoundedRect(bounds, Radius - 2))
                    using (var brush = new SolidBrush(Accent))
                        e.Graphics.FillPath(brush, path);
                }
                int left = e.Bounds.X + 14;
                int right = e.Bounds.Right - 14;

                Func<object, Image> imageFor;
                if (listImages.TryGetValue(list, out imageFor))
                {
                    int size = e.Bounds.Height - 10;
                    var box = new Rectangle(left, e.Bounds.Y + 5, size, size);
                    var image = imageFor(list.Items[e.Index]);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    using (var path = RoundedRect(box, Radius - 2))
                    {
                        if (image != null)
                        {
                            var clip = e.Graphics.Clip;
                            e.Graphics.SetClip(path);
                            e.Graphics.DrawImage(image, box);
                            e.Graphics.Clip = clip;
                        }
                        else
                        {
                            using (var brush = new SolidBrush(selected ? AccentHover : Control))
                                e.Graphics.FillPath(brush, path);
                        }
                    }
                    left += size + 10;
                }

                Func<object, Color?> badgeFor;
                Color? badge;
                if (listBadges.TryGetValue(list, out badgeFor) && (badge = badgeFor(list.Items[e.Index])) != null)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                    int d = 8;
                    using (var brush = new SolidBrush(badge.Value))
                        e.Graphics.FillEllipse(brush, right - d, e.Bounds.Y + (e.Bounds.Height - d) / 2, d, d);
                    right -= d + 8;
                }

                var text = new Rectangle(left, e.Bounds.Y, right - left, e.Bounds.Height);
                TextRenderer.DrawText(e.Graphics, list.Items[e.Index].ToString(), list.Font, text,
                    selected ? Color.White : list.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);

                int[] marker;
                if (listDropMarkers.TryGetValue(list, out marker) && marker[0] >= 0)
                {
                    int y = marker[0] == e.Index ? e.Bounds.Y : marker[0] == e.Index + 1 && e.Index == list.Items.Count - 1 ? e.Bounds.Bottom - 2 : -1;
                    if (y >= 0)
                        using (var brush = new SolidBrush(Accent))
                            e.Graphics.FillRectangle(brush, e.Bounds.X + 4, y, e.Bounds.Width - 9, 2);
                }
            };
            RoundRegion(list);
            list.Resize += (s, e) => RoundRegion(list);
            DarkScrollbars(list);
        }

        static void RoundRegion(System.Windows.Forms.Control c)
        {
            using (var path = RoundedRect(new Rectangle(0, 0, c.Width, c.Height), Radius))
                c.Region = new Region(path);
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
                item.Padding = topLevel ? new Padding(8, 5, 8, 5) : new Padding(14, 7, 28, 7);
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
            menu.Padding = new Padding(5, 6, 5, 6);
            menu.BackColor = Surface;
            menu.DropShadowEnabled = false;
            menu.HandleCreated += (s, e) => RoundCorners(menu.Handle);
            if (menu.IsHandleCreated)
                RoundCorners(menu.Handle);
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        static extern int SetWindowTheme(IntPtr hwnd, string appName, string idList);

        static bool roundedCorners = true;

        static void RoundCorners(IntPtr hwnd)
        {
            // Windows 11 rounds the popup and paints the border for us, older Windows gets a drawn border
            int corner = 3;
            roundedCorners = DwmSetWindowAttribute(hwnd, 33, ref corner, sizeof(int)) == 0;
            if (!roundedCorners) return;

            int border = ColorTranslator.ToWin32(Border);
            DwmSetWindowAttribute(hwnd, 34, ref border, sizeof(int));
        }

        static void DarkScrollbars(System.Windows.Forms.Control c)
        {
            if (c.IsHandleCreated)
                SetWindowTheme(c.Handle, "DarkMode_Explorer", null);
            c.HandleCreated += (s, e) => SetWindowTheme(c.Handle, "DarkMode_Explorer", null);
        }

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

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
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
                if (!(e.ToolStrip is ToolStripDropDown) || roundedCorners) return;
                var bounds = e.AffectedBounds;
                bounds.Width--;
                bounds.Height--;
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, bounds);
            }

            protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

                if (e.Item.Selected && e.Item.Enabled)
                {
                    var bounds = new Rectangle(0, 1, e.Item.Width, e.Item.Height - 2);
                    if (e.Item.Owner is ToolStripDropDown)
                    {
                        // Item bounds can run past the popup edge, so anchor the highlight to the popup itself
                        int left = ItemLeft(e.Item);
                        int right = ItemRight(e.Item);
                        bounds = new Rectangle(left, 1, right - left, e.Item.Height - 2);
                    }
                    using (var path = RoundedRect(bounds, 4))
                    using (var brush = new SolidBrush(ControlHover))
                        e.Graphics.FillPath(brush, path);
                }

                if (e.Item is ToolStripMenuItem menuItem && menuItem.Checked)
                {
                    var bar = new Rectangle(5, 8, 3, e.Item.Height - 17);
                    using (var path = RoundedRect(bar, 1))
                    using (var brush = new SolidBrush(Accent))
                        e.Graphics.FillPath(brush, path);
                }

                // Items can carry a countdown, seconds left out of 30, drawn as a thin line
                if (e.Item.Tag is int secondsLeft)
                {
                    int left = ItemLeft(e.Item) + 9;
                    int width = ItemRight(e.Item) - 9 - left;
                    int y = e.Item.Height - 5;
                    using (var brush = new SolidBrush(Border))
                        e.Graphics.FillRectangle(brush, left, y, width, 2);
                    using (var brush = new SolidBrush(secondsLeft <= 5 ? Warning : Accent))
                        e.Graphics.FillRectangle(brush, left, y, width * secondsLeft / 30, 2);
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
                    e.Graphics.DrawLine(pen, ItemLeft(e.Item) + 5, y, ItemRight(e.Item) - 5, y);
            }

            // Edges of the popup in the item's own coordinates, with a small inset
            private static int ItemLeft(ToolStripItem item)
            {
                return 5 - item.Bounds.Left;
            }

            private static int ItemRight(ToolStripItem item)
            {
                return item.Owner.ClientSize.Width - 5 - item.Bounds.Left;
            }

            protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                var r = e.ArrowRectangle;
                int x = r.Left + r.Width / 2 - 2;
                int y = r.Top + r.Height / 2;
                using (var pen = new Pen(e.Item.Enabled ? Text : TextMuted, 1.5f))
                {
                    e.Graphics.DrawLine(pen, x, y - 4, x + 4, y);
                    e.Graphics.DrawLine(pen, x + 4, y, x, y + 4);
                }
            }

            protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
            {
            }
        }
    }
}
