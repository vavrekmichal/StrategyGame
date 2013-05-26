using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using Mogre;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Strategy.Exceptions;
using Strategy.GameMaterial;
using Strategy.GameObjectControl.Game_Objects.Bullet;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;


namespace Strategy.GameObjectControl.Game_Objects {
	public class ObjectLoader {

		private Dictionary<string, int> usedNameDict;

		private XmlDocument xml;
		private Dictionary<string, Team> teamDict;
		private List<SolarSystem> solarSystemList;
		private Dictionary<Team, List<Team>> teamRealationDict;
		private XmlElement root;
		XmlNode missionNode; // Selected mission

		// Assembly Load
		private AssemblyBuilder assemblyBuilder;
		private ModuleBuilder moduleBuilder;
		private List<MetadataReference> metadataRef;
		private CompilationOptions comilationOption;
		private List<string> isCompiled;

		public ObjectLoader(string path, Dictionary<string, Team> teams,
			List<SolarSystem> solarSystems) {
			teamRealationDict = new Dictionary<Team, List<Team>>();
			usedNameDict = new Dictionary<string, int>();

			this.teamDict = teams;
			this.solarSystemList = solarSystems;
			xml = new XmlDocument();
			xml.Load(path);
			root = xml.DocumentElement;

			// Set reference to runtime compiling
			isCompiled = new List<string>();
			var t = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			metadataRef = new List<MetadataReference>();
			metadataRef.Add(MetadataFileReference.CreateAssemblyReference("mscorlib"));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.MyMogre).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.Game).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(GameObjectControl.Game_Objects.StaticGameObjectBox.StaticGameObject).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Team).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(System.Linq.Enumerable).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(LinkedList<>).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(Path.GetFullPath((new Uri(t + "\\\\Mogre.dll")).LocalPath)));
			metadataRef.Add(new MetadataFileReference(typeof(PropertyManager).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(GameObjectControl.Game_Objects.StaticGameObjectBox.IStaticGameObject).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(Strategy.Game).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(ActionReason).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(ActionAnswer).Assembly.Location));
			metadataRef.Add(new MetadataFileReference(typeof(PropertyEnum).Assembly.Location));
			//metadataRef.Add(new MetadataFileReference(typeof(Missile2).Assembly.Location));


			comilationOption = new CompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly" + Guid.NewGuid()),
															  AssemblyBuilderAccess.RunAndCollect);
			moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");

		}

		/// <summary>
		/// Compiles all given classes in xml file.
		/// </summary>
		/// <param name="root">XmlNode with names and paths to compiling classes</param>
		private void CompileUsedObjects(XmlNode root) {
			var synatexTreeList = new List<SyntaxTree>();

			foreach (XmlNode bullets in root.ChildNodes) {

				foreach (XmlNode bullet in bullets) {
					string fullPath = bullet.Attributes["path"].InnerText;
					string type = bullet.Attributes["name"].InnerText; ;
					if (!isCompiled.Contains(type)) {
						isCompiled.Add(type);
						var syntaxTree = SyntaxTree.ParseFile("../../GameObjectControl/Game Objects/" + fullPath);

						synatexTreeList.Add(syntaxTree);
					}
				}
			} 
			var comp = Compilation.Create("Test.dll"
							 , syntaxTrees: synatexTreeList
							 , references: metadataRef
							 , options: comilationOption
							 );

			// Runtime compilation and check errors
			var result = comp.Emit(moduleBuilder);

			if (!result.Success) {
				foreach (var d in result.Diagnostics) {
					Console.WriteLine(d);
				}
				throw new XmlLoadException("Compilation failed ");
			}
		}


		public void Load(string missionName) {
			//TODO thinks about try-block
			Game.PropertyManager.LoadPropertyToMission(missionName);
			bool hasSun = false;
			IStaticGameObject sun = null;

			// Load mission (first of given Name)
			missionNode = root.SelectNodes("/mission[@name='" + missionName + "'][1]")[0];

			LoadTeams(missionNode.SelectNodes("teams[1]")[0]);


			//pokus
			CompileUsedObjects(missionNode.SelectNodes("usedObjects[1]")[0]);

			// Load IBullets
			//CompileIBullets(missionNode.SelectNodes("usedObjects//ibullets[1]")[0]);

			// Load every IGameObject
			XmlNode missionSolarSystemNode = missionNode.SelectNodes("solarSystems[1]")[0];
			foreach (XmlNode solarSystem in missionSolarSystemNode) {
				List<IStaticGameObject> isgos = new List<IStaticGameObject>();
				List<IMovableGameObject> imgos = new List<IMovableGameObject>();
				string gameObjectType;
				string solarSystemName = "";
				foreach (XmlNode chldNode in solarSystem.Attributes) {
					switch (chldNode.Name) {
						case "gate":
							var gate = CreateGate(solarSystemName);
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
								sun = CreateISGO(gameObject, t);

							} else {
								t = IsgoType.StaticObject;
								IStaticGameObject isgo = CreateISGO(gameObject, t);
								isgo.Team.AddISGO(isgo);
								isgos.Add(isgo);
								//here register
								ReadSGOActions(gameObject.SelectNodes("gameAction"), (StaticGameObject)isgo);
								break;
							}
							break;
						case "imgo":
							gameObjectType = gameObject.Attributes["type"].InnerText;
							IMovableGameObject imgo = CreateIMGO(gameObject);
							imgo.Team.AddIMGO(imgo);
							imgos.Add(imgo);
							break;
						default:
							throw new XmlLoadException("Bad XML format. In SolarSystem cannot be node " + gameObject.Name);
					}
				}

				if (hasSun) {
					this.solarSystemList.Add(CreateSolarSystem(solarSystemName,
						ParseStringToVector3(solarSystem.Attributes["position"].Value), isgos, imgos, sun));
					hasSun = false;
				} else {
					this.solarSystemList.Add(CreateSolarSystem(solarSystemName,
						ParseStringToVector3(solarSystem.Attributes["position"].Value), isgos, imgos));
				}

			}
		}

		/// <summary>
		/// Function Load names of teams from XML file
		/// </summary>
		/// <param Name="teamsNode">XML node with teams and frienships</param>
		private void LoadTeams(XmlNode teamsNode) {

			// Add None team for suns and gates
			var t = new Team("None");
			var noneList = new List<Team>();
			noneList.Add(t);
			teamRealationDict.Add(t, noneList);
			teamDict.Add(t.Name, t);
			foreach (XmlNode node in teamsNode.ChildNodes) {
				List<Team> friends = new List<Team>();
				foreach (XmlNode teamName in node.ChildNodes) {
					foreach (XmlNode att in teamName.Attributes) {
						if (att.Name == "name") {

							t = new Team(att.Value);
							teamDict.Add(t.Name, t);
							friends.Add(t);
						}
					}
				}
				var listWithNone = new List<Team>(friends);
				listWithNone.Add(teamDict["None"]);
				foreach (var team in friends) {

					teamRealationDict.Add(team, listWithNone);
				}

			}

		}

		public Dictionary<Team, List<Team>> GetTeamsRelations() {
			return teamRealationDict;
		}

		/// <summary>
		/// Function runtime creates object with parameters and relative path from XML file.
		/// The function also checks if class is already compiled or not. If not then function
		/// compile it.
		/// </summary>
		/// <param Name="gameObjectPath">XML node with object parameters</param>
		/// <param Name="type">Class type in string (check if is compiled)</param>
		/// <param Name="args">Class arguments for constructor</param>
		/// <returns></returns>
		private object CreateGameObject(string type, object[] args) {
			XmlNode gameObjectPath = GetTypeNode(type);
			string fullPath = gameObjectPath.Attributes["path"].InnerText;
			string fullName = gameObjectPath.Attributes["fullName"].InnerText;


			if (!isCompiled.Contains(type)) {
				throw new XmlLoadException("Unknown type " + fullPath);
			}
			var o = moduleBuilder.GetType(fullName);

			object runTimeObject;
			runTimeObject = Activator.CreateInstance(o, args);
			return runTimeObject;
		}

		/// <summary>
		/// Function creates IMovableGameObject from XML nodes. 
		/// </summary>
		/// <param Name="gameObject">XmlNode with parameters for the instance</param>
		/// <param Name="gameObjectPath">XmlNode with class path and Name</param>
		/// <returns>Instance implmements IMovableGameObject (specific in gameObjectPath)</returns>
		private IMovableGameObject CreateIMGO(XmlNode gameObject) {

			List<object> args = new List<object>();
			args.Add(GetUnusedName(gameObject.Attributes["name"].InnerText));
			args.Add(gameObject.Attributes["mesh"].InnerText);

			string team = gameObject.Attributes["team"].InnerText;
			string type = gameObject.Attributes["type"].InnerText;
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			args.Add(teamDict[team]);
			args.Add(ParseStringToVector3(gameObject.Attributes["position"].InnerText));

			return CreateIMGO(type, args.ToArray());
		}

		/// <summary>
		/// Function creates IMovableGameObject from given arguments and string with specific type of IMovableGameObject.
		/// </summary>
		/// <param Name="type">specific type of IMovableGameObject in string</param>
		/// <param Name="args">arguments for object constructor</param>
		/// <returns>Instance of IMovableGameObject (specific in type)</returns>
		public IMovableGameObject CreateIMGO(string type, object[] args) {
			IMovableGameObject imgo = (IMovableGameObject)CreateGameObject(type, args);
			return imgo;
		}


		/// <summary>
		/// Function creates IStaticGameObject from XML nodes. It gets arguments from XmlNode and calls CreateISGO(string type, object[] args) 
		/// to get created instance of IStaticGameObject.
		/// </summary>
		/// <param Name="gameObject">XmlNode with parameters for the instance</param>
		/// <param Name="gameObjectPath">XmlNode with class path and Name</param>
		/// <param Name="isgoType">Type of creating instance for check special type Sun</param>
		/// <param Name="pointsOnCircle">Number of positions on imaginary circle</param>
		/// <returns>Instance implmements IStaticGameObject (specific in gameObjectPath)</returns>
		private IStaticGameObject CreateISGO(XmlNode gameObject, IsgoType isgoType, int pointsOnCircle = 30) {
			string type;
			List<object> args = new List<object>();
			args.Add(GetUnusedName(gameObject.Attributes["name"].InnerText));
			args.Add(gameObject.Attributes["mesh"].InnerText);
			string team = gameObject.Attributes["team"].InnerText;
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			if (isgoType == IsgoType.Sun) {
				type = IsgoType.Sun.ToString();
				args.Add(teamDict[team]);
			} else {
				type = gameObject.Attributes["type"].InnerText;
				args.Add(teamDict[team]);
				string distance = gameObject.Attributes["distance"].InnerText;
				args.Add(Int32.Parse(distance));
				args.Add(ParseStringToVector3(gameObject.Attributes["position"].InnerText));
				args.Add(pointsOnCircle);

			}

			var isgo = CreateISGO(type, args.ToArray());

			return isgo;
		}

		/// <summary>
		/// Function creates IStaticGameObject from given arguments and string with specific type of IStaticGameObject.
		/// </summary>
		/// <param Name="type">Specific type of IStaticGameObject in string</param>
		/// <param Name="args">Arguments for object constructor</param>
		/// <returns>Instance of IStaticGameObject (specific in type)</returns>
		public IStaticGameObject CreateISGO(string type, object[] args) {
			IStaticGameObject isgo = (IStaticGameObject)CreateGameObject(type, args);
			return isgo;
		}


		/// <summary>
		/// Creates special type Gate (is not runtime compiled). 
		/// </summary>
		/// <param Name="solarSystName">Name of SolarSystem where will be</param>
		/// <returns>Instance of Gate</returns>
		private Gate CreateGate(string solarSystName) {
			string team = "None";
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			var gate = new Gate("Gate " + solarSystName,
				"gate.mesh",
				teamDict[team]);
			gate.Team.AddISGO(gate);
			return gate;
		}

		/// <summary>
		/// Function brings together objects in same SolarSystem (All in one XmlNode "solarSystem")
		/// </summary>
		/// <param Name="Name">Name of SolarSystem</param>
		/// <param Name="position">SolarSystem position in space (for count dostance between others)</param>
		/// <param Name="isgoObjects">All IStaticGameObjects in this SolarSystem</param>
		/// <param Name="imgoObjects">All IMovableGameObjects in this SolarSystem</param>
		/// <param Name="sun">SolarSystem's Sun - can be null</param>
		/// <returns>Instance of SolarSystem</returns>
		private SolarSystem CreateSolarSystem(string name, Vector3 position, List<IStaticGameObject> isgoObjects, List<IMovableGameObject> imgoObjects,
			IStaticGameObject sun = null) {

			SolarSystem sSys = new SolarSystem(name, position);
			sSys.AddISGO(isgoObjects);
			sSys.AddIMGO(imgoObjects);
			sSys.Sun = sun;
			return sSys;
		}

		private XmlNode GetTypeNode(string typeName) {
			var node = missionNode.SelectNodes("usedObjects//gameObject[@name='" + typeName + "']")[0];
			return node;
		}


		private void RegisterSGOaction(IStaticGameObject isgo, string action, string value) {
			//isgo.registerExecuter(action, isgo.Team.GetMaterials(), value);
		}

		private void ReadSGOActions(XmlNodeList actionList, StaticGameObject sgo) {
			foreach (XmlNode action in actionList) {
				RegisterSGOaction(sgo, action.Attributes["name"].Value, action.Attributes["value"].Value);
			}
		}

		/// <summary>
		/// Parse Mogre.Vector3 from string
		/// </summary>
		/// <param Name="input">String with vector</param>
		/// <returns>Mogre.Vector3 parsed from given string</returns>
		private Mogre.Vector3 ParseStringToVector3(string input) {
			string[] splitted = input.Split(',');
			Mogre.Vector3 v;
			try {
				v = new Vector3(Int32.Parse(splitted[0]), Int32.Parse(splitted[1]), Int32.Parse(splitted[2]));
			} catch (Exception) {
				throw new FormatException("Cannot parse string " + input + " to Mogre.Vector3. Given string was in a bad format (right format: \"x,y,z\")");
			}
			return v;
		}

		private string GetUnusedName(string name) {
			if (usedNameDict.ContainsKey(name)) {
				usedNameDict[name]++;
				return name + usedNameDict[name];
			} else {
				usedNameDict.Add(name, 0);
				return name;
			}
		}

	}

}
