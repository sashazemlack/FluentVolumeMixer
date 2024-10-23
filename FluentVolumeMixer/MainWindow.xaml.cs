#region usings
using FluentVolumeMixer.Design;
using FluentVolumeMixer.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using static FluentVolumeMixer.Design.PInvoke.Methods;
using static FluentVolumeMixer.Design.PInvoke.ParameterTypes; 
#endregion


namespace FluentVolumeMixer
{

    public partial class MainWindow : Window
    {
        public static bool SuppressAppExit = false;
        
        public MainWindow()
        {
            InitializeComponent();
            SetWindowPosition();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(10);
            
            RefreshFrame();
            RefreshDarkMode();

            if (Environment.OSVersion.Version.Build < 22523)
            {
                if (!SettingsHelper.EnableWindows10Effect && this.AllowsTransparency == false)
                {
                    SuppressAppExit = true;
                    Hide();
                    Thread.Sleep(1);
                    Show();
                    SuppressAppExit = false;
                }
                else if (SettingsHelper.EnableWindows10Effect)
                {
                    ApplyAcrylicEffect();
                }
            }

            if (SettingsHelper.Windows11Effect > 1)
            {
                SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_SYSTEMBACKDROP_TYPE,
                SettingsHelper.Windows11Effect); // 2 - mica, 3 - acrylic, 4 - tabbed
            }

            inputDeviceMenu.ItemsSource = AudioDeviceManager.LoadDevices(false);
            outputDeviceMenu.ItemsSource = AudioDeviceManager.LoadDevices(true);
        }
        
        #region Design
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        private void ApplyAcrylicEffect()
        {

            WindowInteropHelper windowHelper = new WindowInteropHelper(this);
            IntPtr hwnd = windowHelper.Handle;

            AccentPolicy accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND,
                GradientColor = SettingsHelper.AppBackgroundLightTheme ? (0x01FFFFFF) : (0x01000000)
            };

            int accentStructSize = Marshal.SizeOf(accent);

            WindowCompositionAttributeData data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = Marshal.AllocHGlobal(accentStructSize)
            };

            Marshal.StructureToPtr(accent, data.Data, false);
            SetWindowCompositionAttribute(hwnd, ref data);

            Marshal.FreeHGlobal(data.Data);
        }

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,
            ACCENT_ENABLE_HOSTBACKDROP = 5
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }


        private void RefreshFrame()
        {
            IntPtr mainWindowPtr = new WindowInteropHelper(this).Handle;
            HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            MARGINS margins = new MARGINS();
            margins.cxLeftWidth = -1;
            margins.cxRightWidth = -1;
            margins.cyTopHeight = -1;
            margins.cyBottomHeight = -1;

            ExtendFrame(mainWindowSrc.Handle, margins);
        }

        private void RefreshDarkMode()
        {
            SetWindowAttribute(
                new WindowInteropHelper(this).Handle,
                DWMWINDOWATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE,
                SettingsHelper.AppBackgroundLightTheme ? 0 : 1); // 0 is light theme; 1 - dark

            
        }

        private void SetWindowPosition()
        {
            const int margin = 10;

            TaskbarHelper.RECT taskbarRect;
            TaskbarHelper.TaskbarPosition taskbarPosition = TaskbarHelper.GetTaskbarPosition(out taskbarRect);

            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
            System.Drawing.Rectangle workingArea = screen.WorkingArea;

            double windowLeft = 0;
            double windowTop = 0;

            switch (taskbarPosition)
            {
                case TaskbarHelper.TaskbarPosition.Bottom:
                    windowLeft = workingArea.Right - this.Width - margin;
                    windowTop = workingArea.Bottom - this.Height - margin;
                    break;

                case TaskbarHelper.TaskbarPosition.Top:
                    windowLeft = workingArea.Right - this.Width - margin;
                    windowTop = workingArea.Top + margin;
                    break;

                case TaskbarHelper.TaskbarPosition.Left:
                    windowLeft = workingArea.Left + margin;
                    windowTop = workingArea.Bottom - this.Height - margin;
                    break;

                case TaskbarHelper.TaskbarPosition.Right:
                    windowLeft = workingArea.Right - this.Width - margin;
                    windowTop = workingArea.Bottom - this.Height - margin;
                    break;
            }

            this.Left = windowLeft;
            this.Top = windowTop;
        }

        #endregion
        
        #region UI interactions
        private void BtnOutputDevices_Click(object sender, RoutedEventArgs e)
        {
            btnOutputDevices.ContextMenu.IsOpen = true;
        }

        private void BtnInputDevices_Click(object sender, RoutedEventArgs e)
        {
            btnInputDevices.ContextMenu.IsOpen = true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape) { Exit(); }
        }

        private void MoreButton_Click(object sender, RoutedEventArgs e)
        {
            SuppressAppExit = true;
            SettingsWindow mainWindow = new SettingsWindow();
            mainWindow.Show();
            SuppressAppExit = false;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Exit();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void BtnToggleMuteMic_Click(object sender, RoutedEventArgs e)
        {
            ProcessModel.InputMic.IsMuted = !ProcessModel.InputMic.IsMuted;
        }
        #endregion

        private void Exit(object s = null, object e = null)
        {
            if (!SuppressAppExit) { try { this.Close(); } catch { } }
        }
    }
}
