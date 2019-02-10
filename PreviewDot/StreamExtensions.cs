using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace PreviewDot
{
	internal static class StreamExtensions
	{
		public static Stream ToStream(this IStream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			return new _ReadOnlyStream(stream);
		}

		public static MemoryStream ToMemoryStream(this Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanRead)
				throw new ArgumentException("Stream must be readable", "stream");

			var buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);
			return new MemoryStream(buffer, false);
		}

    	public static string ReadAsString(this Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");
			if (!stream.CanRead)
				throw new ArgumentException("Stream must be readable", "stream");
			if (!stream.CanSeek)
				throw new InvalidOperationException("This operation would corrupt the stream");

			stream.Position = 0;

			try
			{
				var reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
			finally
			{
				stream.Position = 0;
			}
		}

		private class _ReadOnlyStream : Stream
		{
			// ReSharper disable InconsistentNaming
			private const int STATFLAG_NONAME = 1;
			// ReSharper restore InconsistentNaming

			private IStream _stream;

			public _ReadOnlyStream(IStream stream)
			{
				_stream = stream;
			}

			#region stream write members
			public override void Flush()
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override bool CanWrite { get; } = false;
			#endregion

			protected override void Dispose(bool disposing)
			{
				_stream = null;
			}

			public override unsafe long Seek(long offset, SeekOrigin origin)
			{
				if (_stream == null)
					throw new ObjectDisposedException("Stream");

				long pos = 0;
				var posPtr = new IntPtr(&pos);
				_stream.Seek(offset, (int)origin, posPtr);
				return pos;
			}

			public unsafe override int Read(byte[] buffer, int offset, int count)
			{
				if (_stream == null)
					throw new ObjectDisposedException("Stream");

				var bytesRead = 0;
				if (count > 0)
				{
					var ptr = new IntPtr(&bytesRead);
					if (offset == 0)
					{
						if (count > buffer.Length)
							throw new ArgumentOutOfRangeException("count");

						_stream.Read(buffer, count, ptr);
					}
					else
					{
						var tempBuffer = new byte[count];
						_stream.Read(tempBuffer, count, ptr);
						if (bytesRead > 0)
							Array.Copy(tempBuffer, 0, buffer, offset, bytesRead);
					}
				}
				return bytesRead;
			}

			public override bool CanRead => _stream != null;
			public override bool CanSeek => _stream != null;

			public override long Length
			{
				get
				{
					if (_stream == null)
						throw new ObjectDisposedException("Stream");

					STATSTG stats;
					_stream.Stat(out stats, STATFLAG_NONAME);
					return stats.cbSize;
				}
			}

			public override long Position
			{
				get
				{
					return Seek(0, SeekOrigin.Current);
				}
				set
				{
					Seek(value, SeekOrigin.Begin);
				}
			}
		}
	}
}
