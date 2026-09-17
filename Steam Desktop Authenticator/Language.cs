using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Steam_Desktop_Authenticator
{
    // Translations are json files in the languages folder, keyed by the English text
    static class Language
    {
        private static Dictionary<string, string> strings = new Dictionary<string, string>();

        public static string Code { get; private set; } = "";

        public static string Folder
        {
            get { return Path.Combine(Manifest.GetExecutableDir(), "languages"); }
        }

        public static void Load(string code)
        {
            strings = new Dictionary<string, string>();
            Code = "";
            if (string.IsNullOrEmpty(code)) return;

            try
            {
                string file = Path.Combine(Folder, code + ".json");
                if (!File.Exists(file)) return;
                var loaded = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
                if (loaded == null) return;
                strings = loaded;
                Code = code;
            }
            catch (Exception)
            {
            }
        }

        // Every file in the folder, as code and the display name it declares
        public static List<KeyValuePair<string, string>> Available()
        {
            var list = new List<KeyValuePair<string, string>>();
            try
            {
                if (!Directory.Exists(Folder)) return list;
                foreach (string file in Directory.GetFiles(Folder, "*.json").OrderBy(f => f))
                {
                    string code = Path.GetFileNameWithoutExtension(file);
                    if (code.Equals("template", StringComparison.OrdinalIgnoreCase)) continue;
                    string name = code;
                    try
                    {
                        var loaded = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file));
                        if (loaded != null && loaded.TryGetValue("_name", out string declared) && !string.IsNullOrEmpty(declared))
                            name = declared;
                    }
                    catch (Exception)
                    {
                    }
                    list.Add(new KeyValuePair<string, string>(code, name));
                }
            }
            catch (Exception)
            {
            }
            return list;
        }

        public static string T(string text)
        {
            if (text == null) return null;
            string translated;
            if (strings.TryGetValue(text, out translated) && !string.IsNullOrEmpty(translated))
                return translated;
            return text;
        }

        public static void Apply(Form form)
        {
            if (strings.Count == 0) return;
            form.Text = T(form.Text);
            ApplyTo(form);
        }

        public static void Apply(ToolStripItemCollection items)
        {
            if (strings.Count == 0) return;
            foreach (ToolStripItem item in items)
            {
                item.Text = T(item.Text);
                if (item is ToolStripMenuItem menu && menu.HasDropDownItems)
                    Apply(menu.DropDownItems);
            }
        }

        private static void ApplyTo(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                switch (c)
                {
                    case LinkLabel link:
                        bool wholeLink = link.LinkArea.Start == 0 && link.LinkArea.Length == link.Text.Length;
                        link.Text = T(link.Text);
                        if (wholeLink) link.LinkArea = new LinkArea(0, link.Text.Length);
                        break;
                    case TextBox box:
                        box.PlaceholderText = T(box.PlaceholderText);
                        break;
                    case ToolStrip strip:
                        Apply(strip.Items);
                        break;
                    case Label _:
                    case ButtonBase _:
                    case GroupBox _:
                        c.Text = T(c.Text);
                        break;
                }
                if (c.HasChildren)
                    ApplyTo(c);
            }
        }
    }
}
