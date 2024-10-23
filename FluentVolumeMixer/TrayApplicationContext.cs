using FluentVolumeMixer.Helpers;
using System;
using System.Threading;
using System.Windows.Forms;

namespace FluentVolumeMixer
{
    public class SystrayApplicationContext : ApplicationContext
    {
        private NotifyIcon notifyIcon = null;

        public SystrayApplicationContext()
        {
            MenuItem openMenuItem = new MenuItem("Open GUI", new EventHandler(OpenGUI));
            MenuItem StartActivity = new MenuItem("Settings", new EventHandler(OpenSettingsForm));

            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(ExitApp));
            openMenuItem.DefaultItem = true;

            notifyIcon = new NotifyIcon();
            notifyIcon.MouseClick += NotifyIcon_MouseClick;
            notifyIcon.Icon = SettingsHelper.TaskbarWhiteIcon ? Properties.Resources.TrayIconLight : Properties.Resources.TrayIconDark;
            MenuItem[] menuItems = [openMenuItem, StartActivity, new MenuItem("-"), exitMenuItem];
            notifyIcon.ContextMenu = new ContextMenu(menuItems);
            notifyIcon.Visible = true;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                OpenGUI(sender, e);
            }
        }

        private void OpenGUI(object sender, EventArgs e)
        {
            Thread mainWindowThread;
            mainWindowThread = new Thread(() =>
            {
                MainWindow mainWindow = new MainWindow { AllowsTransparency = SettingsHelper.EnableWindows10Effect };
                mainWindow.Show();
                System.Windows.Threading.Dispatcher.Run();
            });

            mainWindowThread.SetApartmentState(ApartmentState.STA);
            mainWindowThread.Start();
        }

        private void OpenSettingsForm(object sender, EventArgs e)
        {
            Thread settingsWindowThread;
            settingsWindowThread = new Thread(() =>
            {
                SettingsWindow mainWindow = new SettingsWindow();
                mainWindow.Show();
                System.Windows.Threading.Dispatcher.Run();
            });

            settingsWindowThread.SetApartmentState(ApartmentState.STA);
            settingsWindowThread.Start();
        }

        private void ExitApp(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Environment.Exit(0);
        }
    }
}
