using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
using Mogre;
using Strategy.GameMaterial;


namespace Strategy.GroupControl.Game_Objects {

	enum IsgoType { StaticObject, Sun}

	class ObjectCreator {

        protected List<IMaterial> materialList;
        protected List<SolarSystem> solarSystems;
		protected Mogre.SceneManager manager;
        protected Dictionary<string,Team> teams;

		protected Dictionary<string, bool> objectIsMovable;
		protected Dictionary<string, IStaticGameObject> isgoDict;
		protected Dictionary<string, IMovableGameObject> imgoDict;

        #region singleton and constructor
        private static ObjectCreator instance;

        /// <summary>
        /// Singleton instance
        /// </summary>
        /// <param name="manager">Mogre SceneManager</param>
        /// <returns>returning singleton instance</returns>
		public static ObjectCreator getInstance(Mogre.SceneManager manager) {
			if (instance == null) {
				instance = new ObjectCreator(manager);
			}
			return instance;
		}

        /// <summary>
        /// private constructor
        /// </summary>
        /// <param name="manager">Mogre SceneManager</param>
		private ObjectCreator(Mogre.SceneManager manager) {
			this.manager = manager;
            teams = new Dictionary<string,Team>();
            solarSystems = new List<SolarSystem>();
		}
        #endregion

        /// <summary>
        /// inicialization of game World
        /// </summary>
        /// <param name="mission">name of mission</param>
		public void initializeWorld(string mission){ 
            //visual part 
            createMaterials();

			ObjectXMLCreator xml = new ObjectXMLCreator("../../Media/Mission/MyMission.xml", manager, teams, materialList, solarSystems);
			xml.load(mission);
			createObjectMap(); //map for hittest
            solarSystems[0].showSolarSystem();

		}

		public void getInicializedObjects(out List<SolarSystem> s){
			s= solarSystems;
		}

		public Dictionary<string, Team> getTeams() {
			return teams;
		}


		public bool isObjectMovable(string name) {
			return objectIsMovable[name];
		}

		public IMovableGameObject getIMGO(string name){
			return imgoDict[name];
		}

		public IStaticGameObject getISGO(string name) {
			return isgoDict[name];
		}

		private void createObjectMap() {
			objectIsMovable = new Dictionary<string, bool>();
			isgoDict = new Dictionary<string, IStaticGameObject>();
			imgoDict = new Dictionary<string, IMovableGameObject>();
			foreach (SolarSystem ss in solarSystems) {
				IStaticGameObject s =  ss.getSun();
				if(s!= null){
					objectIsMovable.Add(s.Name, false);
					isgoDict.Add(s.Name, s);
				}

				foreach(IStaticGameObject isgo in ss.getISGOs()){
					objectIsMovable.Add(isgo.Name, false);
					isgoDict.Add(isgo.Name, isgo);
				}

				foreach (IMovableGameObject imgo in ss.getIMGOs()) {
					objectIsMovable.Add(imgo.Name, true);
					imgoDict.Add(imgo.Name, imgo);
				}
			}
		}

		//DELETE VSE DOLU
		/*
        private SolarSystem createSolarSystem(string name, List<IStaticGameObject> isgoObjects, List<IMovableGameObject> imgoObjects,
            Sun sun = null) {
      
            SolarSystem sSys = new SolarSystem(name);
            sSys.addISGO(isgoObjects);
            sSys.addIMGO(imgoObjects);
            sSys.setSun(sun);
            return sSys;
        }

        private Sun createSun(string name, string mesh) {
            return new Sun(name, mesh, manager);
        }

		//delete
        private Planet createPlanet(string name,string mesh, string team, Vector3 center, int radius) {
            if (!teams.ContainsKey(team)){  
                teams.Add(team,new Team(team, materialList));
            }
            
            Planet p = new Planet(name, mesh, teams[team], manager, radius, center);
            teams[team].addISGO(p);
            Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            Console.WriteLine(p.Team.getName());
            p.registerExecuter("Produce",teams[team].getMaterials(),"hovno");
            return p;
        }
		//delete
		private void createIMGO() {	}
		//delete
        private void createISGO() { }

        

		//TOTO TU NESMI ZUSTAT*/
        private void createMaterials() {
            materialList = new List<IMaterial>() { new Wolenium(), new Wolenarium(), new Class1() };
        }
		/*
	}
	//delete
	static class RandomUtil {
		/// <summary>
		/// Get random string of 11 characters.
		/// </summary>
		/// <returns>Random string.</returns>
		public static string GetRandomString() {
			string path = Path.GetRandomFileName();
			path = path.Replace(".", ""); // Remove period.
			return path;
		}*/
	}
}
