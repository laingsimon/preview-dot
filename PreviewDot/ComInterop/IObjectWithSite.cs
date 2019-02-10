using System;
using System.Runtime.InteropServices;

namespace PreviewDot.ComInterop
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("fc4801a3-2ba9-11cf-a229-00aa003d7352")]
	public interface IObjectWithSite
	{
		void SetSite([In, MarshalAs(UnmanagedType.IUnknown)] object pUnkSite);
		void GetSite(ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppvSite);
	}
}