using System;
using System.Windows.Forms;

namespace PreviewDot
{
	internal partial class ErrorControl : UserControl
	{
		public ErrorControl(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");

			InitializeComponent();
			txtMessage.Text = exception.ToString();

			txtMessage.Text += "\r\n\r\nLog:\r\n" + Logging.ReadLog();
		}
	}
}
