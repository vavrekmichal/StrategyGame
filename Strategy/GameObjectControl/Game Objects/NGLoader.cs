using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Xml;
using System.Xml.Schema;
using Mogre;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;
using Strategy.Exceptions;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects.GameTargets;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;


namespace Strategy.GameObjectControl.Game_Objects {
	public class NGLoader {

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

		private const string schemaPath = "../../Media/Mission/mission.xsd";

		//private string missionName;

		public NGLoader(string path, Dictionary<string, Team> teams,
			List<SolarSystem> solarSystems) {
			teamRealationDict = new Dictionary<Team, List<Team>>();
			usedNameDict = new Dictionary<string, int>();

			this.teamDict = teams;
			this.solarSystemList = solarSystems;
			xml = new XmlDocument();

			XmlSchemaSet schemas = new XmlSchemaSet();
			schemas.Add("", schemaPath);

			xml.Load(path);
			xml.Schemas.Add(schemas);
			string msg = "";
			xml.Validate((o, err) => {
				msg = err.Message;
			});

			if (msg == "") {
				Console.WriteLine("Document is valid");
			} else {
				throw new XmlLoadException("Document invalid: " + msg);
			}
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
			metadataRef.Add(new MetadataFileReference(typeof(XmlLoadException).Assembly.Location));

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


		public void Load(string missionPropFileName) {

			bool hasSun = false;
			IStaticGameObject sun = null;

			// Load mission (first of given Name)
			missionNode = root.SelectNodes("/mission[1]")[0];

			Game.PropertyManager.LoadPropertyToMission(missionNode.Attributes["propertyFilePath"].InnerText);

			LoadTeams(missionNode.SelectNodes("teams[1]")[0]);

			CompileUsedObjects(missionNode.SelectNodes("usedObjects[1]")[0]);

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

								// Create IGameAction for created object
								ReadGameActions(gameObject.SelectNodes("gameAction"), isgo);
								break;
							}
							break;
						case "imgo":
							gameObjectType = gameObject.Attributes["type"].InnerText;
							IMovableGameObject imgo = CreateIMGO(gameObject);
							imgo.Team.AddIMGO(imgo);
							imgos.Add(imgo);
							ReadGameActions(gameObject.SelectNodes("gameAction"), imgo);
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

			// Finally load mission targets and materials
			LoadMissionTargets(missionNode.SelectNodes("missionTargets[1]")[0]);
			LoadMaterials(missionNode.SelectNodes("materials[1]")[0]);
		}

		/// <summary>
		/// Loads the starting quantity of materials
		/// </summary>
		/// <param name="materialNodeList">XmlNode of materials to load.</param>
		private void LoadMaterials(XmlNode materialNodeList) {
			if (materialNodeList == null) {
				return;
			}
			foreach (XmlNode materialNode in materialNodeList) {
				var materialName = materialNode.Attributes["name"].InnerText;
				var team = materialNode.Attributes["team"].InnerText;
				var quantity = Convert.ToInt32(LoadArguments(materialNode)[0]);
				teamDict[team].Produce(materialName, quantity);
			}
		}

		/// <summary>
		/// Loads mission targets from Xml file (node missionTargets).
		/// </summary>
		/// <param name="targetNodeList">XmlNode of mission targets to load.</param>
		private void LoadMissionTargets(XmlNode targetNodeList) {
			foreach (XmlNode targetNode in targetNodeList) {
				var typeName = targetNode.Attributes["name"].InnerText;
				var o = CreateITarget(typeName, new object[1] { LoadArguments(targetNode).ToArray() });
				Game.Mission.AddTarget(o);
			}
		}

		/// <summary>
		/// Loads names of teams from XML file
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
		/// Runtime creates object with parameters and relative path from XML file.
		/// Also checks if class is already compiled or not. If is not then function
		/// throw exception.
		/// </summary>
		/// <param Name="type">Type of compiling object.</param>
		/// <param Name="args">Compiling object's argumentns.</param>
		/// <returns></returns>
		private object CreateObject(XmlNode objectPath, object[] args) {
			string type = objectPath.Attributes["name"].InnerText;
			string fullPath = objectPath.Attributes["path"].InnerText;
			string fullName = objectPath.Attributes["fullName"].InnerText;
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
			//args.Add(gameObject.Attributes["mesh"].InnerText);

			string team = gameObject.Attributes["team"].InnerText;
			string type = gameObject.Attributes["type"].InnerText;
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			args.Add(teamDict[team]);
			args.Add((LoadArguments(gameObject).ToArray()));

