using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using Strategy.Exceptions;

namespace Strategy.GameObjectControl.RuntimeProperty {
	public class PropertyManager {
		private Dictionary<Type, object> baseDict;

		private static PropertyManager instance;
		static FileSystemWatcher watcher;

		// Static - file change
		private static Session session;
		private static DateTime lastRead;


		private List<string> propertiesNameList;




		public PropertyManager() {
			instance = this;
			baseDict = new Dictionary<Type, object>();

			propertiesNameList = new List<string>();

		}

		/// <summary>
		/// Function called when file "Properties.csx" is changed. This function controls delay between
		/// changes (delay must be longer then 5s when is less so properties are not reloaded) 
		/// </summary>
		/// <param Name="source">Source</param>
		/// <param Name="e">Changed file</param>
		private static void OnChanged(object source, FileSystemEventArgs e) {
			Console.WriteLine("Neco se deje ted");
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
		/// Add generic property into baseDict (or modify)
		/// </summary>
		/// <typeparam Name="T">Type of property</typeparam>
		/// <param Name="key">Name of property</param>
		/// <param Name="value">Value of property</param>
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
		/// Function loads properties for given mission from Property file
		/// </summary>
		/// <param Name="missionName">Mission Name</param>
		public void LoadPropertyToMission(string missionPropFilePath) {
			if (watcher != null) {
				watcher.Dispose();
			}

			string[] splited = missionPropFilePath.Split('/');

			watcher = new FileSystemWatcher();
			watcher.Path = missionPropFilePath.Substring(0, missionPropFilePath.Length - 1 - splited[splited.Length - 1].Length);

			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
			// Only watch text files.
			watcher.Filter = splited[splited.Length - 1];
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			// Begin watching.
			watcher.EnableRaisingEvents = true;

			// Create scriptEngine
			var scriptEngine = new ScriptEngine();
			session = scriptEngine.CreateSession();
			session.AddReference("System");
			session.AddReference("System.Core");
			session.ExecuteFile(missionPropFilePath);
			LoadProperties();
		}

		/// <summary>
		/// Function returns a generic property Property<T>
		/// </summary>
		/// <typeparam Name="T">Type of property</typeparam>
		/// <param Name="key">Property Name</param>
		/// <returns>Generic instance of Property found by key </returns>
		public Property<T> GetProperty<T>(string key) {
			if (!propertiesNameList.Contains(key)) {
				try {
					LoadProperty(key);
				} catch (Exception) {
					throw new PropertyMissingException("Missing property " + key);
				}
			}

			Type type = typeof(T);
			Dictionary<string, Property<T>> subDict;
			if (baseDict.ContainsKey(type)) {
				subDict = (Dictionary<string, Property<T>>)baseDict[type];
				if (subDict.ContainsKey(key)) {
					return subDict[key];
				}
			}
			throw new PropertyMissingException("Missing property " + key);

		}

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
		/// A function loads all properties in executing file and save them in generic Property by Name(key)
		/// </summary>
		public void LoadProperties() {

			foreach (var property in new List<string>( propertiesNameList)) {
				LoadProperty(property);
			}
		}

	}
}
