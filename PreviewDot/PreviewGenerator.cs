using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PreviewDot
{
    internal class PreviewGenerator : IPreviewGenerator
    {
        private readonly PreviewSettings _settings;
        private readonly StreamHelper _streamHelper;

        public PreviewGenerator(PreviewSettings settings, StreamHelper streamHelper = null)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _settings = settings;
            _streamHelper = streamHelper ?? new StreamHelper();
        }

        public async Task<GeneratePreviewResult> GeneratePreview(Stream drawingContent, FileDetail fileDetail, Size previewSize, CancellationToken token)
        {
            if (drawingContent == null)
                throw new ArgumentNullException(nameof(drawingContent));
            if (fileDetail == null)
                throw new ArgumentNullException(nameof(fileDetail));
            if (!drawingContent.CanRead)
                throw new ArgumentException("Stream must be readable", nameof(drawingContent));

            var process = new Process
            {
                StartInfo =
                {
                    FileName = Path.Combine(_settings.DotApplicationPath),
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = "-T" + _settings.RenderingFormat.ToString().ToLower()
                },
                EnableRaisingEvents = true
            };

            process.Start();

            var baseOutputStream = process.StandardOutput.BaseStream;
            var baseErrorStream = process.StandardError.BaseStream;

            var drawingStreamSansBom = _streamHelper.ExcludeBomFromStream(drawingContent);
            await drawingStreamSansBom.CopyToAsync(process.StandardInput.BaseStream);
            process.StandardInput.BaseStream.Close();

            var outputStream = new MemoryStream();
            baseOutputStream.CopyTo(outputStream);

            var errorStream = new MemoryStream();
            baseErrorStream.CopyTo(errorStream);

            process.WaitForExit();

            if (errorStream.Length > 0)
            {
                errorStream.Seek(0, SeekOrigin.Begin);
                var message = new StreamReader(errorStream).ReadToEnd();
                return new GeneratePreviewResult(message);
            }

            if (process.ExitCode != 0)
                return new GeneratePreviewResult("Failed to render drawing: " + process.ExitCode);

            outputStream.Seek(0, SeekOrigin.Begin);
            return new GeneratePreviewResult(outputStream);
        }
    }
}
