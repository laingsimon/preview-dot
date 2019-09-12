using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using PreviewDot.ComInterop;

namespace PreviewDot
{
    internal partial class PreviewControl : UserControl
    {
        private const double _maxZoom = 2d;
        private const double _minZoom = 0.3d;

        private readonly PreviewContext _context;
        private readonly Image _originalPreview;
        private double? _currentZoom;
        private bool _ctrlPressed;
        private bool _shouldPan;
        private Point _panOrigin;
        private Point _scrollOrigin;

        public PreviewControl(Image preview, PreviewContext context)
        {
            if (preview == null)
                throw new ArgumentNullException("preview");
            if (context == null)
                throw new ArgumentNullException("context");

            _originalPreview = preview;
            _context = context;
            InitializeComponent();
            picPreview.Image = _ResizePreviewImageToSize(preview, context.DrawingSize);
            picPreview.Size = preview.Size;

            itmZoomIn.Enabled = context.DrawingSize != null;
            itmZoomOut.Enabled = context.DrawingSize != null;
            _UpdateDrawingDetails();
        }

        private Image _ResizePreviewImageToZoom(Image preview, double zoom)
        {
            if (_context.DrawingSize == null)
                return preview;

            var drawingSize = _context.DrawingSize.Value;
            var newSize = new Size((int)(drawingSize.Width * zoom), (int)(drawingSize.Height * zoom));
            return _ResizePreviewImageToSize(preview, newSize);
        }

        private static Image _ResizePreviewImageToSize(Image preview, Size? drawingSize)
        {
            if (drawingSize == null)
                return preview;

            var newSize = drawingSize.Value;
            var image = new Bitmap(newSize.Width, newSize.Height, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;

                graphics.DrawImage(preview, new Rectangle(Point.Empty, newSize));
            }

            return image;
        }

        // ReSharper disable InconsistentNaming
        private void itmPrint_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            string tempFile = null;

