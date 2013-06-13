using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using Strategy.Exceptions;

namespace Strategy.GameObjectControl.RuntimeProperty {
	/// <summary>
	/// Controls Properties loaded from given file. Also controls if the was changed, if was so 
	/// the Properties will be reloaded.
	/// </summary>
	public class PropertyManager {
		private Dictionary<Type, object> baseDict;

		private static PropertyManager instance;

		// Static - file change
		private static Session session;
		private static DateTime lastRead;
		static FileSystemWatcher watcher;

		private List<string> propertiesNameList;

		/// <summary>
		/// Initializes PropertyManager.
		/// </summary>
		public PropertyManager() {
			instance = this;
			baseDict = new Dictionary<Type, object>();

			propertiesNameList = new List<string>();
		}

		/// <summary>
		/// Controls delay between file changes (delay must be longer then 5 seconds when is less so properties are not reloaded) 
		/// The function is called when setted file is changed.
		/// </summary>
		/// <param name="source">The source of action.</param>
		/// <param name="e">The changed file arguments.</param>
		private static void OnChanged(object source, FileSystemEventArgs e) {
			DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath); // Bug fix - function is called twice
			DateTime now = DateTime.Now;
			TimeSpan ts = new TimeSpan(0, 0, 5);
			if (lastWriteTime != lastRead && (now - lastRead) > ts) {       // Reload is called once
				lastRead = lastWriteTime;
				try {
					session.ExecuteFile(e.FullPath);
				} catch (Exception exception) {
					Console.WriteLine("Script must be without errors.");
					Console.WriteLine(exception);
					return;
				}

				instance.LoadProperties();

			}
		}

		/// <summary>
		/// Adds generic property into baseDict (or modify).
		/// Properties are switched by type to sub-directories where are references saved.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="key">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		private void Add<T>(string key, T value) {
			Type type = typeof(T);
			Dictionary<string, Property<T>> subDict;
			if (baseDict.ContainsKey(type)) {
				subDict = (Dictionary<string, Property<T>>)baseDict[type];
				if (subDict.ContainsKey(key)) {
					subDict[key].Value = value;
				} else {
					subDict.Add(key, new Property<T>(value));
				}
			} else {
				subDict = new Dictionary<string, Property<T>>();
				subDict.Add(key, new Property<T>(value));
				baseDict.Add(type, subDict);
			}
			propertiesNameList.Add(key);
		}

		/// <summary>
		/// Loads properties for given mission from the file.
		/// </summary>
		/// <param name="missionName">The string with the mission file path.</param>
		public void LoadPropertyToMission(string missionPropFilePath) {
			if (watcher != null) {
				watcher.Dispose();
			}

			string[] splited = missionPropFilePath.Split('/');

			watcher = new FileSystemWatcher();
			watcher.Path = missionPropFilePath.Substring(0, missionPropFilePath.Length - 1 - splited[splited.Length - 1].Length);

			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;

			watcher.Filter = splited[splited.Length - 1];
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			// Begins watching.
			watcher.EnableRaisingEvents = true;

			// Creates scriptEngine
			var scriptEngine = new ScriptEngine();
			session = scriptEngine.CreateSession();
			session.AddReference("System");
			session.AddReference("System.Core");
			// Executes script
			session.ExecuteFile(missionPropFilePath);
			LoadProperties();
		}

		/// <summary>
		/// Returns a generic property Property. Finds Property by type and after that 
		/// by the name. If the Property does not exists so the exception is thrown.
		/// </summary>
		/// <typeparam name="T">The type of the Property.</typeparam>
		/// <param name="name">The name of the Property.</param>
		/// <returns>Returns generic instance of Property found by name.</returns>
		public Property<T> GetProperty<T>(string name) {
			if (!propertiesNameList.Contains(name)) {
				try {
					LoadProperty(name);
				} catch (Exception) {
					throw new PropertyMissingException("Missing property " + name);
				}
			}

			Type type = typeof(T);
			Dictionary<string, Property<T>> subDict;
			if (baseDict.ContainsKey(type)) {
				subDict = (Dictionary<string, Property<T>>)baseDict[type];
				if (subDict.ContainsKey(name)) {
					return subDict[name];
				}
			}
			throw new PropertyMissingException("Missing property " + name);

		}

		/// <summary>
		/// Loads the Property by run-time generic called Add.
		/// </summary>
		/// <param name="name">The name of the loading Property.</param>
		private void LoadProperty(string name) {

			// Reflection is used because here is needed runtime generic add is private function
			MethodInfo method = typeof(PropertyManager).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
			// Property from a script (int,float...)
			object d = session.Execute(name);
			Type type = d.GetType();

			MethodInfo generic = method.MakeGenericMethod(type);
			List<object> args = new List<object>();
			args.Add(name);
			args.Add(d);

			//  Calls add with type of property from script
			generic.Invoke(this, args.ToArray());
		}

		/// <summary>
		/// Loads all properties from executing file and save them as generic Property by name.
		/// </summary>
		public void LoadProperties() {

			foreach (var property in new List<string>( propertiesNameList)) {
				LoadProperty(property);
			}
		}

	}
}
