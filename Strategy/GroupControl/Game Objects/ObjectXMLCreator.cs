using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Mogre;
using Strategy.Exceptions;
using Strategy.GameMaterial;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using System.IO;
using System.Reflection;
using System.Linq;
using Roslyn.Scripting.CSharp;
using System.Reflection.Emit;
using System.Linq.Expressions;
using Roslyn.Scripting;
using Strategy.GroupControl.RuntimeProperty;


namespace Strategy.GroupControl.Game_Objects {
	class ObjectXMLCreator {

		protected XmlDocument xml;
		protected Mogre.SceneManager manager;
		protected Dictionary<string, Team> teamDict;
		protected List<IMaterial> materialList;
		protected List<SolarSystem> solarSystemList;
		protected Dictionary<Team, List<Team>> teamRealationDict;
		protected XmlElement root;

		//Property manager
		private PropertyManager propMgr;

		//assembly load
		protected AssemblyBuilder assemblyBuilder;
		protected ModuleBuilder moduleBuilder;
		List<MetadataReference> metadataRef;
		CompilationOptions comilationOption;
		List<string> isCompiled;

		public ObjectXMLCreator(string path, Mogre.SceneManager manager, Dictionary<string, Team> teams,
			List<IMaterial> materialList, List<SolarSystem> solarSystems) {
			teamRealationDict = new Dictionary<Team, List<Team>>();

			this.manager = manager;
			this.teamDict = teams;
			this.materialList = materialList;
			this.solarSystemList = solarSystems;
			xml = new XmlDocument();
			xml.Load(path);
			root = xml.DocumentElement;


			isCompiled = new List<string>();
			var t = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			metadataRef = new List<MetadataReference>();
			metadataRef.Add(MetadataFileReference.CreateAssemblyReference("mscorlib"));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.MyMogre).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.Game).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(GroupControl.Game_Objects.StaticGameObjectBox.StaticGameObject).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Team).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(System.Linq.Enumerable).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(LinkedList<>).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(Path.GetFullPath((new Uri(t + "\\\\Mogre.dll")).LocalPath)));
			metadataRef.Add(new MetadataFileReference(typeof(PropertyManager).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(GroupControl.Game_Objects.StaticGameObjectBox.IStaticGameObject).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.Game).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(ActionReason).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(ActionAnswer).Assembly.Location));

			comilationOption = new CompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly" + Guid.NewGuid()),
															  AssemblyBuilderAccess.RunAndCollect);
			moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
		}

		public void load(string missionName, PropertyManager propMan) {
			propMgr = propMan;									//TODO thinks about try-block
			propMan.loadPropertyToMission(missionName);
			bool hasSun = false;
			IStaticGameObject sun = null;
			XmlNode missionNode = root.SelectNodes("/mission[@name='" + missionName + "'][1]")[0]; //load mission (first of given name)

			loadTeams(root.SelectNodes("teams[1]")[0]);

			XmlNode missionSolarSystemNode = missionNode.SelectNodes("solarSystems[1]")[0];
			foreach (XmlNode solarSystem in missionSolarSystemNode) {
				List<IStaticGameObject> isgos = new List<IStaticGameObject>();
				List<IMovableGameObject> imgos = new List<IMovableGameObject>();
				string gameObjectType;
				string solarSystemName = "";
				foreach (XmlNode chldNode in solarSystem.Attributes) {
					switch (chldNode.Name) {
						case "gate":
							var gate = createGate(solarSystemName);
							isgos.Add(gate);
							break;
						case "name":
							solarSystemName = chldNode.InnerText;
							break;
					}
				}

				foreach (XmlNode gameObject in solarSystem.ChildNodes) {
					switch (gameObject.Name) {
						case "isgo":
							IsgoType t;
							gameObjectType = gameObject.Attributes["type"].InnerText;
							if (gameObjectType == "Sun") {
								hasSun = true;
								t = IsgoType.Sun;
								sun = createISGO(gameObject, missionNode.SelectNodes("usedObjects/isgos/sgo[@name='Sun']")[0], t);

							} else {
								t = IsgoType.StaticObject;
								IStaticGameObject isgo = createISGO(gameObject,
									missionNode.SelectNodes("usedObjects/isgos/sgo[@name='" + gameObjectType + "']")[0],
									t
									);
								isgo.Team.addISGO(isgo);
								isgos.Add(isgo);
								//here register
								readSGOActions(gameObject.SelectNodes("gameAction"), (StaticGameObject)isgo);
								break;
							}
							break;
						case "imgo":
							gameObjectType = gameObject.Attributes["type"].InnerText;
							IMovableGameObject imgo = createIMGO(gameObject,
								missionNode.SelectNodes("usedObjects/imgos/mgo[@name='" + gameObjectType + "']")[0]);
							imgo.Team.addIMGO(imgo);
							imgos.Add(imgo);
							break;
						default:
							throw new XmlLoadException("Bad XML format. In SolarSystem cannot be node " + gameObject.Name);
					}
				}

				if (hasSun) {
					this.solarSystemList.Add(createSolarSystem(solarSystemName, parseStringToVector3(solarSystem.Attributes["position"].Value), isgos, imgos, sun));
					hasSun = false;
				} else {
					this.solarSystemList.Add(createSolarSystem(solarSystemName, parseStringToVector3(solarSystem.Attributes["position"].Value), isgos, imgos));
				}

			}
		}

		/// <summary>
		/// Function load names of teams from XML file
		/// </summary>
		/// <param name="teamsNode">XML node with teams and frienships</param>
		private void loadTeams(XmlNode teamsNode) {
			var t = new Team("None", materialList);		//add None team for suns and gates
			teamDict.Add(t.Name, t);
			foreach (XmlNode node in teamsNode.ChildNodes) {
				List<Team> friends = new List<Team>();
				foreach (XmlNode teamName in node.ChildNodes) {
					foreach (XmlNode att in teamName.Attributes) {
						if (att.Name == "name") {

							t = new Team(att.Value, materialList);
							teamDict.Add(t.Name, t);
							friends.Add(t);
						}
					}
				}
				//friends.Add(teamDict["None"]);
				var listWithNone = new List<Team>(friends);
				listWithNone.Add(teamDict["None"]);
				foreach (var team in friends) {

					teamRealationDict.Add(team, listWithNone);
				}

			}

		}

		public Dictionary<Team, List<Team>> getTeamsRelations() {
			return teamRealationDict;
		}

		/// <summary>
		/// Function runtime creates object with parameters and relative path from XML file.
		/// The function also checks if class is already compiled or not. If not then function
		/// compile it.
		/// </summary>
		/// <param name="gameObjectPath">XML node with object parameters</param>
		/// <param name="type">class type in string (check if is compiled)</param>
		/// <param name="args">class arguments for constructor</param>
		/// <returns></returns>
		private object createGameObject(XmlNode gameObjectPath, string type, object[] args) {
			string fullPath = gameObjectPath.Attributes["path"].InnerText;
			string fullName = gameObjectPath.Attributes["fullName"].InnerText;


			if (!isCompiled.Contains(type)) {
				isCompiled.Add(type);
				var syntaxTree = SyntaxTree.ParseFile("../../GroupControl/Game Objects/" + fullPath);

				var comp = Compilation.Create("Test.dll"
					, syntaxTrees: new[] { syntaxTree }
					, references: metadataRef
					, options: comilationOption
					);

				var result = comp.Emit(moduleBuilder);				//runtime compilation and check errors
				if (!result.Success) {
					foreach (var d in result.Diagnostics) {
						Console.WriteLine(d);
					}
					throw new XmlLoadException("Class not found " + fullPath);
				}
			}
			var o = moduleBuilder.GetType(fullName);

			object runTimeObject;
			runTimeObject = Activator.CreateInstance(o, args);
			return runTimeObject;
		}

		/// <summary>
		/// Function creates IMovableGameObject from XML nodes. 
		/// </summary>
		/// <param name="gameObject">XmlNode with parameters for the instance</param>
		/// <param name="gameObjectPath">XmlNode with class path and name</param>
		/// <returns>instance implmements IMovableGameObject (specific in gameObjectPath)</returns>
		private IMovableGameObject createIMGO(XmlNode gameObject, XmlNode gameObjectPath) {
			IMovableGameObject imgo;
			string type;
			List<object> args = new List<object>();
			args.Add(gameObject.Attributes["name"].InnerText);
			args.Add(gameObject.Attributes["mesh"].InnerText);

			string team = gameObject.Attributes["team"].InnerText;
			type = gameObject.Attributes["type"].InnerText;
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			args.Add(teamDict[team]);
			args.Add(manager);

			args.Add(parseStringToVector3(gameObject.Attributes["position"].InnerText));
			args.Add(propMgr);
			imgo = (IMovableGameObject)createGameObject(gameObjectPath, type, args.ToArray());
			return imgo;
		}


		/// <summary>
		/// Function creates IStaticGameObject from XML nodes. 
		/// </summary>
		/// <param name="gameObject">XmlNode with parameters for the instance</param>
		/// <param name="gameObjectPath">XmlNode with class path and name</param>
		/// <param name="isgoType">Type of creating instance for check special type Sun</param>
		/// <param name="pointsOnCircle">number of positions on imaginary circle</param>
		/// <returns>instance implmements IStaticGameObject (specific in gameObjectPath)</returns>
		private IStaticGameObject createISGO(XmlNode gameObject, XmlNode gameObjectPath, IsgoType isgoType, int pointsOnCircle = 30) {
			IStaticGameObject isgo;
			string type;
			List<object> args = new List<object>();
			args.Add(gameObject.Attributes["name"].InnerText);
			args.Add(gameObject.Attributes["mesh"].InnerText);
			string team = gameObject.Attributes["team"].InnerText;
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			if (isgoType == IsgoType.Sun) {
				args.Add(manager);
				type = IsgoType.Sun.ToString();
				args.Add(teamDict[team]);
			} else {
				type = gameObject.Attributes["type"].InnerText;
				args.Add(teamDict[team]);
				args.Add(manager);
				string distance = gameObject.Attributes["distance"].InnerText;
				args.Add(Int32.Parse(distance));
				args.Add(parseStringToVector3(gameObject.Attributes["position"].InnerText));
				args.Add(propMgr);
				args.Add(pointsOnCircle);

			}
			isgo = (IStaticGameObject)createGameObject(gameObjectPath, type, args.ToArray());
			return isgo;
		}

		/// <summary>
		/// Creates special type Gate (is not runtime compiled). 
		/// </summary>
		/// <param name="solarSystName">Name of SolarSystem where will be</param>
		/// <returns>instance of Gate</returns>
		private Gate createGate(string solarSystName) {
			string team = "None";
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			var gate = new Gate("Gate " + solarSystName,
				"gate.mesh",
				manager,
				new Vector3(3000, 0, 3000),
				teamDict[team]);
			gate.Team.addISGO(gate);
			return gate;
		}

		/// <summary>
		/// Function brings together objects in same SolarSystem (All in one XmlNode "solarSystem")
		/// </summary>
		/// <param name="name">Name of SolarSystem</param>
		/// <param name="position">SolarSystem position in space (for count dostance between others)</param>
		/// <param name="isgoObjects">All IStaticGameObjects in this SolarSystem</param>
		/// <param name="imgoObjects">All IMovableGameObjects in this SolarSystem</param>
		/// <param name="sun">SolarSystem's Sun - can be null</param>
		/// <returns>instance of SolarSystem</returns>
		private SolarSystem createSolarSystem(string name, Vector3 position, List<IStaticGameObject> isgoObjects, List<IMovableGameObject> imgoObjects,
			IStaticGameObject sun = null) {

			SolarSystem sSys = new SolarSystem(name, position);
			sSys.addISGO(isgoObjects);
			sSys.addIMGO(imgoObjects);
			sSys.setSun(sun);
			return sSys;
		}

		private void registerSGOaction(StaticGameObject sgo, string action, string value) {
			sgo.registerExecuter(action, sgo.Team.getMaterials(), value);
		}

		private void readSGOActions(XmlNodeList actionList, StaticGameObject sgo) {
			foreach (XmlNode action in actionList) {
				registerSGOaction(sgo, action.Attributes["name"].Value, action.Attributes["value"].Value);
			}
		}

		/// <summary>
		/// Parse Mogre.Vector3 from string
		/// </summary>
		/// <param name="input">string with vector</param>
		/// <returns>Mogre.Vector3 parsed from given string</returns>
		private Mogre.Vector3 parseStringToVector3(string input) {
			string[] splitted = input.Split(',');
			Mogre.Vector3 v;
			try {
				v = new Vector3(Int32.Parse(splitted[0]), Int32.Parse(splitted[1]), Int32.Parse(splitted[2]));
			} catch (Exception) {
				throw new FormatException("Cannot parse string "+ input+ " to Mogre.Vector3. Given string was in a bad format (right format: \"x,y,z\")");
			}
			return v;
		}

	}

}
