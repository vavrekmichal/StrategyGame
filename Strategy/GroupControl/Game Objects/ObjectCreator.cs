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

	enum isgoType { Planet, Sun}
	class ObjectCreator {

        protected List<IMaterial> materialList;
        protected List<SolarSystem> solarSystems;
		protected Mogre.SceneManager manager;
        protected Dictionary<string,Team> teams;

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
		public void initializeWorld(string mission){ //not implemented
			//TODO: XML Reader on mission will be here                !!!!!!!!!!!!!!!!!!!!!!!!!!!! 
            //visual part 
            createMaterials();

			ObjectXMLCreator xml = new ObjectXMLCreator("../../Media/Mission/MyMission.xml", manager, teams, materialList, solarSystems);
			xml.load("StartMission");
            solarSystems[0].showSolarSystem();
		}


        //public void makeInvisibleIMGO(IMGO)
        //public void makeInvisibleISGO(ISGO) dispose entity and sceneNode ...used when you changing visibility

		public void getInicializedObjects(out List<SolarSystem> s){
			s= solarSystems;
		}

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

        private Planet createPlanet(string name,string mesh, string team, Vector3 center, int radius) {
            if (!teams.ContainsKey(team)){  
                teams.Add(team,new Team(team, materialList));
            }
            
            Planet p = new Planet(name, mesh, teams[team], manager, radius, center);
            teams[team].addISGO(p);
            Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            Console.WriteLine(p.team.getName());
            p.registerExecuter("Produce",teams[team].getMaterials());
            return p;
        }

		private void createIMGO() {	}

        private void createISGO() { }

        public Dictionary<string, Team> getTeams() {
            return teams;
        }

        private void createMaterials() {
            materialList = new List<IMaterial>() { new Wolenium(), new Wolenarium(), new Class1() };
        }
	}

	static class RandomUtil {
		/// <summary>
		/// Get random string of 11 characters.
		/// </summary>
		/// <returns>Random string.</returns>
		public static string GetRandomString() {
			string path = Path.GetRandomFileName();
			path = path.Replace(".", ""); // Remove period.
			return path;
		}
	}
}
