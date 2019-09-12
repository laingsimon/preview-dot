using System.Drawing;
using System.Runtime.InteropServices;

namespace PreviewDot.ComInterop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct COLORREF
    {
        public uint Dword;
        public Color Color
        {
            get
            {
                return Color.FromArgb(
                (int)(0x000000FFU & Dword),
                (int)(0x0000FF00U & Dword) >> 8,
                (int)(0x00FF0000U & Dword) >> 16);
            }
        }
    }
}