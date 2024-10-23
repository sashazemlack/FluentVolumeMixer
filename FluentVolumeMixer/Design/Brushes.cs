using Microsoft.Win32;
using System.Runtime.InteropServices;
using System;
using System.Windows.Media;

namespace FluentVolumeMixer.Design
{
    public static class Brushes
    {
        //Extern methods
        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        private static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType,
                                                                    bool bIgnoreHighContrast, uint dwHighContrastCacheMode);
        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        private static extern uint GetImmersiveColorTypeFromName(IntPtr pName);
        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        private static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);
        //Public methods
        private static Color GetAccentColor()
        {
            int userColorSet = GetImmersiveUserColorSetPreference(false, false);
            uint colorType = GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground"));
            uint colorSetEx = GetImmersiveColorFromColorSetEx((uint)userColorSet, colorType, false, 0);
            return ConvertDWordColorToRGB(colorSetEx);
        }
        //Private methods
        private static Color ConvertDWordColorToRGB(uint colorSetEx)
        {
            byte redColor = (byte)((0x000000FF & colorSetEx) >> 0);
            byte greenColor = (byte)((0x0000FF00 & colorSetEx) >> 8);
            byte blueColor = (byte)((0x00FF0000 & colorSetEx) >> 16);
            //byte alphaColor = (byte)((0xFF000000 & colorSetEx) >> 24);
            return Color.FromRgb(redColor, greenColor, blueColor);
        }


        public static SolidColorBrush GetAccentColorBrush()
        {
            return new SolidColorBrush(GetAccentColor());
            //return new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xD7));
        }


    }
}
