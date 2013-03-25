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

namespace Strategy.GroupControl.RuntimeProperty {
	public class PropertyManager {
		private Dictionary<Type, object> baseDict;

		private static PropertyManager instance;

		//static - file change
		private static Session session;
		private static DateTime lastRead;

		private XmlNode root;
		private XmlNode missionPropertyNode;


		public PropertyManager(string missionName) {
			instance = this;
			baseDict = new Dictionary<Type, object>();

			FileSystemWatcher watcher = new FileSystemWatcher();
			watcher.Path = "../../Media/Mission/Scripts/";

			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
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
			XmlDocument xml = new XmlDocument();
			xml.Load("../../Media/Mission/Scripts/test.xml");
			root = xml.DocumentElement;

		}

		//File change
		private static void OnChanged(object source, FileSystemEventArgs e) {
			// Specify what is done when a file is changed, created, or deleted.
			DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
			if (lastWriteTime != lastRead) {
				//Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
				lastRead = lastWriteTime;
				session.ExecuteFile(e.FullPath);
				instance.loadProperties();

			}
		}

		/// <summary>
		/// add generic property into baseDict (or modify)
		/// </summary>
		/// <typeparam name="T">type of property</typeparam>
		/// <param name="key">name of property</param>
		/// <param name="value">value of property</param>
		private void add<T>(string key, T value) {
			Type type = typeof(T);
			Dictionary<string, Property<T>> subDict;
			if (baseDict.ContainsKey(type)) {
				subDict = (Dictionary<string, Property<T>>)baseDict[type];
				if (subDict.ContainsKey(key)) {
					subDict[key].Value=value;
				} else {
					subDict.Add(key, new Property<T>(value));
				}
			} else {
				subDict = new Dictionary<string, Property<T>>();
				subDict.Add(key, new Property<T>(value));
				baseDict.Add(type, subDict);
			}
		}


		public void setPropertyPath(string missionName) {
			session.ExecuteFile("../../Media/Mission/Scripts/Testing.csx");
			missionPropertyNode = root.SelectNodes("runTimeProperty[@missionName='" + missionName + "'][1]")[0];
			loadProperties();
		}

		public Property<T> getProperty<T>(string key) {
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

		public void loadProperties() {

			MethodInfo method = typeof(PropertyManager).GetMethod("add", BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (XmlNode property in missionPropertyNode.ChildNodes) {
				string propertyName = property.Attributes["name"].InnerText;
				object d = session.Execute(propertyName);
				Type type = d.GetType();

				MethodInfo generic = method.MakeGenericMethod(type);
				List<object> args = new List<object>();
				args.Add(propertyName);
				args.Add(d);
				generic.Invoke(this, args.ToArray());

			}
		}





	}
}
