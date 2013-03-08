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


namespace Strategy.GroupControl.Game_Objects {
	class ObjectXMLCreator {

		protected XmlDocument xml;  
		protected Mogre.SceneManager manager;
		protected Dictionary<string, Team> teams;
		protected List<IMaterial> materialList;
		protected List<SolarSystem> solarSystems;
		protected XmlElement root;

		public ObjectXMLCreator(string path, Mogre.SceneManager manager, Dictionary<string, Team> teams,
			List<IMaterial> materialList, List<SolarSystem> solarSystems) {
			this.manager = manager;
			this.teams = teams;
			this.materialList = materialList;
			this.solarSystems = solarSystems;
			xml = new XmlDocument();
			xml.Load(path);
			root = xml.DocumentElement;
			testingFunction(); //TODO remove
		}

		public void load(string missionName) {
			bool hasSun = false;
			IStaticGameObject sun = null;
			XmlNode missionSolarSystems = root.SelectNodes("/mission[@name='" + missionName + "'][1]/solarSystems[1]")[0];
			foreach (XmlNode solarSystem in missionSolarSystems) {
				List<IStaticGameObject> isgos = new List<IStaticGameObject>();
				foreach (XmlNode gameObject in solarSystem.ChildNodes) {
					switch (gameObject.Name) {
							case "isgo":
							isgoType t;
							switch (gameObject.Attributes["type"].InnerText) {
								case "Planet":
									t=isgoType.Planet;
									IStaticGameObject isgo = createISGO(gameObject.Attributes["name"].InnerText,
										gameObject.Attributes["mesh"].InnerText,
										gameObject.Attributes["team"].InnerText,
										parseInputToVector3(gameObject.Attributes["centerPossition"].InnerText),
										Int32.Parse(gameObject.Attributes["distance"].InnerText),
										t
										);
									isgos.Add(isgo);
									//here register
									readSGOActions(gameObject.SelectNodes("gameAction"), (StaticGameObject)isgo);	
									break;
								case "Sun":
									hasSun = true;
									t = isgoType.Sun;
									sun = createISGO(gameObject.Attributes["name"].InnerText,
										gameObject.Attributes["mesh"].InnerText,
										gameObject.Attributes["team"].InnerText,
										parseInputToVector3(gameObject.Attributes["centerPossition"].InnerText),
										Int32.Parse(gameObject.Attributes["distance"].InnerText),
										t
										);
									
									break;
								default:
									t = isgoType.Sun;
									break;
								}

								break;
							case "imgo":

								break;
							default:
								throw new XmlLoadException("Bad XML format. In SolarSystem cannot be node " + gameObject.Name);
						}
				}

				if (hasSun) {
					this.solarSystems.Add(createSolarSystem(solarSystem.Attributes["name"].Value, isgos, new List<IMovableGameObject>(), sun));
					hasSun = false;
				} else {
					this.solarSystems.Add(createSolarSystem(solarSystem.Attributes["name"].Value, isgos, new List<IMovableGameObject>()));
				}
				
			}  
		}

		

		private IStaticGameObject createISGO(string name, string mesh, string team, Vector3 center, int radius, isgoType type, int pointsOnCircle = 30) {
			if (!teams.ContainsKey(team)) {
				teams.Add(team, new Team(team, materialList));
			}
			System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
			Type t = ass.GetType("Strategy.GroupControl.Game_Objects.StaticGameObjectBox."+type);
			IStaticGameObject o;
			if(type==isgoType.Sun){
				Object[] args = { name, mesh, manager};
				o = (IStaticGameObject)Activator.CreateInstance(t, args);
			}else{
				Object[] args = { name, mesh, teams[team], manager, radius, center, pointsOnCircle };
				o = (IStaticGameObject)Activator.CreateInstance(t, args);
			}
			
			return o;
		}
		

		private SolarSystem createSolarSystem(string name, List<IStaticGameObject> isgoObjects, List<IMovableGameObject> imgoObjects,
			IStaticGameObject sun = null) {

			SolarSystem sSys = new SolarSystem(name);
			sSys.addISGO(isgoObjects);
			sSys.addIMGO(imgoObjects);
			sSys.setSun((Sun)sun);
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

		private Mogre.Vector3 parseInputToVector3(string input) {
			string[] splitted = input.Split(',');
			return new Vector3(Int32.Parse(splitted[0]), Int32.Parse(splitted[1]), Int32.Parse(splitted[2]));
		}



		//testing loading classes
		private void testingFunction() {
			SyntaxTree syntaxTree = SyntaxTree.ParseText(
				@"using System;
				using System.Collections;
				using System.Linq;
				using System.Text;
 
				namespace HelloWorld
				{
					class Program
					{
						static void Main(string[] args)
						{
							Console.WriteLine(""Hello, World!"");
						}
					}
				}");

			var root = (CompilationUnitSyntax)syntaxTree.GetRoot();

			var firstMember = root.Members[0];

			var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;

			var programDeclaration = (TypeDeclarationSyntax)helloWorldDeclaration.Members[0];

			var mainDeclaration = (MethodDeclarationSyntax)programDeclaration.Members[0];

			var argsParameter = mainDeclaration.ParameterList.Parameters[0];

		}
	}
}
