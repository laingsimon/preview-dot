using System.Windows.Forms;
using System.Xml.Linq;

namespace PreviewDot
{
	public partial class XmlControl : UserControl
	{
		private bool _reformat = false;
		private readonly string _xml;

		public XmlControl(string xml = null, bool initiallyReformatted = false)
		{
			InitializeComponent();
			_reformat = initiallyReformatted;
			_xml = xml ?? string.Empty;
			_UpdateDisplay();
		}

		private void _UpdateDisplay()
		{
			if (!_reformat)
			{
				txtXml.Text = _xml;
				return;
			}

			var xDocument = XDocument.Parse(_xml);
			txtXml.Text = xDocument.ToString();
		}
	}
}
