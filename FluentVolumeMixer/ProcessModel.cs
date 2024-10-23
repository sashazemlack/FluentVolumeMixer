using FluentVolumeMixer.Helpers;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace FluentVolumeMixer
{
    public class ProcessModel : INotifyPropertyChanged
    {
        #region Variables

        private static readonly MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
        private static readonly MMDevice defaultOutputDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        private static readonly MMDevice defaultInputDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
        public AudioSessionControl AudioSessionControl { get; set; }

        public static ProcessModel MasterSounds { get; private set; } = null;
        public static ProcessModel SysSounds { get; private set; } = null;
        public static ProcessModel InputMic { get; private set; } = null;

        public string DisplayName { get; set; }
        public PR_TYPE TYPE { get; set; }
        public ImageSource DisplayIconPath { get; set; }

        private bool isMuted { get; set; }
        public bool IsMuted
        {
            get => isMuted;
            set
            {
                if (isMuted != value)
                {
                    isMuted = value;
                    ChangeMute(isMuted);
                    OnPropertyChanged(nameof(IsMuted));
                }
            }
        }
        private byte volumeLevel;
        public byte VolumeLevel
        {
            get => volumeLevel;
            set
            {
                if (volumeLevel != value)
                {
                    volumeLevel = value;
                    ChangeVolume(volumeLevel);
                    OnPropertyChanged(nameof(VolumeLevel));
                }
            }
        }

        public enum PR_TYPE
        {
            Generic,
            MasterSounds,
            SysSounds,
            InputMic
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static void InitMasterSysSounds()
        {
            MasterSounds = new ProcessModel()
            {
                DisplayName = "Master Volume",
                TYPE = PR_TYPE.MasterSounds,
                VolumeLevel = (byte)(defaultOutputDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100),
                IsMuted = defaultOutputDevice.AudioEndpointVolume.Mute,
                DisplayIconPath = GetDefaultIcon(PR_TYPE.MasterSounds)
            };

            InputMic = new ProcessModel()
            {
                DisplayName = "Microphone",
                TYPE = PR_TYPE.InputMic,
                VolumeLevel = (byte)(defaultInputDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100),
                IsMuted = defaultInputDevice.AudioEndpointVolume.Mute,
                DisplayIconPath = GetDefaultIcon(PR_TYPE.InputMic)
            };

            SessionCollection sessions = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioSessionManager.Sessions;
            for(int i = 0; i < sessions.Count; i++)
            {
                AudioSessionControl session = sessions[i];
                string PrName = Process.GetProcessById((int)session.GetProcessID).ProcessName;

                if (PrName == "taskhostw" || PrName == "Idle")
                {
                    SysSounds = new ProcessModel
                    {
                        DisplayName = "System Sounds",
                        TYPE = PR_TYPE.SysSounds,
                        VolumeLevel = (byte)(session.SimpleAudioVolume.Volume * 100),
                        IsMuted = session.SimpleAudioVolume.Mute,
                        AudioSessionControl = session,
                        DisplayIconPath = GetDefaultIcon(PR_TYPE.SysSounds)
                    };
                }
            }
        }

        private void ChangeVolume(byte newVolumeLevel)
        {
            
            switch (TYPE)
            {
                case PR_TYPE.MasterSounds:
                {
                    MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                    MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = VolumeLevel / 100.0f;
                    break;
                }

                case PR_TYPE.InputMic:
                {
                    MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                    MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                    defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = VolumeLevel / 100.0f;
                    break;
                }

                default:
                if (AudioSessionControl != null)
                {
                    AudioSessionControl.SimpleAudioVolume.Volume = VolumeLevel / 100.0f;
                }
                break;
            }
            
        }
        
        private void ChangeMute(bool vaule)
        {
            switch (TYPE)
            {
                case PR_TYPE.MasterSounds:
                {
                    MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                    MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                    defaultDevice.AudioEndpointVolume.Mute = vaule;
                    break;
                }

                case PR_TYPE.InputMic:
                {
                    MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
                    MMDevice defaultDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                    defaultDevice.AudioEndpointVolume.Mute = vaule;
                    break;
                }

                default:
                if (AudioSessionControl != null)
                {
                    AudioSessionControl.SimpleAudioVolume.Mute = vaule;
                }
                break;
            }
        }
        
        public static List<ProcessModel> GetAudioSessions()
        { 
            List<ProcessModel> processList = new List<ProcessModel>();
            MMDevice outputDevice = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            SessionCollection sessions = outputDevice.AudioSessionManager.Sessions;

            if (SysSounds == null || MasterSounds == null)
            {
                InitMasterSysSounds();
            }

            processList.Insert(0, MasterSounds);
            processList.Insert(1, SysSounds);

            for (int i = 0; i < sessions.Count; i++)
            {
                AudioSessionControl session = sessions[i];
                try
                {
                    Process process = Process.GetProcessById((int)session.GetProcessID); // System.ArgumentException process not running
                    ImageSource displayIconPath = GetDefaultIcon(PR_TYPE.Generic);
                    try
                    {
                        if (!string.IsNullOrEmpty(process.MainModule.FileName))
                        {
                            Icon processIcon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
                            if (processIcon != null)
                            {
                                displayIconPath = GetIcon(processIcon);
                            }
                        }
                    } catch { }

                    ProcessModel proccesModel = new ProcessModel
                    {
                        DisplayName = GetFriendlyProcessName(process),
                        TYPE = PR_TYPE.Generic,
                        VolumeLevel = (byte)(session.SimpleAudioVolume.Volume * 100),
                        IsMuted = session.SimpleAudioVolume.Mute,
                        AudioSessionControl = session,
                        DisplayIconPath = displayIconPath
                    };

                    if (process.ProcessName != "taskhostw" && process.ProcessName != "Idle")
                    {
                        processList.Add(proccesModel);
                    }
                }
                catch (ArgumentException)
                {
                    continue;
                }
            }
            
            return processList;
        }

        private static string GetFriendlyProcessName(Process process)
        {
            try
            {
                string productName = FileVersionInfo.GetVersionInfo(process.MainModule.FileName).FileDescription;
                return !string.IsNullOrEmpty(productName) ? productName : process.ProcessName;
            } catch (Exception) { }

            return process.ProcessName;
        }

        private static ImageSource GetIcon(Icon icon)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Bitmap bitmap = icon.ToBitmap();
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        
        private static ImageSource GetDefaultIcon(PR_TYPE type)
        {
            ImageSource imageResource = SettingsHelper.AppTextsLightTheme ? (ImageSource)Application.Current.TryFindResource("DefIcon_blackDrawingImage") : (ImageSource)Application.Current.TryFindResource("DefIcon_whiteDrawingImage");

            switch (type)
            {
                case PR_TYPE.MasterSounds:
                    imageResource = SettingsHelper.AppTextsLightTheme ? (ImageSource)Application.Current.TryFindResource("MasterSounds_blackDrawingImage") : (ImageSource)Application.Current.TryFindResource("MasterSounds_whiteDrawingImage");
                    break;
                case PR_TYPE.SysSounds:
                    imageResource = SettingsHelper.AppTextsLightTheme ? (ImageSource)Application.Current.TryFindResource("SysSounds_blackDrawingImage") : (ImageSource)Application.Current.TryFindResource("SysSounds_whiteDrawingImage");
                    break;
                default:
                    break;
            }
            return imageResource;
        }

    }
}