﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PreviewDot.Common;

namespace PreviewDot.UI
{
	internal class PreviewGenerator : IPreviewGenerator
	{
		private readonly PreviewSettings _settings;

		public PreviewGenerator(PreviewSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			_settings = settings;
		}

		public async Task<Stream> GeneratePreview(Stream drawingContent, FileDetail fileDetail, Size previewSize, CancellationToken token)
		{
			if (drawingContent == null)
				throw new ArgumentNullException("drawingContent");
			if (fileDetail == null)
				throw new ArgumentNullException("fileDetail");
			if (!drawingContent.CanRead)
				throw new ArgumentException("Stream must be readable", "drawingContent");

			if (!File.Exists(_settings.DotApplicationPath))
				throw new FileNotFoundException("Could not execute dot.exe at configured path, file not found", _settings.DotApplicationPath);

			var process = new Process
			{
				StartInfo =
				{
					FileName = _settings.DotApplicationPath,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true,
					Arguments = "-T" + _settings.RenderingFormat.ToLower()
				},
				EnableRaisingEvents = true
			};

			Stream baseStream = Stream.Null;
			var errorStream = new MemoryStream();
			using (var errorStreamWriter = new StreamWriter(errorStream, Encoding.Default, 4096, true))
			{
				process.Start();

				process.ErrorDataReceived += (sender, args) =>
				{
					errorStreamWriter.Write(args.Data);
				};
				process.BeginErrorReadLine();

				baseStream = process.StandardOutput.BaseStream;

				await drawingContent.CopyToAsync(process.StandardInput.BaseStream).ConfigureAwait(false);
				process.StandardInput.BaseStream.Close();
			}

			var outputStream = new MemoryStream();
			baseStream.CopyTo(outputStream);

			process.WaitForExit();

			if (errorStream.Length > 0)
			{
				errorStream.Seek(0, SeekOrigin.Begin);
				var message = new StreamReader(errorStream).ReadToEnd();
				return GetStreamOfErrorMessage(message);
			}

			outputStream.Seek(0, SeekOrigin.Begin);
			return outputStream;
		}

		private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private Stream GetStreamOfErrorMessage(String errorMessage)
		{
			using (var image = new Bitmap(500, 500))
			{
				using (var graphics = Graphics.FromImage(image))
				{
					graphics.DrawString(
						errorMessage,
						new Font(FontFamily.GenericSansSerif, 10),
						Brushes.Black,
						PointF.Empty);
				}

				var outputStream = new MemoryStream();
				image.Save(outputStream, (ImageFormat)Enum.Parse(typeof(ImageFormat), _settings.RenderingFormat, true));
				outputStream.Seek(0, SeekOrigin.Begin);
				return outputStream;
			}
		}
	}
}
