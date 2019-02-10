using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PreviewDot
{
	internal class CachingPreviewGenerator : IPreviewGenerator
	{
		private static readonly Dictionary<FileDetail, MemoryStream> _cache = new Dictionary<FileDetail, MemoryStream>();

		private readonly IPreviewGenerator _underlyingGenerator;

		public CachingPreviewGenerator(IPreviewGenerator underlyingGenerator)
		{
			if (underlyingGenerator == null)
				throw new ArgumentNullException("underlyingGenerator");

			_underlyingGenerator = underlyingGenerator;
		}

		public async Task<Stream> GeneratePreview(Stream drawingContent, FileDetail fileDetail, Size previewSize, CancellationToken token)
		{
			if (drawingContent == null)
				throw new ArgumentNullException("drawingContent");
			if (fileDetail == null)
				throw new ArgumentNullException("fileDetail");

			if (_cache.ContainsKey(fileDetail))
				return _CopyStream(_cache[fileDetail]);

			var preview = await _underlyingGenerator.GeneratePreview(drawingContent, fileDetail, previewSize, token);

			if (preview == null)
				throw new InvalidOperationException("Unexpected null preview response");

			var cachablePreview = _CopyStream(preview);
			_cache.Add(fileDetail, cachablePreview);

			return _CopyStream(cachablePreview);
		}

		private static MemoryStream _CopyStream(Stream source)
		{
			var copy = new MemoryStream();
			if (source.CanSeek)
				source.Position = 0;
			source.CopyTo(copy);

			copy.Position = 0;
			return copy;
		}

		public static void EvictFromCache(FileDetail fileDetail)
		{
			if (fileDetail == null)
				throw new ArgumentNullException("fileDetail");

			_cache.Remove(fileDetail);
		}
	}
}
