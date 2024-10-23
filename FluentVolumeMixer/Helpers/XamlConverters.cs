using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace FluentVolumeMixer.Helpers
{

    public class BoolThemeToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //bool DarkThemeRequired = (bool)value;
            bool LightThemeRequired = SettingsHelper.AppTextsLightTheme;
            return LightThemeRequired ? App.Current.Resources["LightThemeTextBrush"] : App.Current.Resources["DarkThemeTextBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolControlColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //bool LightThemeRequired = (bool)value;
            bool LightThemeRequired = SettingsHelper.AppBackgroundLightTheme;
            return LightThemeRequired ? App.Current.Resources["ControlLightBackground"] : App.Current.Resources["ControlDarkBackground"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolPageBackgroundToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //bool LightThemeRequired = (bool)value;
            bool LightThemeRequired = SettingsHelper.AppBackgroundLightTheme;
            return LightThemeRequired ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToDropShadowEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? new DropShadowEffect
                {
                    ShadowDepth = 0,
                    BlurRadius = 16,
                    Color = Colors.White,
                    Opacity = 1
                } : null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VolumeToGlyphConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return "\ue74f";

            if (values[0] is byte volume)
            {
                if (values[1] is bool param && param)
                {
                    return "\ue74f";
                }

                return volume switch
                {
                    0 => "\ue74f",
                    > 0 and <= 33 => "\ue993",
                    > 33 and <= 66 => "\ue994",
                    > 66 and <= 100 => "\ue995",
                    _ => "\ue74f"
                };
            }

            return "\ue74f";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MicLevelAndMuteToGlyphConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            byte volume = (byte)values[0];
            bool muted = (bool)values[1];
            if (volume > 0 && !muted)
            {
                return "\ue720";
            }
            else
            {
                return "\uf781";
            }            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
