﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PreviewDot
{
    internal class PreviewHandlerForm : Form
    {
        private readonly PreviewContext _context;
        private readonly IPreviewGenerator _previewGenerator;

        public PreviewHandlerForm(PreviewContext context, IPreviewGenerator previewGenerator)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (previewGenerator == null)
                throw new ArgumentNullException("previewGenerator");

            _context = context;
            _previewGenerator = previewGenerator;
            context.PreviewRequired += _PreviewRequired;
            context.ViewPortChanged += _ViewPortChanged;

            InitializeComponent();
        }

        private void _ViewPortChanged(object sender, EventArgs e)
        {
            _InvokeOnUiThread(_UpdateSize);
        }

        private void _PreviewRequired(object sender, EventArgs e)
        {
            if (!_context.DisplayPreview)
                return;

            _InvokeOnUiThread(_UpdatePreview);
        }

        private void _InvokeOnUiThread(MethodInvoker method)
        {
            if (!Created)
                return;

            if (!InvokeRequired)
                method.Invoke();
            else
                Invoke(method);
        }

        private void _UpdateSize()
        {
            Bounds = _context.ViewPort;
        }

        private async void _UpdatePreview()
        {
            try
            {
                _UpdateSize();
                _Reset();

                var drawing = new Drawing(_context.FileStream, _context.FileDetail);

                _context.DrawingSize = new Size(1000, 1000);
                var previewSize = _context.GetPreviewSize();
                var previewResult = await drawing.GeneratePreview(_previewGenerator, previewSize, _context.TokenSource.Token);

                if (!previewResult.Success)
                {
                    _InvokeOnUiThread(() => _ReplaceControl(new ErrorControl(previewResult.ErrorMessage)));
                    return;
                }

                try
                {
                    var image = Image.FromStream(previewResult.ImageData);
                    _context.RecalculateDrawingSize(image.Size);
                    _InvokeOnUiThread(() => _ReplaceControl(new PreviewControl(image, _context)));
                }
                catch (Exception exc)
                {
                    previewResult.ImageData.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(previewResult.ImageData))
                    {
                        var responseBody = reader.ReadToEnd();
                        throw new InvalidOperationException("Invalid image data returned: " + responseBody, exc);
                    }
                }
            }
            catch (OperationCanceledException)
            { }
            catch (Exception exc)
            {
                _InvokeOnUiThread(() => _ReplaceControl(new ErrorControl(exc.ToString())));
            }
        }

        public void Reset()
        {
            _InvokeOnUiThread(_Reset);
        }

        private void _Reset()
        {
            _ReplaceControl(new LoadingControl());
        }

        private void _ReplaceControl(UserControl control)
        {
            Controls.Clear();
            Controls.Add(control);
            control.Dock = DockStyle.Fill;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // PreviewHandlerForm
            //
            this.BackColor = SystemColors.Window;
            this.ClientSize = new Size(284, 262);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "PreviewHandlerForm";
            this.ResumeLayout(false);
        }
    }
}
