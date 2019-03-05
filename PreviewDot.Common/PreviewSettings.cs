using System;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace PreviewDot.Common
{
	public class PreviewSettings : MarshalByRefObject
	{
		public static readonly string InstallDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		public int UpScaleForPrint { get; set; }
		public string RenderingFormat { get; set; }
		public string DotApplicationPath { get; set; }

		public PreviewSettings()
		{
			UpScaleForPrint = _ReadInt(ConfigurationManager.AppSettings["upScale"], 4);
			RenderingFormat = _ReadImageFormat(ConfigurationManager.AppSettings["format"], ImageFormat.Png).ToString();
			DotApplicationPath = ConfigurationManager.AppSettings["dotPath"];
			if (string.IsNullOrEmpty(DotApplicationPath))
			{
				DotApplicationPath = Path.Combine(InstallDirectory, @"external\dot.exe");
			}
		}

		private static ImageFormat _ReadImageFormat(string value, ImageFormat defaultFormat)
		{
			if (string.IsNullOrEmpty(value))
				return defaultFormat;

			var property = typeof(ImageFormat).GetProperty(value, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.IgnoreCase);
			if (property == null)
				return defaultFormat;

			return (ImageFormat)property.GetValue(null);
		}

		private static int _ReadInt(string value, int defaultResolution)
		{
			if (string.IsNullOrEmpty(value))
				return defaultResolution;

			int resolution;
			return !int.TryParse(value, out resolution)
				? defaultResolution
				: resolution;
		}
	}
}
