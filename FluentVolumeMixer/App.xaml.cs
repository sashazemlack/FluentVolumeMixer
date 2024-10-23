using FluentVolumeMixer.Helpers;
using System.IO;
using System;
using System.Threading;
using System.Windows;

namespace FluentVolumeMixer
{
    public partial class App : Application
    {
        private static Mutex mutex = null;
        protected override void OnStartup(StartupEventArgs e)
        {
            const string mutexName = "ZemlackovsFluentAudioMixer";
            mutex = new Mutex(true, mutexName, out bool createdNew);

            if (!createdNew)
            {
                Shutdown();
                return;
            }

            //SetBrushColors();

            Thread systrayThread = new Thread(() =>
            {
                SystrayApplicationContext systrayContext = new SystrayApplicationContext();
                System.Windows.Forms.Application.Run(systrayContext);
            });

            systrayThread.SetApartmentState(ApartmentState.STA);
            systrayThread.Start();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            mutex?.ReleaseMutex();
            base.OnExit(e);
        }

        private void SetBrushColors()
        {
            try
            {
                Resources["AccentColorBrush"] = Design.Brushes.GetAccentColorBrush();
            } catch { }
        }

        #region Exception logging
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException(e.ExceptionObject as Exception);
        }

        private void LogException(Exception ex)
        {
            string executableDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string logFilePath = Path.Combine(executableDirectory, "ErrorLog.txt");

            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date: " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message: " + ex.Message);
                    writer.WriteLine("StackTrace: " + ex.StackTrace);
                    ex = ex.InnerException;
                }
            }
        }
        #endregion
    }
}
