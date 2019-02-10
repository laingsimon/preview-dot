using NUnit.Framework;
using System.IO;
using System.Web;

namespace PreviewDot.UnitTests
{
	[TestFixture]
	public class UrlEncodingStreamTests
	{
		[Test]
		public void ShouldWriteUrlEncodedData()
		{
			var underlyingStream = new MemoryStream();
			var urlEncoding = new UrlEncodingStream(underlyingStream, false);
			var dataAsBytes = HttpUtility.UrlEncodeToBytes("<helloWorld />");
			
			urlEncoding.Write(dataAsBytes, 0, dataAsBytes.Length);
			urlEncoding.Flush();
			urlEncoding.Close();

			var writtenData = underlyingStream.ToArray();
			Assert.That(writtenData, Is.EqualTo(HttpUtility.UrlEncodeToBytes("<helloWorld />")));
		}

		[Test]
		public void ShouldReadUrlEncodedData()
		{
			var dataAsBytes = HttpUtility.UrlEncodeToBytes("<helloWorld />");
			var underlyingStream = new MemoryStream(dataAsBytes);
			var urlEncoding = new UrlEncodingStream(underlyingStream, true);

			var readData = new StreamReader(urlEncoding).ReadToEnd();

			Assert.That(readData, Is.EqualTo("<helloWorld />"));
		}
	}
}
