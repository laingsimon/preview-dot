using NUnit.Framework;
using System;
using System.IO;
using System.Text;

namespace PreviewDot.UnitTests
{
	[TestFixture]
	public class Base64StreamTests
	{
		[Test]
		public void ShouldWriteBase64Data()
		{
			var underlyingStream = new MemoryStream();
			var base64Stream = new Base64Stream(underlyingStream, false);
			var dataAsBytes = Encoding.UTF8.GetBytes("hello world");

			base64Stream.Write(dataAsBytes, 0, dataAsBytes.Length);
			base64Stream.Flush();
			base64Stream.Close();

			var writtenData = Encoding.UTF8.GetString(underlyingStream.ToArray());
			Assert.That(writtenData, Is.EqualTo(Convert.ToBase64String(dataAsBytes)));
		}

		[Test]
		public void ShouldReadBase64Data()
		{
			var dataAsBytes = Encoding.UTF8.GetBytes("hello world");
			var base64Data = Convert.ToBase64String(dataAsBytes);
			var underlyingStream = new MemoryStream(Encoding.UTF8.GetBytes(base64Data));
			var base64Stream = new Base64Stream(underlyingStream, true);

			var readData = new StreamReader(base64Stream).ReadToEnd();

			Assert.That(readData, Is.EqualTo("hello world"));
		}

		[Test]
		public void ShouldReadBase64DataFromBase64String()
		{
			var dataAsBytes = Encoding.UTF8.GetBytes("hello world");
			var base64Data = Convert.ToBase64String(dataAsBytes);
			var base64Stream = new Base64Stream(base64Data);

			var readData = new StreamReader(base64Stream).ReadToEnd();

			Assert.That(readData, Is.EqualTo("hello world"));
		}
	}
}
