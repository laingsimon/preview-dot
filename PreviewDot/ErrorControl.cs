using System.Windows.Forms;

namespace PreviewDot
{
    internal partial class ErrorControl : UserControl
    {
        public ErrorControl(string message)
        {
            InitializeComponent();
            txtMessage.Text = message;

            txtMessage.Text += "\r\n\r\nLog:\r\n" + Logging.ReadLog();
        }
    }
}
