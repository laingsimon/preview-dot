using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PreviewDot.Common;

namespace PreviewDot.UI
{
	internal class PreviewHandlerForm : Form, IPreviewHandlerForm
	{
		private readonly PreviewContext _context;
		private readonly IPreviewGenerator _previewGenerator;

		public PreviewHandlerForm()
			:this(new PreviewContext())
		{ }

		public PreviewHandlerForm(PreviewContext context)
			: this(context, new PreviewGeneratorFactory(context.Settings))
		{ }

		public PreviewHandlerForm(PreviewContext context, IPreviewGeneratorFactory previewGenerator)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (previewGenerator == null)
				throw new ArgumentNullException("previewGenerator");

			_context = context;
			_previewGenerator = previewGenerator.Create();
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
				var preview = await drawing.GeneratePreview(_previewGenerator, previewSize, _context.TokenSource.Token);

				try
				{
					if (preview.Length == 0)
					{
						throw new InvalidOperationException("No data was returned from preview application");
					}

					var image = Image.FromStream(preview);
					_context.RecalculateDrawingSize(image.Size);
					_InvokeOnUiThread(() => _ReplaceControl(new PreviewControl(image, _context)));
				}
				catch (Exception exc)
				{
					using (var reader = new StreamReader(preview))
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
				_InvokeOnUiThread(() => _ReplaceControl(new ErrorControl(exc)));
			}
		}

		public PreviewContext GetContext()
		{
			return _context;
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
