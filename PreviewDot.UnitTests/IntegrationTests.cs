using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace PreviewDot.UnitTests
{
	[TestFixture]
	public class IntegrationTests
	{
		[TestCase("Sample file.xml")]
		[TestCase("Sample file - plain.xml")]
		public async Task ShouldBeAbleToGenerateAPreview(string fileName)
		{
			var settings = new PreviewSettings
			{
				RenderingFormat = ImageFormat.Png,
				UpScaleForPrint = 4
			};

			using (var fileStream = File.OpenRead(@"..\..\..\" + fileName))
			{
				var drawing = new Drawing(fileStream, new FileDetail("file", DateTime.MinValue));
				var previewGeneratorFactory = new HttpPreviewGeneratorFactory(settings);
				var generator = previewGeneratorFactory.Create();

				var preview = await drawing.GeneratePreview(generator, new Size(100, 100), CancellationToken.None);

				Assert.That(
					() => Image.FromStream(preview),
					Throws.Nothing);
			}
		}
	}
}
