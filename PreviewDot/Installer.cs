using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PreviewDot
{
	[RunInstaller(true)]
	// ReSharper disable UnusedMember.Global
	// ReSharper disable ClassNeverInstantiated.Global
	public class Installer : System.Configuration.Install.Installer
	// ReSharper restore ClassNeverInstantiated.Global
	// ReSharper restore UnusedMember.Global
	{
		public const string ApplicationId = "b7ae83a1-5834-4ca5-b52e-84682fe187ba";
		private const string _previewHandlerClassId = "{8895b1c6-b41f-4c1c-a562-0d564250836f}";
		private const string _componentClassId = "{47D4602D-23A5-44F4-89AA-4634334830E6}";

		public override void Install(IDictionary stateSaver)
		{
			try
			{
				base.Install(stateSaver);

				var registrationServices = new RegistrationServices();
				if (!registrationServices.RegisterAssembly(GetType().Assembly, AssemblyRegistrationFlags.SetCodeBase))
					throw new InstallException("Failed to register for COM interop.");
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
				throw;
			}
		}

		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);

			var registrationServices = new RegistrationServices();
			if (!registrationServices.UnregisterAssembly(GetType().Assembly))
				throw new InstallException("Failed to unregister for COM interop.");
		}

		internal static void RegisterPreviewHandler(string name, Type previewerType)
		{
			var previewTypeClassId = previewerType.GUID.ToString("B");

			// Add preview handler to preview handler list
			using (var handlersKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PreviewHandlers", true))
				handlersKey.SetValue(previewTypeClassId, name, RegistryValueKind.String);

			// Modify preview handler registration
			using (var clsidKey = Registry.ClassesRoot.OpenSubKey("CLSID"))
			using (var idKey = clsidKey.OpenSubKey(previewTypeClassId, true))
			{
				idKey.SetValue("DisplayName", name, RegistryValueKind.String);
				idKey.SetValue("AppID", "{6d2b5079-2f0b-48dd-ab7f-97cec514d30b}", RegistryValueKind.String); //see https://msdn.microsoft.com/en-us/library/windows/desktop/cc144144(v=vs.85).aspx
			}

			Trace.WriteLine("Registering extension '.xml' with previewer '" + previewTypeClassId + "'");

			// Set preview handler for specific extension
			using (var extensionKey = Registry.ClassesRoot.CreateSubKey(".xml"))
			using (var shellexKey = extensionKey.CreateSubKey("shellex"))
			using (var previewKey = shellexKey.CreateSubKey(_previewHandlerClassId))
			{
				previewKey.SetValue(null, previewTypeClassId, RegistryValueKind.String);
			}
		}

		internal static void UnregisterPreviewHandler(Type previewerType)
		{
			var previewTypeClassId = previewerType.GUID.ToString("B");

			Trace.WriteLine("Unregistering extension '.xml' with previewer '" + previewTypeClassId + "'");
			using (var shellexKey = Registry.ClassesRoot.OpenSubKey(".xml\\shellex", true))
			{
				try { shellexKey.DeleteSubKey(_previewHandlerClassId); }
				catch { }
			}

			using (var appIdsKey = Registry.ClassesRoot.OpenSubKey("AppID", true))
			{
				try { appIdsKey.DeleteSubKey(_componentClassId); }
				catch { }
			}

			using (var classesKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PreviewHandlers", true))
			{
				try { classesKey.DeleteValue(previewTypeClassId); }
				catch { }
			}
		}
	}
}
