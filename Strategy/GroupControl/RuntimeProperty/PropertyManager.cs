using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using Strategy.Exceptions;

namespace Strategy.GroupControl.RuntimeProperty {
	class PropertyManager {
		private Dictionary<Type, object> baseDict;

		//static - file change
		private static Session session;
		private static DateTime lastRead;

		public PropertyManager(string missionName) {
			baseDict = new Dictionary<Type, object>();

			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path ="../../Media/Mission/Scripts/";

			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName ;
			// Only watch text files.
			watcher.Filter = "Testing.csx";
			watcher.Changed += new FileSystemEventHandler(OnChanged);
			// Begin watching.
			watcher.EnableRaisingEvents = true;

			//create scriptEngine
			var scriptEngine = new ScriptEngine();
			session = scriptEngine.CreateSession();
			session.AddReference("System");
			session.AddReference("System.Core");
			//create start properties - load XML file with used ones
			
		}

		private void add<T>(string key, T value) {
			Type type = typeof(T);
			Dictionary<string, Property<T>> subDict;
			if (baseDict.ContainsKey(type)) {
				subDict = (Dictionary<string, Property<T>>)baseDict[type];
				if (subDict.ContainsKey(key)) {
					subDict[key].setValue(value);
				} else {
					subDict.Add(key, new Property<T>(value));
				}
			} else {
				subDict = new Dictionary<string, Property<T>>();
				subDict.Add(key, new Property<T>(value));
				baseDict.Add(type, subDict);
			}
		}

		public Property<T> getValue<T>(string key) {
			Type type = typeof(T);
			Dictionary<string, Property<T>> subDict;
			if(baseDict.ContainsKey(type)){
				subDict = (Dictionary<string, Property<T>>)baseDict[type];
				if (subDict.ContainsKey(key)) {
					return subDict[key];
				}
			}
			throw new PropertyMissingException("Missing property "+ key);
		}

		public void loadProperties(string missionName) {
			//TODO Property load
		}
		
		//File change
		private static void OnChanged(object source, FileSystemEventArgs e) {
			// Specify what is done when a file is changed, created, or deleted.
			DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
			if (lastWriteTime != lastRead) {
				Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
				lastRead = lastWriteTime;
				//engine.
				session.ExecuteFile(e.FullPath);
				int i = session.Execute<int>("a");
				Console.WriteLine(i);

			}
		}



	}
}
