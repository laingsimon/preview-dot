using System.Configuration;
using System.Drawing.Imaging;
using System.Reflection;

namespace PreviewDot
{
	internal class PreviewSettings
	{
		public int UpScaleForPrint { get; set; }
		public ImageFormat RenderingFormat { get; set; }

		public PreviewSettings()
		{
			UpScaleForPrint = _ReadInt(ConfigurationManager.AppSettings["upScale"], 4);
			RenderingFormat = _ReadImageFormat(ConfigurationManager.AppSettings["format"], ImageFormat.Png);
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
