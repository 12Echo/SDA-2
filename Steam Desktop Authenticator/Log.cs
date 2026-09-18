using System;
using System.IO;

namespace Steam_Desktop_Authenticator
{
    // Plain text log next to the exe, rolled over at 1 MB with one older copy kept. Never holds secrets or codes.
    static class Log
    {
        private static readonly object gate = new object();

        public static string Path
        {
            get { return System.IO.Path.Combine(Manifest.GetExecutableDir(), "sda2.log"); }
        }

        public static void Write(string text)
        {
            try
            {
                lock (gate)
                {
                    string path = Path;
                    var info = new FileInfo(path);
                    if (info.Exists && info.Length > 1024 * 1024)
                    {
                        File.Delete(path + ".old");
                        File.Move(path, path + ".old");
                    }
                    File.AppendAllText(path, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + text + Environment.NewLine);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Error(string context, Exception ex)
        {
            string text = context + ": " + ex.GetType().Name + ": " + ex.Message;
            if (ex.InnerException != null)
                text += " (" + ex.InnerException.Message + ")";
            Write(text);
        }

        public static void Crash(Exception ex)
        {
            if (ex == null) return;
            Write("Unhandled " + ex.GetType().Name + ": " + ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}
