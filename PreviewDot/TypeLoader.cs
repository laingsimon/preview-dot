using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PreviewDot
{
	internal class TypeLoader : IDisposable
	{
		private const string AssemblyName = "PreviewDot.UI";
		private readonly Dictionary<Type, object> _dependencies;
		private readonly Dictionary<Type, object> _createdObjects = new Dictionary<Type, object>();
		private AppDomain _appDomain;
		private readonly List<FileSystemWatcher> _fileSystemWatchers = new List<FileSystemWatcher>();
		private readonly Action _whenFilesChange;

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

			var installDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			_appDomain = AppDomain.CreateDomain(
				AssemblyName + ".Isolation",
				null,
				new AppDomainSetup
				{
					ShadowCopyFiles = "true",
					ApplicationBase = installDirectory,
					ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
				});
		}
	}
}
