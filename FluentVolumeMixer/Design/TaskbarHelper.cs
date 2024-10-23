using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace FluentVolumeMixer.Design
{
    public class TaskbarHelper
    {
        private const int ABM_GETTASKBARPOS = 0x00000005;

        // Make the RECT structure public
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;   // Make fields public
            public int top;    // Make fields public
            public int right;  // Make fields public
            public int bottom; // Make fields public
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;   // RECT structure is now public
            public IntPtr lParam;
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        public enum TaskbarPosition
        {
            Top,
            Bottom,
            Left,
            Right
        }

        // Make the GetTaskbarPosition method public and return the RECT structure
        public static TaskbarPosition GetTaskbarPosition(out RECT taskbarRect)
        {
            APPBARDATA appBarData = new APPBARDATA();
            appBarData.cbSize = Marshal.SizeOf(appBarData);
            IntPtr result = SHAppBarMessage(ABM_GETTASKBARPOS, ref appBarData);

            taskbarRect = appBarData.rc;

            switch (appBarData.uEdge)
            {
                case 0: // ABE_LEFT
                    return TaskbarPosition.Left;
                case 1: // ABE_TOP
                    return TaskbarPosition.Top;
                case 2: // ABE_RIGHT
                    return TaskbarPosition.Right;
                case 3: // ABE_BOTTOM
                    return TaskbarPosition.Bottom;
                default:
                    throw new InvalidOperationException("Unknown taskbar position.");
            }
        }
    }

}
