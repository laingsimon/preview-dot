using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace PreviewDot.UnitTests
{
	[TestFixture]
	public class CompressedXmlStreamTests
	{
		private const string decompressedData = @"<mxGraphModel dx=""1434"" dy=""782"" grid=""1"" gridSize=""10"" guides=""1"" tooltips=""1"" connect=""1"" arrows=""1"" fold=""1"" page=""1"" pageScale=""1"" pageWidth=""826"" pageHeight=""1169"" background=""#ffffff"" math=""0""><root><mxCell id=""0""/><mxCell id=""1"" parent=""0""/><mxCell id=""3"" value="""" style=""verticalLabelPosition=bottom;html=1;verticalAlign=top;strokeColor=none;fillColor=#00BEF2;shape=mxgraph.azure.file_2;fontSize=12;"" vertex=""1"" parent=""1""><mxGeometry x=""213"" y=""660"" width=""47.5"" height=""50"" as=""geometry""/></mxCell></root></mxGraphModel>";
		private const string compressedData = @"fZNBr6IwEMc/DVcDVNE9Lq7PPewmL/Gwx02FAo2lQ0rxoZ/eKZ2KxOx6ofObf2faf8eI7dvxaHjX/IZSqCiNyzFiP6I0TdZsjR9Hbp5sdxg5UBtZkmgGJ3kXBGOigyxFvxBaAGVlt4QFaC0Ku2DcGPhayipQy64dr0PHGZwKrt7pH1naxtNdms38p5B1Ezon2TefOfPiUhsYNPWLUlZNP59ueag1XZQd0EQDgGXcqh33Qjkjg0fejY9/ZJ+HNELTQf6/gfkNV64GuqcHvb2Fi1+FsRJ9+MXPQn1CL60EjakzWAttxPLGtgrjBJdB+13J2mksdEh7a+Ai9qDAINOgsXJeSaUCQkviOD984PXzvuGd69yOtRukFb8PRqxQLf66dAXa0nAkLqbzY18RRu3dgwmRAUcBrbDmhhLakCbkAk1mltHIfc3vvN6uNh42L4+8ISGn2aqftWfLcUGuh3B+3Sn38n9hhwc=";

		[Test]
		public void ShouldBeAbleToReadDrawIoData()
		{
			using (var stream = CompressedXmlStream.Read(compressedData))
			using (var reader = new StreamReader(stream))
			{
				var decompressedData = reader.ReadToEnd();

				Assert.That(
					decompressedData,
					Is.EqualTo(CompressedXmlStreamTests.decompressedData));
			}
		}

		[Test]
		public void ShouldBeAbleToWriteDrawIoData()
		{
			var underlyingStream = new MemoryStream();
			using (var stream = CompressedXmlStream.Write(underlyingStream))
			using (var writer = new StreamWriter(stream))
			{
				writer.Write(decompressedData);
				writer.Flush();
			}

			var compressedData = Encoding.UTF8.GetString(underlyingStream.ToArray());
			Assert.That(
					compressedData,
					Is.EqualTo(CompressedXmlStreamTests.compressedData),
					"\r\nExpected: " + _WriteBytes(CompressedXmlStreamTests.compressedData) + "\r\nActual: " + _WriteBytes(compressedData));
		}

		[Test]
		public void ShouldBeAbleToReadWrittenData()
		{
			const string compressedData = @"ZVJNb8IwDP0rUXaHtkBhKkXa0MYOmzSJw45TaN02Io2r1EDh189t2AdaTn628/L8nGXdbZxqqjfMwYi8S2U4nUylyM+pnC8iKUqnc076YKsvwCBgdNA5tEOBEA3pxoMMrYWMhlg5hyefLtB4lkaV8BNsM2V+0YfOqUrlIoo9fgFdVj1TGN9LsVPZvnR4sMxzVwxHilr1NwK5WjpEWi3rbg3GiF5xIMc3eHjEgaX/pYkUR2UOrESKls69pCM40qzuVe3AvGOrSaNNd0iEdVJRbdIw+e55MLq0KWGTtORwD2s06FKLFpJCG+PhXRA8Pj1HSVupBtK6K3vTR+pycDDiLviMkgItDQaHUcKKmB26G92h7GVvAGsgdxZcjELWzpuKY17Jyfs3nY9mUlRX82ZcULyD8nqrH33sZ+fAuzb++wdWXw==";
			using (var stream = CompressedXmlStream.Read(compressedData))
			using (var reader = new StreamReader(stream))
			{
				var decompressedData = reader.ReadToEnd();

				Assert.That(
					decompressedData,
					Is.EqualTo(CompressedXmlStreamTests.decompressedData));
			}
		}

		private static string _WriteBytes(string base64)
		{
			var bytes = Convert.FromBase64String(base64);
			var head = bytes.Take(25);

			var tailStart = bytes.Length - 10;
			if (tailStart <= 0)
				tailStart = bytes.Length;

			var tail = bytes.Skip(tailStart);

			return string.Format("[{0} ... {1}] x {2}", _WriteBytes(head), _WriteBytes(tail), bytes.Length);
		}

		private static string _WriteBytes(IEnumerable<byte> bytes)
		{
			return string.Join(" ", bytes.Select(b => Convert.ToString(b, 16)));
		}
	}
}
