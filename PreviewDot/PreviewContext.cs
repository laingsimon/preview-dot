using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace PreviewDot
{
    internal class PreviewContext
    {
        public event EventHandler ViewPortChanged;
        public event EventHandler PreviewRequired;

        public Rectangle ViewPort { get; private set; }
        public bool DisplayPreview { get; private set; }
        public Stream FileStream { get; private set; }
        public PreviewSettings Settings { get; }
        public CancellationTokenSource TokenSource { get; private set; }
        public Size? DrawingSize { get; set; }
        public FileDetail FileDetail { get; private set; }

        public PreviewContext()
        {
            TokenSource = new CancellationTokenSource();
            Settings = new PreviewSettings();
        }

        public void OnViewPortChanged(Rectangle newSize)
        {
            ViewPort = newSize;

            ViewPortChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OnPreviewRequired(Stream stream, FileDetail fileDetail)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (fileDetail == null)
                throw new ArgumentNullException("fileDetail");
            if (!stream.CanRead)
                throw new ArgumentException("Stream must be readable", "stream");

            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
            FileStream = stream;
            FileDetail = fileDetail;
            DisplayPreview = true;
            PreviewRequired?.Invoke(this, EventArgs.Empty);
        }

        public Size GetPreviewSize()
        {
            return _IncreaseSizeForPrint(DrawingSize) ?? Size.Empty;
        }

        private Size? _IncreaseSizeForPrint(Size? drawingSize)
        {
            if (drawingSize == null)
                return null;

            var size = drawingSize.Value;
            return new Size(size.Width * Settings.UpScaleForPrint, size.Height * Settings.UpScaleForPrint);
        }

        public void RecalculateDrawingSize(Size upscaledPreviewSize)
        {
            if (Settings.UpScaleForPrint <= 0)
                throw new InvalidOperationException("Settings.UpScaleForPrint must be a positive number");

            var actualSize = new Size(
                upscaledPreviewSize.Width / Settings.UpScaleForPrint,
                upscaledPreviewSize.Height / Settings.UpScaleForPrint);

            var previousDrawingSize = DrawingSize;
            DrawingSize = actualSize;

            if (previousDrawingSize == null || previousDrawingSize.Value.Width == 0 || previousDrawingSize.Value.Height == 0)
                return;

            //work out the actual scale of the preview compared to the requested size
            var scale = new SizeF(
                ((float)actualSize.Width / previousDrawingSize.Value.Width) * Settings.UpScaleForPrint,
                ((float)actualSize.Height / previousDrawingSize.Value.Height) * Settings.UpScaleForPrint);

            var mostAppropriateScale = _GetMostAppropriateScale(scale);
            if (mostAppropriateScale == 0)
                mostAppropriateScale = 1;

            //reset the drawing size to that of the preview
            DrawingSize = new Size(
                upscaledPreviewSize.Width / mostAppropriateScale,
                upscaledPreviewSize.Height / mostAppropriateScale);
        }

        private int _GetMostAppropriateScale(SizeF scale)
        {
            var widthScale = Math.Abs(Settings.UpScaleForPrint - scale.Width);
            var heightScale = Math.Abs(Settings.UpScaleForPrint - scale.Height);

            if (widthScale < heightScale)
                return (int)scale.Height;

            return (int)scale.Width;
        }
    }
}
