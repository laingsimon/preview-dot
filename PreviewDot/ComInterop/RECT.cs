using System.Drawing;
using System.Runtime.InteropServices;

namespace PreviewDot.ComInterop
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;
		public Rectangle ToRectangle() { return Rectangle.FromLTRB(left, top, right, bottom); }
		public static RECT FromRectangle(Rectangle source)
		{
			return new RECT
			{
				left = source.Left,
				top = source.Top,
				right = source.Right,
				bottom = source.Bottom
			};
		}
	}
}
