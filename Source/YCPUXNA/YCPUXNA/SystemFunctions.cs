using System;
using System.Runtime.InteropServices;

namespace YCPUXNA
{
    static class SystemFunctions
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void SetFocus(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                SetForegroundWindow(hWnd);
                ShowWindow(hWnd, 1);
            }
        }
    }
}