			return CreateIMGO(type, args.ToArray());
		}

		/// <summary>
		/// Function creates IMovableGameObject from given arguments and string with specific type of IMovableGameObject.
		/// </summary>
		/// <param Name="type">specific type of IMovableGameObject in string</param>
		/// <param Name="args">arguments for object constructor</param>
		/// <returns>Instance of IMovableGameObject (specific in type)</returns>
		public IMovableGameObject CreateIMGO(string type, object[] args) {
			IMovableGameObject imgo = (IMovableGameObject)CreateObject(GetGameObjectTypeNode(type), args);
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
			var teamNode = gameObject.Attributes["team"];
			string team = "Sun";
			if (teamNode != null) {
				team = gameObject.Attributes["team"].InnerText;
			}

			if (isgoType == IsgoType.Sun) {
				type = IsgoType.Sun.ToString();
			} else {
				if (!teamDict.ContainsKey(team)) {
					throw new XmlException("Undefined Team " + team + " .");
				}
				args.Add(teamDict[team]);
				type = gameObject.Attributes["type"].InnerText;
			}

			var argList = LoadArguments(gameObject);
			args.Add(argList.ToArray());

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
			IStaticGameObject isgo = (IStaticGameObject)CreateObject(GetGameObjectTypeNode(type), args);
			return isgo;
		}


		/// <summary>
		/// Creates special type Gate (is not runtime compiled). 
		/// </summary>
		/// <param Name="solarSystName">Name of SolarSystem where will be</param>
		/// <returns>Instance of Gate</returns>
		private Gate CreateGate(string solarSystName) {
			var gate = new Gate("Gate " + solarSystName,
				teamDict["None"]);
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

		private IGameAction CreateIGameAction(string gameActionType, object[] args) {
			return (IGameAction)CreateObject(GetGameActionTypeNode(gameActionType), args);
		}

		private ITarget CreateITarget(string gameTargetType, object[] args) {
			return (ITarget)CreateObject(GetGameTargetTypeNode(gameTargetType), args);
		}

		private XmlNode GetGameObjectTypeNode(string typeName) {
			var node = missionNode.SelectNodes("usedObjects/isgos//gameObject[@name='" + typeName + "']")[0];
			if (node == null) {
				node = missionNode.SelectNodes("usedObjects/imgos//gameObject[@name='" + typeName + "']")[0];
			}
			return node;
		}

		private XmlNode GetGameActionTypeNode(string typeName) {
			var node = missionNode.SelectNodes("usedObjects/gameActions//gameObject[@name='" + typeName + "']")[0];
			return node;
		}

		private XmlNode GetGameTargetTypeNode(string typeName) {
			var node = missionNode.SelectNodes("usedObjects/gameTargets//gameObject[@name='" + typeName + "']")[0];
			return node;
		}




		private void ReadGameActions(XmlNodeList actionList, IGameObject gameObject) {
			foreach (XmlNode action in actionList) {
				object[] args = new object[2];
				args[0] = gameObject;
				args[1] = LoadArguments(action).ToArray();

				Console.WriteLine();
				gameObject.AddIGameAction(CreateIGameAction(action.Attributes["name"].InnerText, args));
			}
		}

		private List<object> LoadArguments(XmlNode gameObjectNode) {
			var selectedArguments = gameObjectNode.SelectNodes("./argument");
			var result = new List<object>();
			foreach (XmlNode item in selectedArguments) {
				result.Add(item.InnerText);
			}
			return result;
		}

		/// <summary>
		/// Parse Mogre.Vector3 from string
		/// </summary>
		/// <param Name="input">String with vector</param>
		/// <returns>Mogre.Vector3 parsed from given string</returns>
		private static Mogre.Vector3 ParseStringToVector3(string input) {
			string[] splitted = input.Split(';');
			Mogre.Vector3 v;
			try {
				v = new Vector3(Int32.Parse(splitted[0]), Int32.Parse(splitted[1]), Int32.Parse(splitted[2]));
			} catch (Exception) {
				throw new FormatException("Cannot parse string " + input + " to Mogre.Vector3. Given string was in a bad format (right format: \"x;y;z\")");
			}
			return v;
		}

		public string GetUnusedName(string name) {
			if (usedNameDict.ContainsKey(name)) {
				usedNameDict[name]++;
				if (usedNameDict.ContainsKey(name + usedNameDict[name])) {
					return GetUnusedName(name + usedNameDict[name]);
				}
				return name + usedNameDict[name];
			} else {
				usedNameDict.Add(name, 0);
				return name;
			}
		}

	}

}
