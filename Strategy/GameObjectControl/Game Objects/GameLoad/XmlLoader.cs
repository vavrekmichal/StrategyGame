using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Mogre;
using Roslyn.Compilers.CSharp;
using Strategy.Exceptions;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.GameLoad {
	/// <summary>
	/// Loads the mission from XML file and initializes all the game objects.
	/// </summary>
	public class XmlLoader {

		protected RunTimeCreator runtimeCtor;

		private XmlDocument xml;
		private Dictionary<string, Team> teamDict;
		private List<SolarSystem> solarSystemList;
		private Dictionary<Team, List<Team>> teamRealationDict;
		private XmlElement root;
		XmlNode missionNode; // Selected mission

		private const string schemaPath = "../../Media/Mission/mission.xsd";

		/// <summary>
		/// Initializes the mission and vilidates the mission file with mission XMLSchema file.
		/// </summary>
		/// <param name="missionFilePath">The path to the file with mission.</param>
		/// <param name="teams">The dictionary which will be filled by Teams (should be empty).</param>
		/// <param name="solarSystems">The dictionary which will be filled by SolarSystem. (should be empty).</param>
		public XmlLoader(string missionFilePath, Dictionary<string, Team> teams,
			List<SolarSystem> solarSystems) {

			teamRealationDict = new Dictionary<Team, List<Team>>();

			this.teamDict = teams;
			this.solarSystemList = solarSystems;
			xml = new XmlDocument();

			// Checks the mission XmlSchema
			XmlSchemaSet schemas = new XmlSchemaSet();
			schemas.Add("", schemaPath);

			xml.Load(missionFilePath);
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

			runtimeCtor = new RunTimeCreator();
		}

		#region Public
		/// <summary>
		/// Loads mission from setted file and creates all game objects from the mission file.
		/// Also load materials and mission targets.
		/// </summary>
		public void LoadMission() {
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
							var gate = runtimeCtor.CreateGate(solarSystemName, teamDict["None"]);
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
								CreateGameActions(gameObject.SelectNodes("gameAction"), isgo);
								break;
							}
							break;
						case "imgo":
							gameObjectType = gameObject.Attributes["type"].InnerText;
							IMovableGameObject imgo = CreateIMGO(gameObject);
							imgo.Team.AddIMGO(imgo);
							imgos.Add(imgo);
							CreateGameActions(gameObject.SelectNodes("gameAction"), imgo);
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
		/// Returns Team relations.
		/// </summary>
		/// <returns></returns>
		public Dictionary<Team, List<Team>> GetTeamsRelations() {
			return teamRealationDict;
		}

		/// <summary>
		/// Finds information for RunTimCreator by the given name and creates the instance 
		/// of the IStaticGameObject.
		/// </summary>
		/// <param name="typeName">The type of the creating object.</param>
		/// <param name="args">The arguments of the object.</param>
		/// <returns>Returns created IStaticGameObject.</returns>
		public IStaticGameObject CreateISGO(string typeName, object[] args) {
			var typeNode = GetGameObjectTypeNode(typeName);
			string fullName = typeNode.Attributes["fullName"].InnerText;

			var isgo = runtimeCtor.CreateISGO(fullName, args.ToArray());
			return isgo;
		}

		/// <summary>
		/// Finds information for RunTimCreator by the given name and creates the instance 
		/// of the IMovableGameObject.
		/// </summary>
		/// <param name="typeName">The type of the creating object.</param>
		/// <param name="args">The arguments of the object.</param>
		/// <returns>Returns created IMovableGameObject.</returns>
		public IMovableGameObject CreateIMGO(string typeName, object[] args) {
			var typeNode = GetGameObjectTypeNode(typeName);

			string fullName = typeNode.Attributes["fullName"].InnerText;

			var imgo = runtimeCtor.CreateIMGO(fullName, args.ToArray());

			return imgo;
		}

		/// <summary>
		/// Returns unused name with given base.
		/// </summary>
		/// <param name="name">The base of the name.</param>
		/// <returns>Returns unused name with given base.</returns>
		public string GetUnusedName(string name) {
			return runtimeCtor.GetUnusedName(name);
		}
		#endregion


		/// <summary>
		/// Compiles all game object which are used in the mission.
		/// </summary>
		/// <param name="node">The node with names and paths of compiling objects.</param>
		private void CompileUsedObjects(XmlNode node) {
			var synatexTreeList = new List<SyntaxTree>();
			var compiled = new List<string>();

			foreach (XmlNode usedObj in node.ChildNodes) {
				foreach (XmlNode obj in usedObj) {
					string fullPath = obj.Attributes["path"].InnerText;
					string type = obj.Attributes["name"].InnerText; ;
					if (!compiled.Contains(type)) {
						compiled.Add(type);
						var syntaxTree = SyntaxTree.ParseFile("../../GameObjectControl/Game Objects/" + fullPath);
						synatexTreeList.Add(syntaxTree);
					}
				}
			}
			runtimeCtor.CompileUsedObjects(synatexTreeList);
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
		/// Loads mission targets.
		/// </summary>
		/// <param name="targetNodeList">XmlNode of mission targets to load.</param>
		private void LoadMissionTargets(XmlNode targetNodeList) {
			foreach (XmlNode targetNode in targetNodeList) {
				var typeName = targetNode.Attributes["name"].InnerText;

				var typeNode = GetGameTargetTypeNode(typeName);

				string fullName = typeNode.Attributes["fullName"].InnerText;

				var o = runtimeCtor.CreateITarget(fullName, new object[1] { LoadArguments(targetNode).ToArray() });
				Game.Mission.AddTarget(o);
			}
		}

		/// <summary>
		/// Loads names of teams and creates the relations.
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
				// Teams in same node are friendly
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

		/// <summary>
		/// Creates a new SolarSystem with given unique name, position, and members.
		/// </summary>
		/// <param name="name">The unique name.</param>
		/// <param name="position">The position of the SolarSystem.</param>
		/// <param name="isgoObjects">The static members.</param>
		/// <param name="imgoObjects">The movable members.</param>
		/// <param name="sun">The sun of the SolarSystem (could be null).</param>
		/// <returns>Returns created SolarSystem.</returns>
		private SolarSystem CreateSolarSystem(string name, Vector3 position, List<IStaticGameObject> isgoObjects, List<IMovableGameObject> imgoObjects,
			IStaticGameObject sun = null) {

			SolarSystem sSys = new SolarSystem(name, position);
			sSys.AddISGO(isgoObjects);
			sSys.AddIMGO(imgoObjects);
			sSys.Sun = sun;
			return sSys;
		}

		/// <summary>
		/// Loads all subnodes with name "argument" from the given node. Inserts their InnerText to list
		/// and returns it.
		/// </summary>
		/// <param name="gameObjectNode">The node with arguement subnodes.</param>
		/// <returns>Returns the List with the InnerText of argument subnodes.</returns>
		private List<object> LoadArguments(XmlNode gameObjectNode) {
			var selectedArguments = gameObjectNode.SelectNodes("./argument");
			var result = new List<object>();
			foreach (XmlNode item in selectedArguments) {
				result.Add(item.InnerText);
			}
			return result;
		}

		/// <summary>
		/// Tries to parse the given string to Vector3. If the parsing fails so throws a exception.
		/// </summary>
		/// <param name="input"></param>
		/// <returns>Returns parsed Vector3 from the string</returns>
		private static Mogre.Vector3 ParseStringToVector3(string input) {
			string[] splitted = input.Split(';');
			Mogre.Vector3 v;
			try {
				v = new Vector3(Single.Parse(splitted[0]), Single.Parse(splitted[1]), Single.Parse(splitted[2]));
			} catch (Exception) {
				throw new FormatException("Cannot parse string " + input + " to Mogre.Vector3. Given string was in a bad format (right format: \"x;y;z\")");
			}
			return v;
		}

		/// <summary>
		/// Creates gameObject's game actions from the given list.
		/// </summary>
		/// <param name="actionList">The list with XmlNodes which contains game actions.</param>
		/// <param name="gameObject">The owner of the action.</param>
		private void CreateGameActions(XmlNodeList actionList, IGameObject gameObject) {
			foreach (XmlNode action in actionList) {
				object[] args = new object[2];
				args[0] = gameObject;
				args[1] = LoadArguments(action).ToArray();

				var typeNode = GetGameActionTypeNode(action.Attributes["name"].InnerText);

				string fullName = typeNode.Attributes["fullName"].InnerText;

				gameObject.AddIGameAction(runtimeCtor.CreateIGameAction(fullName, args));
			}
		}

		/// <summary>
		/// Finds information for RunTimCreator by the given name and collects arguments. After that creates the instance 
		/// of the IStaticGameObject. 
		/// </summary>
		/// <param name="gameObject">The XmlNode with creating object type and arguments.</param>
		/// <param name="isgoType">Represents if the creating type is a Sun ro not.</param>
		/// <returns>Returns created IStaticGameObject.</returns>
		private IStaticGameObject CreateISGO(XmlNode gameObject, IsgoType isgoType) {
			string type;
			List<object> args = new List<object>();

			args.Add(runtimeCtor.GetUnusedName(gameObject.Attributes["name"].InnerText));
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
		/// Finds information for RunTimCreator by the given name and collects arguments. After that creates the instance 
		/// of the IMovableGameObject. 
		/// </summary>
		/// <param name="gameObject">The XmlNode with creating object type and arguments.</param>
		/// <returns>Returns created IMovableGameObject.</returns>
		private IMovableGameObject CreateIMGO(XmlNode gameObject) {

			List<object> args = new List<object>();
			args.Add(runtimeCtor.GetUnusedName(gameObject.Attributes["name"].InnerText));


			string team = gameObject.Attributes["team"].InnerText;
			string type = gameObject.Attributes["type"].InnerText;
			if (!teamDict.ContainsKey(team)) {
				throw new XmlException("Undefined Team " + team + " .");
			}
			args.Add(teamDict[team]);
			args.Add((LoadArguments(gameObject).ToArray()));

			var typeNode = GetGameObjectTypeNode(type);

			string fullName = typeNode.Attributes["fullName"].InnerText;

			return runtimeCtor.CreateIMGO(fullName, args.ToArray());
		}

		/// <summary>
		/// Returns XmlNode with information about the object type. Searchs at "usedObjects/isgos//gameObject"
		/// or "usedObjects/imgos//gameObject".
		/// </summary>
		/// <param name="typeName">The name of the object type.</param>
		/// <returns>Returns XmlNode with information about the object type.</returns>
		private XmlNode GetGameObjectTypeNode(string typeName) {
			var node = missionNode.SelectNodes("usedObjects/isgos//gameObject[@name='" + typeName + "']")[0];
			if (node == null) {
				node = missionNode.SelectNodes("usedObjects/imgos//gameObject[@name='" + typeName + "']")[0];
			}
			return node;
		}

		/// <summary>
		/// Returns XmlNode with information about the action type. Searchs at "usedObjects/gameActions//gameObject".
		/// </summary>
		/// <param name="typeName">The name of the action type.</param>
		/// <returns>Returns XmlNode with information about the action type.</returns>
		private XmlNode GetGameActionTypeNode(string typeName) {
			var node = missionNode.SelectNodes("usedObjects/gameActions//gameObject[@name='" + typeName + "']")[0];
			return node;
		}

		/// <summary>
		/// Returns XmlNode with information about the target type. Searchs at "usedObjects/gameTargets//gameObject".
		/// </summary>
		/// <param name="typeName">The name of the target type.</param>
		/// <returns>Returns XmlNode with information about the target type.</returns>
		private XmlNode GetGameTargetTypeNode(string typeName) {
			var node = missionNode.SelectNodes("usedObjects/gameTargets//gameObject[@name='" + typeName + "']")[0];
			return node;
		}
	}
}
