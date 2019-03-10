using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PreviewDot
{
	internal class TypeLoader : MarshalByRefObject, IDisposable
	{
		private static readonly string _installDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		private const string AssemblyName = "PreviewDot.UI";
		private readonly Dictionary<Type, object> _dependencies;
		private readonly Dictionary<Type, object> _createdObjects = new Dictionary<Type, object>();
		private AppDomain _appDomain;
		private readonly List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();
		private readonly Action _whenFilesChange;

		static TypeLoader()
		{
			AppDomain.CurrentDomain.AssemblyResolve += _appDomain_AssemblyResolve;
		}

		public TypeLoader(Action whenFilesChange, params object[] dependencies)
		{
			_dependencies = dependencies.ToDictionary(d => d.GetType());
			this._whenFilesChange = whenFilesChange;
		}

		public void Dispose()
		{
			Reset();
		}

		private void Reset()
		{
			_createdObjects.Clear();
			AppDomain.Unload(_appDomain);
			_appDomain = null;

			foreach (var watcher in _fileSystemWatchers)
			{
				watcher.Dispose();
			}
			_fileSystemWatchers.Clear();
		}

		internal T Load<T>()
		{
			try
			{
				if (_createdObjects.ContainsKey(typeof(T)))
					return (T)_createdObjects[typeof(T)];

				LoadAssembly();

				string typeName = typeof(T).IsInterface
					? RemoveIPrefix(typeof(T).Name)
					: typeof(T).Name;
				var instance = _appDomain.CreateInstanceAndUnwrap(
					AssemblyName,
					AssemblyName + "." + typeName,
					false,
					BindingFlags.Default,
					null,
					new object[0],
					null,
					null);

				_createdObjects.Add(typeof(T), instance);

				return (T)instance;
			}
			catch (InvalidCastException exc)
			{
				try
				{
					Trace.TraceError("{0}: {1} :: {2}", AppDomain.CurrentDomain.FriendlyName, AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
					Trace.TraceError("{0}: {1} :: {2}", _appDomain.FriendlyName, _appDomain.BaseDirectory, _appDomain.SetupInformation.ApplicationBase);

					throw new InvalidCastException(exc.Message + "\n\n" + GetLoadedAssemblies(AppDomain.CurrentDomain) + "\n\n" + GetLoadedAssemblies(_appDomain), exc);
				}
				finally
				{
					Reset();
				}
			}
		}

		private static string GetLoadedAssemblies(AppDomain appDomain)
		{
			return string.Join("\n", appDomain.GetAssemblies()
				.Select(a => GetAssemblyDetail(a)));
		}

		private static string GetAssemblyDetail(Assembly assembly)
		{
			try
			{
				return string.Format("{0}: {1}", assembly.GetName().Name, assembly.Location);
			}
			catch (Exception exc)
			{
				return assembly.ToString() + "[" + exc.Message + "]";
			}
		}

		private string RemoveIPrefix(string name)
		{
			if (name.StartsWith("I"))
				return name.Substring(1);

			return name;
		}

		private void LoadAssembly()
		{
			CreateDomain();

			var watcher = new FileSystemWatcher(_appDomain.BaseDirectory)
			{
				EnableRaisingEvents = true,
				Filter = "*.dll",
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
			};
			watcher.Changed += Watcher_Changed;
			_fileSystemWatchers.Add(watcher);
		}

		private void Watcher_Changed(object sender, FileSystemEventArgs e)
		{
			Reset();
			_whenFilesChange();
		}

		private void CreateDomain()
		{
			if (_appDomain != null)
				return;

			Trace.TraceInformation("Main domain ({0}) assemblies: {1}", AppDomain.CurrentDomain, GetLoadedAssemblies(AppDomain.CurrentDomain));

			_appDomain = AppDomain.CreateDomain(
				AssemblyName + ".Isolation",
				null,
				new AppDomainSetup
				{
					ShadowCopyFiles = "true",
					ApplicationBase = _installDirectory,
					ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
					ShadowCopyDirectories = "true",
				});

			_appDomain.AssemblyResolve += _appDomain_AssemblyResolve;
		}

		private static Assembly _appDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Trace.TraceError("{0}: Unable to resolve assembly {1} for {2}", AppDomain.CurrentDomain.FriendlyName, args.Name, args.RequestingAssembly);
			return null;
		}
	}
}
