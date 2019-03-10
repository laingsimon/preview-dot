using PreviewDot.ComInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreviewDot.TestLauncher
{
	public partial class Form1 : Form
	{
		private PreviewHandlerController _controller;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			_controller = new PreviewHandlerController();

			var rect = RECT.FromRectangle(this.ClientRectangle);
			PreviewHandler.SetWindow(this.Handle, rect);

			var stream = new FileIStream(Environment.GetCommandLineArgs()[1]);
			InitialiseWithStream.Initialize(stream, 0);

			Task.Delay(500).ContinueWith(_ => this.Invoke(new MethodInvoker(() => _controller.DoPreview())));
		}

		private IInitializeWithStream InitialiseWithStream
		{
			get { return _controller as IInitializeWithStream; }
		}

		private IPreviewHandler PreviewHandler
		{
			get { return _controller as IPreviewHandler; }
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_controller.Unload();
		}

		private class FileIStream : IStream
		{
			private readonly FileStream _stream;

			public FileIStream(string filePath)
			{
				_stream = new FileStream(filePath, FileMode.Open);
			}

			public void Read(byte[] pv, int cb, IntPtr pcbRead)
			{
				int read = _stream.Read(pv, 0, cb);
				/*TODO: update pcbRead to report <read> bytes, pcbRead += read doesn't work*/
			}

			public void Write(byte[] pv, int cb, IntPtr pcbWritten)
			{
				throw new NotImplementedException();
			}

			public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
			{
				throw new NotImplementedException();
			}

			public void SetSize(long libNewSize)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
			{
				throw new NotImplementedException();
			}

			public void Commit(int grfCommitFlags)
			{
				throw new NotImplementedException();
			}

			public void Revert()
			{
				throw new NotImplementedException();
			}

			public void LockRegion(long libOffset, long cb, int dwLockType)
			{
				throw new NotImplementedException();
			}

			public void UnlockRegion(long libOffset, long cb, int dwLockType)
			{
				throw new NotImplementedException();
			}

			public void Stat(out STATSTG pstatstg, int grfStatFlag)
			{
				pstatstg = new STATSTG
				{
					pwcsName = _stream.Name,
					mtime = new FILETIME
					{
						dwHighDateTime = 0
					},
					cbSize = _stream.Length
				};
			}

			public void Clone(out IStream ppstm)
			{
				throw new NotImplementedException();
			}
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			RECT rect = RECT.FromRectangle(ClientRectangle);
			PreviewHandler.SetRect(ref rect);
		}
	}
}
