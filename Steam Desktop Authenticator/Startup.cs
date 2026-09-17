using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace Steam_Desktop_Authenticator
{
    static class Startup
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string ValueName = "Steam Desktop Authenticator 2";

        public static void Apply(Manifest manifest)
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(RunKey))
                {
                    if (manifest.StartWithWindows)
                        key.SetValue(ValueName, "\"" + Environment.ProcessPath + "\"" + (manifest.StartMinimized ? " -s" : ""));
                    else
                        key.DeleteValue(ValueName, false);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception)
            {
            }
        }
    }
}
