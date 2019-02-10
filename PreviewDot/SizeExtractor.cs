using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace PreviewDot
{
	public interface ISizeExtractor
	{
		Task<Size?> ExtractSize(Stream fileContent, FileDetail fileDetail, CancellationToken token);
	}

	public class SizeExtractor : ISizeExtractor
	{
		public async Task<Size?> ExtractSize(Stream fileContent, FileDetail fileDetail, CancellationToken token)
		{
			if (fileContent == null)
				throw new ArgumentNullException("fileContent");
			if (fileDetail == null)
				throw new ArgumentNullException("fileDetail");
			if (!fileContent.CanRead)
				throw new ArgumentException("Stream must be readable", "fileContent");

			return await Task.Factory.StartNew(() =>
			{
				try
				{
					fileContent.Position = 0;
					var reader = new StreamReader(fileContent);
					var document = XDocument.Load(reader);

					return _ReadSizeFromDocument(document);
				}
				finally
				{
					fileContent.Position = 0;
				}
			}, token);
		}

		private static Size? _ReadSizeFromDocument(XDocument document)
		{
			var rootNode = document.Root;
			var rootNodeName = rootNode.Name.LocalName;
			if (!rootNodeName.Equals("mxGraphModel"))
				rootNode = _UnCompressDocument(rootNode.Element("diagram"));

			if (rootNode == null)
				return null;

			var dx = rootNode.Attribute("dx").Value;
			var dy = rootNode.Attribute("dy").Value;

			var size = new Size(int.Parse(dx), int.Parse(dy));
			if (size.Width > 0 && size.Height > 0)
				return size;

			return null;
		}

		private static XElement _UnCompressDocument(XElement diagram)
		{
			if (diagram == null)
				return null;

			var stream = CompressedXmlStream.Read(diagram.Value);
			using (var reader = new StreamReader(stream))
			{
				var urlEncodedXml = reader.ReadToEnd();
				var xml = HttpUtility.UrlDecode(urlEncodedXml);
				return XElement.Parse(xml);
			}
		}
	}
}
