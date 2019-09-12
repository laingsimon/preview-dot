using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using PreviewDot.ComInterop;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace PreviewDot
{
    [ProgId("PreviewDot.PreviewHandlerController")]
    [Guid(Installer.ControllerId)]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public class PreviewHandlerController : IPreviewHandler, IOleWindow, IObjectWithSite, IInitializeWithStream
    {
        private readonly PreviewContext _context;
        private readonly PreviewHandlerForm _previewForm;

        private IntPtr _previewWindowHandle;
        private Stream _previewFileStream = Stream.Null;
        private IPreviewHandlerFrame _frame;
        private FileDetail _fileDetail;

        public PreviewHandlerController()
        {
            try
            {
                Logging.InstallListeners();

                _context = new PreviewContext();

                var previewGeneratorFactory = new PreviewGeneratorFactory(_context.Settings);
                var generator = previewGeneratorFactory.Create();

                _previewForm = new PreviewHandlerForm(_context, generator);
                _previewForm.Handle.GetHashCode(); //initialse the form
            }
            catch (Exception exc)
            {
                Trace.TraceError("PreviewHandlerController.ctor: {0}", exc);
            }
        }

        void IInitializeWithStream.Initialize(IStream pstream, uint grfMode)
        {
            try
            {
                if (_previewForm == null)
                    return;

                _previewForm.Reset();
                _fileDetail = _GetPreviewFileDetail(pstream);
                _previewFileStream = pstream.ToStream().ToMemoryStream();
                Marshal.ReleaseComObject(pstream);
            }
            catch (Exception exc)
            {
                Trace.TraceError("PreviewHandlerController.Initialize: {0}", exc);
            }
        }

        private static FileDetail _GetPreviewFileDetail(IStream pstream)
        {
            STATSTG stats;
            pstream.Stat(out stats, 0);

            return new FileDetail(
                stats.pwcsName,
                DateTime.FromFileTime(stats.mtime.dwHighDateTime));
        }

        void IPreviewHandler.SetWindow(IntPtr hwnd, ref RECT rect)
        {
            try
            {
                if (_previewForm == null)
                    return;

                _previewForm.Invoke(new MethodInvoker(() => _previewForm.Show()));

                _previewWindowHandle = hwnd;
                _context.OnViewPortChanged(rect.ToRectangle());
                WinApi.SetParent(_previewForm.Handle, _previewWindowHandle);
            }
            catch (Exception exc)
            {
                Trace.TraceError("PreviewHandlerController.SetWindow: {0}", exc);
            }
        }

        void IPreviewHandler.SetRect(ref RECT rect)
        {
            try
            {
                if (_previewForm == null)
                    return;

                _previewForm.Invoke(new MethodInvoker(() => _previewForm.Show()));

                _context.OnViewPortChanged(rect.ToRectangle());

                WinApi.SetParent(_previewForm.Handle, _previewWindowHandle); //is this required? - if not then remove _previewWindowHandle?
            }
            catch (Exception exc)
            {
                Trace.TraceError("PreviewHandlerController.SetRect: {0}", exc);
            }
        }

        public void DoPreview()
        {
            try
            {
                if (_previewForm == null)
                    return;

                _previewForm.Invoke(new MethodInvoker(() => _previewForm.Show()));

                if (_previewFileStream != Stream.Null)
                {
                    _context.OnPreviewRequired(_previewFileStream, _fileDetail);
                    WinApi.SetParent(_previewForm.Handle, _previewWindowHandle); //is this required? - if not then remove _previewWindowHandle
                }
                else
                {
                    Trace.TraceError("No File stream set");
                }
            }
            catch (Exception exc)
            {
                Trace.TraceError("PreviewHandlerController.DoPreview: {0}", exc);
            }
            finally
            {
                if (_previewFileStream != Stream.Null)
                {
                    _previewFileStream.Dispose();
                }
            }
        }

        public void Unload()
        {
            _previewForm?.Invoke(new MethodInvoker(() => _previewForm.Reset()));
        }

        public void SetFocus()
        {
            _previewForm?.Invoke(new MethodInvoker(() => _previewForm.Focus()));
        }

        public void QueryFocus(out IntPtr phwnd)
        {
            var focusResult = IntPtr.Zero;
            _previewForm?.Invoke(new MethodInvoker(() => WinApi.GetFocus()));

            phwnd = focusResult;
        }

        uint IPreviewHandler.TranslateAccelerator(ref MSG pmsg)
        {
            if (_previewForm != null && _frame != null)
            {
                var msg = new Message
                {
                    HWnd = pmsg.hwnd,
                    LParam = pmsg.lParam,
                    Msg = pmsg.message,
                    WParam = pmsg.wParam
                };

                if (_previewForm.PreProcessMessage(ref msg))
                    return _frame.TranslateAccelerator(ref pmsg);
            }

            return WinApi.S_FALSE;
        }

        public void GetWindow(out IntPtr phwnd)
        {
            phwnd = _previewForm?.Handle ?? IntPtr.Zero;
        }

        public void ContextSensitiveHelp(bool fEnterMode)
        {
            //not implemented
        }

        public void SetSite(object pUnkSite)
        {
            _frame = pUnkSite as IPreviewHandlerFrame;
        }

        public void GetSite(ref Guid riid, out object ppvSite)
        {
            ppvSite = _frame;
        }

        [ComRegisterFunction]
        public static void Register(Type type)
        {
            if (type != typeof(PreviewHandlerController))
                return;

            Installer.RegisterPreviewHandler("DOT format diagram previewer", type);
        }

        [ComUnregisterFunction]
        public static void Unregister(Type type)
        {
            if (type != typeof(PreviewHandlerController))
                return;

            Installer.UnregisterPreviewHandler(type);
        }
    }
}
