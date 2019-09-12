using System.Runtime.InteropServices;

namespace PreviewDot.ComInterop
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("8327b13c-b63f-4b24-9b8a-d010dcc3f599")]
    interface IPreviewHandlerVisuals
    {
        void SetBackgroundColor(COLORREF color);
        void SetFont(ref LOGFONT plf);
        void SetTextColor(COLORREF color);
    }
}