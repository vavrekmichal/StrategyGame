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
		protected Dictionary<string, Team> teams;
		protected List<IMaterial> materialList;
		protected List<SolarSystem> solarSystems;
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
			this.manager = manager;
			this.teams = teams;
			this.materialList = materialList;
			this.solarSystems = solarSystems;
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

			comilationOption = new CompilationOptions(OutputKind.DynamicallyLinkedLibrary);

			assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly" + Guid.NewGuid()),
															  AssemblyBuilderAccess.RunAndCollect);
			moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
		}

		public void load(string missionName, PropertyManager propMan) {
			propMgr = propMan;
			propMan.setPropertyPath(missionName);
			bool hasSun = false;
			IStaticGameObject sun = null;
			XmlNode missionNode = root.SelectNodes("/mission[@name='" + missionName + "'][1]")[0];
			XmlNode missionSolarSystems = missionNode.SelectNodes("solarSystems[1]")[0];
			foreach (XmlNode solarSystem in missionSolarSystems) {
				List<IStaticGameObject> isgos = new List<IStaticGameObject>();
				List<IMovableGameObject> imgos = new List<IMovableGameObject>();
				string gameObjectType;
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

				string solarSystemName = solarSystem.Attributes["name"].Value;
				if (hasSun) {
					this.solarSystems.Add(createSolarSystem(solarSystemName, isgos, imgos, sun));
					hasSun = false;
				} else {
					this.solarSystems.Add(createSolarSystem(solarSystemName, isgos, imgos));
				}

			}
		}


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

				var result = comp.Emit(moduleBuilder);
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


		private IMovableGameObject createIMGO(XmlNode gameObject, XmlNode gameObjectPath) {
			IMovableGameObject imgo;
			string type;
			List<object> args = new List<object>();
			args.Add(gameObject.Attributes["name"].InnerText);
			args.Add(gameObject.Attributes["mesh"].InnerText);
			
			string team = gameObject.Attributes["team"].InnerText;
			type = gameObject.Attributes["type"].InnerText;
			if (!teams.ContainsKey(team)) {
				teams.Add(team, new Team(team, materialList));
			}
			args.Add(teams[team]);
			args.Add(manager);

			args.Add(parseInputToVector3(gameObject.Attributes["position"].InnerText));
			args.Add(propMgr);
			imgo = (IMovableGameObject)createGameObject(gameObjectPath, type, args.ToArray());
			return imgo;
		}


		private IStaticGameObject createISGO(XmlNode gameObject, XmlNode gameObjectPath, IsgoType isgoType, int pointsOnCircle = 30) {
			IStaticGameObject isgo;
			string type;
			List<object> args = new List<object>();
			args.Add(gameObject.Attributes["name"].InnerText);
			args.Add(gameObject.Attributes["mesh"].InnerText);
			if (isgoType == IsgoType.Sun) {
				args.Add(manager);
				type = IsgoType.Sun.ToString();
			} else {
				string team = gameObject.Attributes["team"].InnerText;
				type = gameObject.Attributes["type"].InnerText;
				if (!teams.ContainsKey(team)) {
					teams.Add(team, new Team(team, materialList));
				}
				args.Add(teams[team]);
				args.Add(manager);
				args.Add(Int32.Parse(gameObject.Attributes["distance"].InnerText));
				args.Add(parseInputToVector3(gameObject.Attributes["centerPosition"].InnerText));
				args.Add(propMgr);
				args.Add(pointsOnCircle);

			}
			isgo = (IStaticGameObject)createGameObject(gameObjectPath, type, args.ToArray());
			return isgo;
		}

		private SolarSystem createSolarSystem(string name, List<IStaticGameObject> isgoObjects, List<IMovableGameObject> imgoObjects,
			IStaticGameObject sun = null) {

			SolarSystem sSys = new SolarSystem(name);
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

		private Mogre.Vector3 parseInputToVector3(string input) {
			string[] splitted = input.Split(',');
			return new Vector3(Int32.Parse(splitted[0]), Int32.Parse(splitted[1]), Int32.Parse(splitted[2]));
		}

		//testing loading classes
		private void testingFunction() {
			
			var syntaxTree = SyntaxTree.ParseFile("../../GroupControl/Game Objects/StaticGameObjectBox/Planet.cs");

			var t = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			var comp = Compilation.Create("Test.dll"
				, syntaxTrees: new[] { syntaxTree }
				, references: new[] { 
					MetadataFileReference.CreateAssemblyReference("mscorlib"),
					new MetadataFileReference(typeof(Strategy.MyMogre).Assembly.Location),
					new MetadataFileReference(typeof(Strategy.Game).Assembly.Location),
					new MetadataFileReference(typeof(GroupControl.Game_Objects.StaticGameObjectBox.StaticGameObject).Assembly.Location),
					new MetadataFileReference(typeof(Team).Assembly.Location),
					new MetadataFileReference(typeof(System.Linq.Enumerable).Assembly.Location),
					new MetadataFileReference(typeof(LinkedList<>).Assembly.Location),
					new MetadataFileReference(Path.GetFullPath((new Uri(t + "\\\\" + "Mogre.dll")).LocalPath)),
					new MetadataFileReference(typeof(GroupControl.Game_Objects.StaticGameObjectBox.IStaticGameObject).Assembly.Location),
					new MetadataFileReference(typeof(Strategy.Game).Assembly.Location)
				}
				, options: new CompilationOptions(OutputKind.DynamicallyLinkedLibrary)
				);
			var assemblyBuilder =
				AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly" + Guid.NewGuid()),
															  AssemblyBuilderAccess.RunAndCollect);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule");
			var result = comp.Emit(moduleBuilder);
			if (!result.Success)
            {
                foreach(var d in result.Diagnostics){
                    Console.WriteLine(d);
                }
				throw new XmlLoadException("Class not found ");
            }
			
			var o = moduleBuilder.GetType("Strategy.GroupControl.Game_Objects.StaticGameObjectBox.Planet");
			if (!teams.ContainsKey("bla")) {
				teams.Add("bla", new Team("bla", materialList));
			}
			
			
			MethodInfo mainMethod = o.GetMethod("rotate");
			MethodInfo testMethod = o.GetMethod("registerExecuter");


			object helloObject = Activator.CreateInstance(o, "name", "jupiter.mesh", teams["bla"], manager, 500, Mogre.Vector3.ZERO, 30);
			
			IStaticGameObject isgo = (IStaticGameObject)helloObject;
		}

	}

}
