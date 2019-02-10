using NUnit.Framework;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PreviewDot.UnitTests
{
	[TestFixture]
	public class SizeExtractorTests
	{
		[Test]
		public async Task ShouldExtractSizeFromFileWhenCompressed()
		{
			const string compressedSampleFile = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<mxfile userAgent=""Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36"" type=""device""><diagram>fZJNc7MgEMc/DXeVjGOvT5qml5489Ex1A0zRdQhWfT59UJYQpy8eHPa37Mt/F8aP3Xy2YlBv2IJhRdbOjD+zoqiqzP9XsARwOFQBSKvbgPIEav0fCFKcHHUL191Fh2icHvawwb6Hxu2YsBan/bULmn3VQchYMYG6EeY7fdetUySrKBN/BS1VrJyXT8HzIZpPaXHsqR4r+GX7grsTMdcmlJ/8DC2iT7OeuvkIZp1jnFGYxssv3nuTFnpq5O8A71oDvoQZSWcAV7dE4ZPSDupBNKs9+d0y/k+5zngr90dKANYBrfqHJjZEHZwBO3B28VdiAK2YXkY0pzTmnGRk6mHEJTFBm5X3xEmwP5DmaKbZbr6Hx8pPNw==</diagram></mxfile>";
			var extractor = new SizeExtractor();
			var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(compressedSampleFile));
			var result = await extractor.ExtractSize(contentStream, new FileDetail("", DateTime.MinValue), CancellationToken.None);

			Assert.That(result.Value.Width, Is.EqualTo(880));
			Assert.That(result.Value.Height, Is.EqualTo(448));
		}

		[Test]
		public async Task ShouldExtractSizeFromFileWhenUncompressed()
		{
			const string sampleFile = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<mxGraphModel dx=""880"" dy=""492"" grid=""1"" gridSize=""10"" guides=""1"" tooltips=""1"" connect=""1"" arrows=""1"" fold=""1"" page=""1"" pageScale=""1"" pageWidth=""826"" pageHeight=""1169"" background=""#ffffff"" math=""0""><root><mxCell id=""0""/><mxCell id=""1"" parent=""0""/><mxCell id=""2"" value="""" style=""whiteSpace=wrap;html=1;"" vertex=""1"" parent=""1""><mxGeometry x=""10"" y=""10"" width=""120"" height=""60"" as=""geometry""/></mxCell></root></mxGraphModel>";
			var extractor = new SizeExtractor();
			var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(sampleFile));

			var result = await extractor.ExtractSize(contentStream, new FileDetail("", DateTime.MinValue), CancellationToken.None);

			Assert.That(result.Value.Width, Is.EqualTo(880));
			Assert.That(result.Value.Height, Is.EqualTo(492));
		}

		[Test]
		public async Task ShouldBeAbleToReadDataFromStreamAfterReadingSize()
		{
			const string sampleFile = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<mxGraphModel dx=""880"" dy=""492"" grid=""1"" gridSize=""10"" guides=""1"" tooltips=""1"" connect=""1"" arrows=""1"" fold=""1"" page=""1"" pageScale=""1"" pageWidth=""826"" pageHeight=""1169"" background=""#ffffff"" math=""0""><root><mxCell id=""0""/><mxCell id=""1"" parent=""0""/><mxCell id=""2"" value="""" style=""whiteSpace=wrap;html=1;"" vertex=""1"" parent=""1""><mxGeometry x=""10"" y=""10"" width=""120"" height=""60"" as=""geometry""/></mxCell></root></mxGraphModel>";
			var extractor = new SizeExtractor();
			var contentStream = new MemoryStream(Encoding.UTF8.GetBytes(sampleFile));

			await extractor.ExtractSize(contentStream, new FileDetail("", DateTime.MinValue), CancellationToken.None);
			var reader = new StreamReader(contentStream);
			var rereadXml = reader.ReadToEnd();

			Assert.That(rereadXml, Is.EqualTo(sampleFile));
		}
	}
}
