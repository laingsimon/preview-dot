using System;
using System.Runtime.InteropServices;

namespace PreviewDot.ComInterop
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("8895b1c6-b41f-4c1c-a562-0d564250836f")]
	interface IPreviewHandler
	{
		void SetWindow(IntPtr hwnd, ref RECT rect);
		void SetRect(ref RECT rect);
		void DoPreview();
		void Unload();
		void SetFocus();
		void QueryFocus(out IntPtr phwnd);
		[PreserveSig]
		uint TranslateAccelerator(ref MSG pmsg);
	}
}