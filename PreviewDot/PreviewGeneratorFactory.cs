using System;

namespace PreviewDot
{
    internal class PreviewGeneratorFactory
	{
		private readonly PreviewSettings _settings;

		public PreviewGeneratorFactory(
			PreviewSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");

			_settings = settings;
		}

		public IPreviewGenerator Create()
		{
			return new PreviewGenerator(_settings);
		}
	}
}
