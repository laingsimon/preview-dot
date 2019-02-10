using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PreviewDot.ComInterop
{
	public static class Wia
	{
		public static void Print(IWin32Window owner, string fileName)
		{
			var vector = new WIA.VectorClass();
			object objFilename = fileName;
			vector.Add(ref objFilename, 0);

			var dialogClass = new WIA.CommonDialogClass();

			var form = new Form
			{
				ShowInTaskbar = false,
				FormBorderStyle = FormBorderStyle.None
			};
			form.TransparencyKey = form.BackColor;
			form.Shown += (sender, args) =>
			{
				object vectorObject = vector;
				dialogClass.ShowPhotoPrintingWizard(ref vectorObject);
				form.Close();
			};

			form.ShowDialog(owner);
			form = null;

			Marshal.ReleaseComObject(dialogClass);
			dialogClass = null;
		}
	}
}
