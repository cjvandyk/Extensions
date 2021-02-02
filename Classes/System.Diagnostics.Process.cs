#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060, IDE0079 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args, Remove unnecessary suppression)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>

using System;
using System.Linq;

namespace Extensions
{
    /// <summary>
    /// Extensions for the System.Diagnostics.Process class.
    /// </summary>
    public static class Process
    {
        /// <summary>
        /// Elevate the current app to admin level.
        /// The call to Process.Elevate() is made at the beginning of the
        /// application flow resulting in the app restarting in admin mode.
        /// </summary>
        /// <param name="proc">The current app process.</param>
        /// <param name="args">Array of arguments used to start the app.</param>
        /// <returns>Restarts current app in admin mode.</returns>
        public static bool Elevate(this System.Diagnostics.Process proc, string[] args)
        {
            if ((args.Count() != 2) ||
                (args[1] != "ELEVATED"))
            {
                string appName = proc.MainModule.FileName;// System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                Console.WriteLine($"Elevating priviledge to for app [{appName}]...");
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(appName);
                startInfo.Verb = "runas";
                startInfo.Arguments = "restart ELEVATED";
                System.Diagnostics.Process.Start(startInfo);
                Console.WriteLine("Elevation complete!");
                Environment.Exit(0);
            }
            return true;
        }
    }
}
