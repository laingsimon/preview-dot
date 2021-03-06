﻿using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PreviewDot
{
    internal interface IPreviewGenerator
    {
        Task<GeneratePreviewResult> GeneratePreview(Stream drawingContent, FileDetail fileDetail, Size previewSize, CancellationToken token);
    }
}
