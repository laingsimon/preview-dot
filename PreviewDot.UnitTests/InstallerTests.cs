using System;
using System.Diagnostics;
using NUnit.Framework;

namespace PreviewDot.UnitTests
{
	[TestFixture, Explicit]
	public class InstallerTests
	{
		private string _assemblyPath;
		private string _registerApplicationPath;

		[TestFixtureSetUp]
		public void SetupOnce()
		{
			var codeBase = new Uri(typeof(Installer).Assembly.CodeBase);
			_assemblyPath = codeBase.AbsolutePath.Replace("/", "\\").Replace(".UnitTests", "");
			_registerApplicationPath = "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\regasm.exe";
		}

		[Test]
		public void ShouldInstallSuccessfully()
		{
			var exitCode = _ExecuteRegisterApplication("\"" + _assemblyPath + "\" /codebase");

			if (exitCode != 0)
				Assert.Fail("Failed to install - " + exitCode);
		}

		[Test]
		public void ShouldUninstallSuccessfully()
		{
			var exitCode = _ExecuteRegisterApplication("\"" + _assemblyPath + "\" /unregister");

			if (exitCode != 0)
				Assert.Fail("Failed to uninstall - " + exitCode);
		}

		private int _ExecuteRegisterApplication(string arguments)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo(_registerApplicationPath, arguments)
				{
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				},
				EnableRaisingEvents = true
			};
			process.ErrorDataReceived += (sender, args) => Trace.TraceError(args.Data.Trim());
			process.OutputDataReceived += (sender, args) => Trace.TraceInformation(args.Data.Trim());

			Trace.TraceInformation("Starting: " + process.StartInfo.FileName + " " + process.StartInfo.Arguments);

			process.Start();
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();
			return process.ExitCode;
		}
	}
}
