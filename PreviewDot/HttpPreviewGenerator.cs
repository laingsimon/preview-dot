using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PreviewDot
{
	internal class HttpPreviewGenerator : IPreviewGenerator
	{
		private readonly HttpClient _client;
		private readonly Uri _requestUri;
		private readonly PreviewSettings _settings;

		public HttpPreviewGenerator(PreviewSettings settings, HttpClient client, Uri requestUri)
		{
			if (settings == null)
				throw new ArgumentNullException("settings");
			if (client == null)
				throw new ArgumentNullException("client");
			if (requestUri == null)
				throw new ArgumentNullException("requestUri");

			_settings = settings;
			_client = client;
			_requestUri = requestUri;
		}

		public async Task<Stream> GeneratePreview(Stream drawingContent, FileDetail fileDetail, Size previewSize, CancellationToken token)
		{
			if (drawingContent == null)
				throw new ArgumentNullException("drawingContent");
			if (fileDetail == null)
				throw new ArgumentNullException("fileDetail");
			if (!drawingContent.CanRead)
				throw new ArgumentException("Stream must be readable", "drawingContent");

			var request = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "filename", "preview" },
				{ "format", _settings.RenderingFormat.ToString() },
				{ "xml", _ReadFileContent(drawingContent) },
				{ "base64", "O" },
				{ "bg", "none" },
				{ "w", previewSize.Width.ToString() },
				{ "h", previewSize.Height.ToString() },
				{ "border", "1" }
			});

			var response = await _client.PostAsync(
				_requestUri,
				request,
				cancellationToken: token);

			return await response.Content.ReadAsStreamAsync();
		}

		private static string _ReadFileContent(Stream drawingContent)
		{
			using (var reader = new StreamReader(drawingContent))
			{
				var xml = reader.ReadToEnd();
				if (_IsCompressed(xml))
					return xml;

				return _Compress(xml);
			}
		}

		private static string _Compress(string xml)
		{
			var deflatedUrlEncodedXml = new MemoryStream();
			using (var encodingStream = CompressedXmlStream.Write(deflatedUrlEncodedXml))
			using (var writer = new StreamWriter(encodingStream, Encoding.UTF8))
				writer.Write(_RemoveXmlHeader(xml));

			var base64DeflatedXml = Encoding.UTF8.GetString(deflatedUrlEncodedXml.ToArray());

			var mxFile = new XElement(
				"mxfile",
				new XElement("diagram")
				{
					Value = base64DeflatedXml
				},
				new XAttribute("userAgent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.71 Safari/537.36"),
				new XAttribute("type", "device"));

			var compressedMxFile = new MemoryStream();
			using (var writer = XmlWriter.Create(compressedMxFile, new XmlWriterSettings
			{
				Indent = false,
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				mxFile.WriteTo(writer);
			}

			return Encoding.UTF8.GetString(compressedMxFile.ToArray());
		}

		private static string _RemoveXmlHeader(string xml)
		{
			var document = XDocument.Parse(xml);
			return document.Root.ToString();
		}

		private static bool _IsCompressed(string xml)
		{
			var headerText = xml.Substring(0, Math.Min(xml.Length, 53));

			return headerText.Contains("<mxfile ");
		}
	}
}
