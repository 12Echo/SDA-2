using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CommandLine;

namespace Steam_Desktop_Authenticator
{
    static class Program
    {
        // Posted to every top level window when a second copy starts, so the running one can come to the front
        public static readonly int ShowMessage = RegisterWindowMessage("SteamDesktopAuthenticator2.Show");

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int RegisterWindowMessage(string name);

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        public static Process PriorProcess()
        // Returns a System.Diagnostics.Process pointing to
        // a pre-existing process with the same name as the
        // current one, if any; or null if the current process
        // is unique.
        {
            try
            {
                Process curr = Process.GetCurrentProcess();
                Process[] procs = Process.GetProcessesByName(curr.ProcessName);
                foreach (Process p in procs)
                {
                    if ((p.Id != curr.Id) &&
                        (p.MainModule.FileName == curr.MainModule.FileName))
                        return p;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void Crash(Exception ex)
        {
            Log.Crash(ex);
            MessageForm.Show("Something went wrong and the details were written to sda2.log next to SDA. Please attach it when reporting this.\n\n" + ex.Message, "Steam Desktop Authenticator 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // run the program only once, a second start just brings the first one up
            if (PriorProcess() != null)
            {
                PostMessage((IntPtr)0xffff, ShowMessage, IntPtr.Zero, IntPtr.Zero);
                return;
            }

            // Parse command line arguments
            CommandLineOptions options = new();
            Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed(o => options = o);

            ApplicationConfiguration.Initialize();
            Log.Write("Steam Desktop Authenticator 2 " + Application.ProductVersion.Split('+')[0] + " started");
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => Crash(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.Crash(e.ExceptionObject as Exception);
            System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, e) => Log.Crash(e.Exception);

            if (Manifest.HasBackup())
            {
                var restore = MessageForm.Show("An encryption change did not finish the last time SDA ran, so a backup of your maFiles was kept.\nRestore the backup? Choose No to keep the files as they are now.", "Steam Desktop Authenticator 2", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                try
                {
                    if (restore == DialogResult.Yes)
                        Manifest.RestoreBackup();
                    else
                        Manifest.DiscardBackup();
                }
                catch (Exception ex)
                {
                    MessageForm.Show("Could not restore the backup: " + ex.Message + "\nThe backup is in the maFiles.backup folder next to SDA.", "Steam Desktop Authenticator 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Manifest man;

            try
            {
                man = Manifest.GetManifest();
            }
            catch (ManifestParseException)
            {
                // Manifest file was corrupted, generate a new one.
                try
                {
                    MessageForm.Show("Your settings were unexpectedly corrupted and were reset to defaults.", "Steam Desktop Authenticator 2", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    man = Manifest.GenerateNewManifest(true);
                }
                catch (MaFileEncryptedException)
                {
                    // An maFile was encrypted, we're fucked.
                    MessageForm.Show("Sorry, but SDA was unable to recover your accounts since you used encryption.\nYou'll need to recover your Steam accounts by removing the authenticator.\nClick OK to view instructions.", "Steam Desktop Authenticator 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Startup.OpenUrl("https://github.com/Jessecar96/SteamDesktopAuthenticator/wiki/Help!-I'm-locked-out-of-my-account");
                    return;
                }
            }

            Language.Load(man.Language);

            if (man.FirstRun)
            {
                if (man.Entries.Count > 0)
                {
                    // Already has accounts, just run
                    MainForm mf = new MainForm();
                    mf.SetEncryptionKey(options.EncryptionKey);
                    mf.StartSilent(options.Silent || man.StartMinimized);
                    Application.Run(mf);
                }
                else
                {
                    // No accounts, run welcome form
                    Application.Run(new WelcomeForm());
                }
            }
            else
            {
                MainForm mf = new MainForm();
                mf.SetEncryptionKey(options.EncryptionKey);
                mf.StartSilent(options.Silent || man.StartMinimized);
                Application.Run(mf);
            }
        }
    }
}
