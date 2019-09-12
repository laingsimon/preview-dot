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
        [TestCase("Sample file.gv")]
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
                var previewGeneratorFactory = new PreviewGeneratorFactory(settings);
                var generator = previewGeneratorFactory.Create();

                var preview = await drawing.GeneratePreview(generator, new Size(100, 100), CancellationToken.None);

                Assert.That(preview.Length, Is.GreaterThan(0));
                Assert.That(
                    () => Image.FromStream(preview),
                    Throws.Nothing);
            }
        }
    }
}
