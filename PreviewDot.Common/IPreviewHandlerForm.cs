using System;
using System.Windows.Forms;

namespace PreviewDot.Common
{
	public interface IPreviewHandlerForm
	{
		void Reset();
		object Invoke(Delegate toInvoke);
		void Show();
		bool Focus();
		IntPtr Handle { get; }
		bool PreProcessMessage(ref Message message);
		PreviewContext GetContext();
	}
}
