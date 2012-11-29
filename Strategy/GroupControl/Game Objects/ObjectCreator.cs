using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;


namespace Strategy.GroupControl.Game_Objects {
	class ObjectCreator {

		protected List<IStaticGameObject> listOfISGO;
		protected List<IMovableGameObject> listOfIMGO;
		protected Mogre.SceneManager manager;

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
			listOfISGO = new List<IStaticGameObject>();
			listOfIMGO = new List<IMovableGameObject>();
		}

        /// <summary>
        /// inicialization of game World
        /// </summary>
        /// <param name="mission">name of mission</param>
		public void initializeWorld(string mission){ //not implemented
			//XML Reader on mission will be here                !!!!!!!!!!!!!!!!!!!!!!!!!!!! 
            //TODO list invisible sol. system Manager > rember and watch it
            //visual part 
			createISGO();
			createSun();
		}

        //public void createVirtualISGO
        //public void createVirualIMGO  not inicialized entity and sceneNode
        //public void makeInvisibleIMGO(IMGO)
        //public void makeInvisibleISGO(ISGO) dispose entity and sceneNode ...used when you changing visibility

		public void getInicializedObjects(out List<IStaticGameObject> s, out List<IMovableGameObject> m){
			s= listOfISGO;
			m= listOfIMGO;
		}

       

		private void createISGO() {
            #region The Solar System
            listOfISGO.Add(new Planet("PlanetMercury", "mercury.mesh", 0, 0, manager, 500));
            listOfISGO.Add(new Planet("PlanetVenus", "venus.mesh", 0, 0, manager, 1500));
            listOfISGO.Add(new Planet("PlanetEarth", "earth.mesh", 0, 0, manager, 2500));
            listOfISGO.Add(new Planet("PlanetMars", "mars.mesh", 0, 0, manager, 3500));
            listOfISGO.Add(new Planet("PlanetJupiter", "jupiter.mesh", 0, 0, manager, 6000));
            listOfISGO.Add(new Planet("PlanetSaturn", "saturn.mesh", 0, 0, manager, 8000));
            listOfISGO.Add(new Planet("PlanetUranus", "uranus.mesh", 0, 0, manager, 10000));
            listOfISGO.Add(new Planet("PlanetNeptune", "neptune.mesh", 0, 0, manager, 12000));
            #endregion

            listOfISGO.Add(new Planet("FunnyThinks", "knot.mesh", 0, 1, manager, 2000));

            listOfISGO.Add(new Planet("FunnyThinks2", "ninja.mesh", 0, 1, manager, 4000));

            listOfISGO.Add(new Planet("FunnyThinks3", "robot.mesh", 0, 1, manager, 1000));
        }

		private void createSun() {
			listOfISGO.Add(new Sun("Sun", "sun.mesh", 0, manager));
            listOfISGO.Add(new Sun("Sun2", "jupiter.mesh", 1, manager));
		}

		private void createIMGO() {	}


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
