using System.Diagnostics;
using System.IO;

namespace PreviewDot.Common
{
	public static class Logging
	{
		private static readonly StringWriter _log = new StringWriter();

		public static void InstallListeners()
		{
			Trace.Listeners.Add(
				new TextWriterTraceListener(_log, "Text-writer trace-listener"));
		}

		public static string ReadLog()
		{
			var log = _log.GetStringBuilder().ToString();
			_log.GetStringBuilder().Clear();

			return log;
		}
	}
}
