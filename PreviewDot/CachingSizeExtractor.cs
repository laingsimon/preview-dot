using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PreviewDot
{
	public class CachingSizeExtractor : ISizeExtractor
	{
		private readonly ISizeExtractor _underlyingExtractor;
		private static readonly Dictionary<FileDetail, Size?> _cache = new Dictionary<FileDetail, Size?>();

		public CachingSizeExtractor(ISizeExtractor underlyingExtractor)
		{
			if (underlyingExtractor == null)
				throw new ArgumentNullException("underlyingExtractor");

			_underlyingExtractor = underlyingExtractor;
		}

		public async Task<Size?> ExtractSize(Stream fileContent, FileDetail fileDetail, CancellationToken token)
		{
			if (fileContent == null)
				throw new ArgumentNullException("fileContent");
			if (fileDetail == null)
				throw new ArgumentNullException("fileDetail");

			if (_cache.ContainsKey(fileDetail))
				return _cache[fileDetail];

			var size = await _underlyingExtractor.ExtractSize(fileContent, fileDetail, token);
			_cache.Add(fileDetail, size);

			return size;
		}
	}
}