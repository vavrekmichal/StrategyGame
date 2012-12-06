using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;


namespace Strategy.GroupControl.Game_Objects {
	class ObjectCreator {

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

            string myTeam = "This is my team";
            solarSystems.Add(createSolarSystem("SolarSystem", new List<IStaticGameObject>(){
                createPlanet("PlanetMercury", "mercury.mesh", myTeam, 500),
                createPlanet("PlanetVenus", "venus.mesh", myTeam, 1500),
                createPlanet("PlanetEarth", "earth.mesh", myTeam, 2500),
                createPlanet("PlanetMars", "mars.mesh", myTeam, 3500),
                createPlanet("PlanetJupiter", "jupiter.mesh", myTeam, 6000),
                createPlanet("PlanetSaturn", "saturn.mesh", myTeam, 8000),
                createPlanet("PlanetUranus", "uranus.mesh", myTeam, 10000),
                createPlanet("PlanetNeptune", "neptune.mesh", myTeam, 12000)
            }, 
            new List<IMovableGameObject>(),
            createSun("Sun", "sun.mesh")));
            myTeam = "This is funny Team";
            solarSystems.Add(createSolarSystem("FunnySystem", new List<IStaticGameObject>() {
                createPlanet("FunnyThings", "knot.mesh", myTeam, 2000),
                createPlanet("FunnyThings2", "ninja.mesh", myTeam, 4000),
                createPlanet("FunnyThings3", "robot.mesh", myTeam, 1000)
            },
            new List<IMovableGameObject>(),
            createSun("FunnySun", "jupiter.mesh")));

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

        private Planet createPlanet(string name,string mesh, string team, int radius) {
            Planet p = new Planet(name, mesh, team, manager, radius);
            if (!teams.ContainsKey(team)){
                teams.Add(team,new Team(team));
            }
            teams[team].addISGO(p);
            return p;
        }

		private void createIMGO() {	}

        private void createISGO() { }

        public Dictionary<string, Team> getTeams() {
            return teams;
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
