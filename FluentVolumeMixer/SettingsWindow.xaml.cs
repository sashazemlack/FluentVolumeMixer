using FluentVolumeMixer.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;


namespace FluentVolumeMixer
{
    public partial class SettingsWindow : Window
    {
        public bool IsWin11 { get; set; } = true;
        public byte Win11Effect { get; set; } = 1;
        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Loaded += SettingsWindow_Loaded;
            this.Closed += SettingsWindow_Closed;
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            System.Windows.Threading.Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IsWin11 = Environment.OSVersion.Version.Build < 22523;
            Win11Effect = SettingsHelper.Windows11Effect;
            RestoreSettings();
        }

        private void RestoreSettings()
        {
            byte Win11Effect = SettingsHelper.Windows11Effect;
            switch (Win11Effect)
            { 
                case 1:
                    Win11EffectNone.IsChecked = true;
                    break;
                case 2:
                    Win11EffectMica.IsChecked = true;
                    break;
                case 3:
                    Win11EffectAcrylic.IsChecked = true;
                    break;
                case 4:
                    Win11EffectTabbed.IsChecked = true;
                    break;
            }

            CbWin10Acrylic.IsChecked = SettingsHelper.EnableWindows10Effect;
            CbTextFog.IsChecked = SettingsHelper.EnableTextFogEffect;
            CboxAppThemeBg.SelectedIndex = SettingsHelper.AppBackgroundLightTheme ? 1 : 0;
            CboxAppThemeTexts.SelectedIndex = SettingsHelper.AppTextsLightTheme ? 1 : 0;
            CboxAppThemeTrayIcon.SelectedIndex = SettingsHelper.TaskbarWhiteIcon ? 1 : 0;
        }

        private void EffectChecked_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            byte tag = Convert.ToByte(rb.Tag);
            SettingsHelper.Windows11Effect = tag;
        }

        private void CbWin10Acrylic_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            SettingsHelper.EnableWindows10Effect = (bool)cb.IsChecked;
        }

        private void CbTextFog_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            SettingsHelper.EnableTextFogEffect = (bool)cb.IsChecked;
        }

        private void CboxAppThemeBg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int tag = int.Parse((cb.SelectedItem as ComboBoxItem).Tag as string);
            bool newVaule = Convert.ToBoolean(tag);
            SettingsHelper.AppBackgroundLightTheme = newVaule;
        }

        private void CboxAppThemeTexts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int tag = int.Parse((cb.SelectedItem as ComboBoxItem).Tag as string);
            bool newVaule = Convert.ToBoolean(tag);
            SettingsHelper.AppTextsLightTheme = newVaule;
        }

        private void CboxAppThemeTrayIcon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            int tag = int.Parse((cb.SelectedItem as ComboBoxItem).Tag as string);
            bool newVaule = Convert.ToBoolean(tag);
            SettingsHelper.TaskbarWhiteIcon = newVaule;
        }
    }
}
