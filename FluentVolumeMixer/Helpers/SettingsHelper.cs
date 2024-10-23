namespace FluentVolumeMixer.Helpers
{
    public static class SettingsHelper
    {
        public static byte Windows11Effect
        {
            get => Properties.Settings.Default.Win11Effect;
            set
            {
                Properties.Settings.Default.Win11Effect = value;
                SaveSettings();
            }
        }

        public static bool EnableWindows10Effect
        {
            get => Properties.Settings.Default.Win10BgEffect;
            set
            {
                Properties.Settings.Default.Win10BgEffect = value;
                SaveSettings();
            }
        }

        public static bool AppBackgroundLightTheme
        {
            get => Properties.Settings.Default.AppLightTheme;
            set
            {
                Properties.Settings.Default.AppLightTheme = value;
                SaveSettings();
            }
        }

        public static bool AppTextsLightTheme
        {
            get => Properties.Settings.Default.AppLightTextTheme;
            set
            {
                Properties.Settings.Default.AppLightTextTheme = value;
                SaveSettings();
            }
        }

        public static bool EnableTextFogEffect
        {
            get => Properties.Settings.Default.EnableTextFogEffect;
            set
            {
                Properties.Settings.Default.EnableTextFogEffect = value;
                SaveSettings();
            }
        }

        public static bool TaskbarWhiteIcon
        {
            get => Properties.Settings.Default.TaskbarWhiteIcon;
            set
            {
                Properties.Settings.Default.TaskbarWhiteIcon = value;
                SaveSettings();
            }
        }

        private static void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }
    }
}