            try
            {
                tempFile = _CreateTempFile();

                _originalPreview.Save(tempFile, ImageFormat.Png);

                Wia.Print(this, tempFile);
            }
            catch (Exception exc)
            {
                if (ParentForm == null)
                    throw;

                var parentForm = ParentForm; //we have to store a reference to the ParentForm as it will be removed when this control is remove from it (by Controls.Clear())
                parentForm.Controls.Clear();
                parentForm.Controls.Add(new ErrorControl(exc)
                {
                    Dock = DockStyle.Fill
                });
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempFile))
                    _DeleteTempFile(tempFile);
            }
        }

        private static string _CreateTempFile()
        {
            try
            {
                var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");
                File.WriteAllBytes(path, new byte[0]);
                return path;
            }
            catch (Exception exc)
            {
                throw new IOException("Could not create temporary file for printing - " + exc.Message);
            }
        }

        private static void _DeleteTempFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
        }

        // ReSharper disable InconsistentNaming
        private void itmFitImage_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (_currentZoom != null)
                picPreview.Image = _ResizePreviewImageToSize(_originalPreview, _context.DrawingSize);

            itmFitImage.Checked = !itmFitImage.Checked;

            pnlScroller.AutoScrollMinSize = itmFitImage.Checked
                ? Size.Empty
                : picPreview.Image.Size;

            pnlScroller.AutoScroll = !itmFitImage.Checked;
            picPreview.SizeMode = itmFitImage.Checked
                ? PictureBoxSizeMode.Zoom
                : PictureBoxSizeMode.CenterImage;
            _currentZoom = null;
            picPreview_MouseUp(null, null);
            _UpdateDrawingDetails();
        }

        // ReSharper disable InconsistentNaming
        private void itmZoomIn_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            _UpdateZoom(0.1d);
        }

        // ReSharper disable InconsistentNaming
        private void itmZoomOut_Click(object sender, EventArgs e)
        // ReSharper restore InconsistentNaming
        {
            _UpdateZoom(-0.1d);
        }

        private double _CalculateZoomStart()
        {
            var zoom = _CalculateZoom();
            var roundedZoom = Math.Floor(zoom * 10) / 10;

            return roundedZoom;
        }

        private void _UpdateZoom(double step)
        {
            if (_currentZoom.HasValue)
                _currentZoom = _currentZoom.Value + step;
            else
            {
                _currentZoom = _CalculateZoomStart();
                if (_currentZoom.Value == _CalculateZoom())
                    _currentZoom = _currentZoom.Value + step;
            }

            _currentZoom = Math.Min(Math.Max(_currentZoom.Value, _minZoom), _maxZoom);

            picPreview.Image = _ResizePreviewImageToZoom(_originalPreview, _currentZoom.Value);
            itmFitImage.Checked = false;
            pnlScroller.AutoScrollMinSize = picPreview.Image.Size;
            pnlScroller.AutoScroll = true;
            picPreview.SizeMode = PictureBoxSizeMode.CenterImage;
            _UpdateDrawingDetails();
        }

        // ReSharper disable InconsistentNaming
        private void PreviewControl_KeyDown(object sender, KeyEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (e.Control)
                _ctrlPressed = true;
        }

        // ReSharper disable InconsistentNaming
        private void PreviewControl_KeyUp(object sender, KeyEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (e.KeyCode == Keys.Add && _ctrlPressed)
                _UpdateZoom(0.1d);
            if (e.KeyCode == Keys.Subtract && _ctrlPressed)
                _UpdateZoom(-0.1d);

            if (e.Control)
                _ctrlPressed = false;
        }

        // ReSharper disable InconsistentNaming
        private void PreviewControl_Scroll(object sender, ScrollEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (!_ctrlPressed)
                return;

            double? zoomAdjustment = null;

            switch (e.Type)
            {
                case ScrollEventType.LargeIncrement:
                    zoomAdjustment = 0.2d;
                    break;
                case ScrollEventType.LargeDecrement:
                    zoomAdjustment = -0.2d;
                    break;
                case ScrollEventType.SmallIncrement:
                    zoomAdjustment = 0.1d;
                    break;
                case ScrollEventType.SmallDecrement:
                    zoomAdjustment = -0.1d;
                    break;
            }

            if (zoomAdjustment == null)
                return;

            _UpdateZoom(zoomAdjustment.Value);
        }

        private void _UpdateDrawingDetails()
        {
            var zoomPercentage = _currentZoom ?? _CalculateZoom();
            var zoom = string.Format(" (x{0:n0}%)", zoomPercentage * 100);

            var size = _context.DrawingSize ?? picPreview.Image.Size;

            itmDrawingDetails.Text = string.Format(
                "{0} x {1}{2}",
                size.Width,
                size.Height,
                zoom);
        }

        private double _CalculateZoom()
        {
            var originalWidth = (double)_context.DrawingSize.Value.Width;

            if (originalWidth == 0)
                return 0;

            var displayWidth = (double)picPreview.Width;

            return Math.Round(displayWidth / originalWidth, 2);
        }

        // ReSharper disable InconsistentNaming
        private void picPreview_MouseDown(object sender, MouseEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (!pnlScroller.AutoScroll || e.Button != MouseButtons.Left)
                return;

            picPreview.Cursor = Cursors.SizeAll;
            _shouldPan = true;
            _panOrigin = new Point(
                e.Location.X + pnlScroller.AutoScrollPosition.X,
                e.Location.Y + pnlScroller.AutoScrollPosition.Y);
            _scrollOrigin = pnlScroller.AutoScrollPosition;
        }

        // ReSharper disable InconsistentNaming
        private void picPreview_MouseUp(object sender, MouseEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            picPreview.Cursor = Cursors.Default;
            _shouldPan = false;
            _panOrigin = Point.Empty;
            _scrollOrigin = Point.Empty;
        }

        // ReSharper disable InconsistentNaming
        private void picPreview_MouseMove(object sender, MouseEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            if (!_shouldPan)
                return;

            var currentLocation = new Point(
                e.Location.X + pnlScroller.AutoScrollPosition.X,
                e.Location.Y + pnlScroller.AutoScrollPosition.Y);

            var movement = new Point(
                currentLocation.X - _panOrigin.X,
                currentLocation.Y - _panOrigin.Y);

            var newScroll = new Point(
                0 - _scrollOrigin.X - movement.X,
                0 - _scrollOrigin.Y - movement.Y);

            pnlScroller.AutoScrollPosition = newScroll;
        }

        // ReSharper disable InconsistentNaming
        private void mnuContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        // ReSharper restore InconsistentNaming
        {
            _UpdateDrawingDetails();

            var zoomStart = _CalculateZoom();
            itmZoomIn.Enabled = zoomStart < _maxZoom;
            itmZoomOut.Enabled = zoomStart > _minZoom;
        }

        private void itmCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(_originalPreview);
        }
    }
}
