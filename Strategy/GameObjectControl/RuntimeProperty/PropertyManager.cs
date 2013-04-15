﻿using System;
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
			watcher.Filter = "Properties.csx";
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

		/// <summary>
		/// Function called when file "Properties.csx" is changed. This function controls delay between
		/// changes (delay must be longer then 5s when is less so properties are not reloaded) 
		/// </summary>
		/// <param name="source">Source</param>
		/// <param name="e">Changed file</param>
		private static void OnChanged(object source, FileSystemEventArgs e) {
			DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath); //Bug fix - function is called twice
			DateTime now = DateTime.Now;
			TimeSpan ts = new TimeSpan(0, 0, 5);
			if (lastWriteTime != lastRead && (now-lastRead)>ts) {       //reload is called once
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

		/// <summary>
		/// A function loads properties for given mission from Property file
		/// </summary>
		/// <param name="missionName">mission name</param>
		public void loadPropertyToMission(string missionName) {
			session.ExecuteFile("../../Media/Mission/Scripts/Properties.csx");
			missionPropertyNode = root.SelectNodes("runTimeProperty[@missionName='" + missionName + "'][1]")[0];
			loadProperties();
		}

		/// <summary>
		/// A function returns a generic property Property<T>
		/// </summary>
		/// <typeparam name="T">type of property</typeparam>
		/// <param name="key">property name</param>
		/// <returns>generic instance of Property found by key </returns>
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

		/// <summary>
		/// A function loads all properties in executing file and save them in generic Property by name(key)
		/// </summary>
		public void loadProperties() {
			//reflection is used because here is needed runtime generic 
			MethodInfo method = typeof(PropertyManager).GetMethod("add", BindingFlags.NonPublic | BindingFlags.Instance); //add is private function
			foreach (XmlNode property in missionPropertyNode.ChildNodes) {
				string propertyName = property.Attributes["name"].InnerText;
				object d = session.Execute(propertyName);					//property from a script (int,float...)
				Type type = d.GetType();

				MethodInfo generic = method.MakeGenericMethod(type);		
				List<object> args = new List<object>();
				args.Add(propertyName);
				args.Add(d);
				generic.Invoke(this, args.ToArray());						//calls add with type of property from script

			}
		}

	}
}