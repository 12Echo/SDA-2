using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Steam_Desktop_Authenticator
{
    class Release
    {
        public Version Version;
        public string Name;
        public string PageUrl;
        public string ZipUrl;
    }

    // Fetches the latest GitHub release and swaps the running install for it
    static class Updater
    {
        internal static string LatestUrl = "https://api.github.com/repos/12Echo/SDA-2/releases/latest";
        private static readonly HttpClient http = new HttpClient();

        static Updater()
        {
            http.DefaultRequestHeaders.UserAgent.ParseAdd("Steam Desktop Authenticator 2");
            http.Timeout = TimeSpan.FromMinutes(5);
        }

        public static Version Current
        {
            get { return new Version(Application.ProductVersion.Split('+')[0]); }
        }

        public static async Task<Release> CheckAsync()
        {
            string body = await http.GetStringAsync(LatestUrl);
            var json = JObject.Parse(body);

            string tag = json.Value<string>("tag_name") ?? "";
            var release = new Release
            {
                Version = new Version(tag.TrimStart('v', 'V')),
                Name = json.Value<string>("name"),
                PageUrl = json.Value<string>("html_url")
            };

            var assets = json["assets"] as JArray;
            if (assets != null)
            {
                var zip = assets.FirstOrDefault(a => (a.Value<string>("name") ?? "").EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
                release.ZipUrl = zip?.Value<string>("browser_download_url");
            }
            return release;
        }

        public static async Task<string> DownloadAsync(Release release, IProgress<string> status)
        {
            string work = Path.Combine(Path.GetTempPath(), "sda2-update");
            if (Directory.Exists(work)) Directory.Delete(work, true);
            Directory.CreateDirectory(work);

            string zip = Path.Combine(work, "update.zip");
            status?.Report("Downloading " + release.Version + "...");
            using (var response = await http.GetAsync(release.ZipUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();
                long? total = response.Content.Headers.ContentLength;
                using (var input = await response.Content.ReadAsStreamAsync())
                using (var output = File.Create(zip))
                {
                    byte[] buffer = new byte[81920];
                    long done = 0;
                    long percent = -1;
                    int read;
                    while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, read);
                        done += read;
                        if (total > 0 && done * 100 / total.Value != percent)
                        {
                            percent = done * 100 / total.Value;
                            status?.Report("Downloading " + release.Version + "... " + percent + "%");
                        }
                    }
                }
            }

            status?.Report("Unpacking...");
            string files = Path.Combine(work, "files");
            ZipFile.ExtractToDirectory(zip, files);
            return Flatten(files);
        }

        // Release zips may wrap everything in one folder, the install files are wherever the exe is
        private static string Flatten(string folder)
        {
            string exe = Path.GetFileName(Environment.ProcessPath);
            for (int depth = 0; depth < 3; depth++)
            {
                if (File.Exists(Path.Combine(folder, exe))) return folder;
                var dirs = Directory.GetDirectories(folder);
                if (dirs.Length != 1 || Directory.GetFiles(folder).Length > 0) break;
                folder = dirs[0];
            }
            throw new InvalidDataException("The update package does not contain " + exe + ".");
        }

        // Copies the new files over once this process has exited, then starts the new version
        public static void Apply(string source)
        {
            string target = Manifest.GetExecutableDir();
            string script = Path.Combine(Path.GetTempPath(), "sda2-update", "apply.cmd");
            int pid = Process.GetCurrentProcess().Id;

            File.WriteAllText(script, string.Join("\r\n", new[]
            {
                "@echo off",
                ":wait",
                "tasklist /fi \"PID eq " + pid + "\" 2>nul | find \"" + pid + "\" >nul",
                "if not errorlevel 1 (",
                "    timeout /t 1 /nobreak >nul",
                "    goto wait",
                ")",
                "robocopy \"" + source + "\" \"" + target + "\" /e /xd maFiles /r:10 /w:1 >nul",
                "start \"\" \"" + Path.Combine(target, Path.GetFileName(Environment.ProcessPath)) + "\"",
                "rmdir /s /q \"" + Path.GetDirectoryName(source) + "\"",
                ""
            }));

            Process.Start(new ProcessStartInfo("cmd.exe", "/c \"" + script + "\"")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetTempPath()
            });
        }
    }
}
