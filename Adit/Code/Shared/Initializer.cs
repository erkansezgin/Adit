﻿using Adit.Models;
using Adit.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Adit.Code.Shared
{
    public static class Initializer
    {
        public static void ProcessCommandLineArgs(string[] args)
        {
            if (args.Contains("-background"))
            {
                Config.Current.StartupMode = Config.StartupModes.Background;
            }
            else if (args.Contains("-notifier"))
            {
                Config.Current.StartupMode = Config.StartupModes.Notifier;
            }
            else if (args.Contains("-install"))
            {
                ServiceConfig.InstallService(true);
                App.Current.Shutdown();
            }
            else if (args.Contains("-uninstall"))
            {
                ServiceConfig.RemoveService(true);
                App.Current.Shutdown();
            }
            else if (args.Contains("-upgrade"))
            {
                // TODO
            }
        }

        public static void CleanupTempFiles()
        {
            // Clean up temp files from previous file transfers.
            var di = new DirectoryInfo(Path.GetTempPath() + @"\Adit");
            if (di.Exists)
            {
                di.Delete(true);
            }
        }
        public static void SetGlobalErrorHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            App.Current.DispatcherUnhandledException += DispatcherUnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Utilities.WriteToLog(e.ExceptionObject as Exception);
            Utilities.DisplayErrorMessage();
        }

        private static void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Utilities.WriteToLog(e.Exception);
            Utilities.DisplayErrorMessage();
        }
        public static void SetShutdownMode()
        {
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            App.Current.Exit += (send, arg) =>
            {
                Config.Save();
                if (TrayIcon.Icon?.IsDisposed == false)
                {
                    TrayIcon.Icon.Dispose();
                }
            };
        }
    }
}
