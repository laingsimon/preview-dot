using System;
using System.Runtime.InteropServices;

namespace PreviewDot
{
    internal static class WinApi
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        public const uint S_FALSE = 1;
    }
}
