using AudioSwitcher.AudioApi.CoreAudio;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace FluentVolumeMixer
{
    public static class AudioDeviceManager
    {
        private static CoreAudioController controller = new CoreAudioController();

        private static TextBlock GetIconByDeviceType(string Icon)
        {
            TextBlock Tb = new()
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Foreground = Helpers.SettingsHelper.AppTextsLightTheme ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.White),
                TextAlignment = TextAlignment.Center,
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Text = Icon switch
                {
                    "%windir%\\system32\\mmres.dll,-3010" => "\ue7f5",
                    "%windir%\\system32\\mmres.dll,-3011" => "\ue7f6",
                    "%windir%\\system32\\mmres.dll,-3012" => "\ue95f",
                    "%windir%\\system32\\mmres.dll,-3013" => "\ue967",
                    "%windir%\\system32\\mmres.dll,-3014" => "\ue720",
                    "%windir%\\system32\\mmres.dll,-3015" => "\ue95b",
                    "%windir%\\system32\\mmres.dll,-3016" => "\ue717",
                    "%windir%\\system32\\mmres.dll,-3017" => "\ue7f4",
                    "%windir%\\system32\\mmres.dll,-3018" => "\uf211",
                    "%windir%\\system32\\mmres.dll,-3019" => "\ue7f5",
                    "%windir%\\system32\\mmres.dll,-3020" => "\ue960",
                    "%windir%\\system32\\mmres.dll,-3021" => "\ue720",
                    _ => "\ue897"
                }
            };
            
            return Tb;
        }
        
        public static List<MenuItem> LoadDevices(bool isInput)
        {
            IEnumerable<CoreAudioDevice> devices = isInput ? controller.GetPlaybackDevices(AudioSwitcher.AudioApi.DeviceState.Active) : controller.GetCaptureDevices(AudioSwitcher.AudioApi.DeviceState.Active);
            List<MenuItem> MenuItems = new List<MenuItem>();
            
            foreach (CoreAudioDevice device in devices)
            {
                MenuItem menuItem = new MenuItem
                {
                    Header = device.FullName,
                    Icon = GetIconByDeviceType(device.IconPath),
                    Tag = device.Id
                };
                menuItem.Click += DeviceMenuItem_Click;
                MenuItems.Add(menuItem);
            }
            return MenuItems;
        }

        private static void DeviceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem selectedItem)
            {
                if (selectedItem.Tag is Guid selectedDevice)
                {
                    SetDefaultDevice(selectedDevice);
                }
            }
        }

        public static void SetDefaultDevice(Guid deviceId)
        {
            CoreAudioDevice Device = controller.GetDevice(deviceId);
            if (Device != null)
            {
                if (Device.IsCaptureDevice)
                {
                    controller.DefaultCaptureDevice = Device;
                    controller.DefaultCaptureCommunicationsDevice = Device;
                }
                else
                {
                    controller.DefaultPlaybackDevice = Device;
                    controller.DefaultPlaybackCommunicationsDevice = Device;
                }
            }
        }

    }
}
