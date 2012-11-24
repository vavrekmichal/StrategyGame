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

        
		public void initializeWorld(string mission){
			//XML Reader on mission will be here                !!!!!!!!!!!!!!!!!!!!!!!!!!!! 
            //TODO list invisible sol. system Manager > rember and watch it
			createISGO();
			createSun();
		}

		public void getInicializedObjects(out List<IStaticGameObject> s, out List<IMovableGameObject> m){
			s= listOfISGO;
			m= listOfIMGO;
		}

       

		private void createISGO() {
            #region The Solar System
            listOfISGO.Add( new Planet("PlanetMercury", "mercury.mesh", 0, 0, manager, 500));
            listOfISGO.Add(new Planet("PlanetVenus", "venus.mesh", 0, 0, manager, 1500));
            listOfISGO.Add(new Planet("PlanetEarth", "earth.mesh", 0, 0, manager, 2500));
            listOfISGO.Add(new Planet("PlanetMars", "mars.mesh", 0, 0, manager, 3500));
            listOfISGO.Add(new Planet("PlanetJupiter", "jupiter.mesh", 0, 0, manager, 6000));
            listOfISGO.Add(new Planet("PlanetSaturn", "saturn.mesh", 0, 0, manager, 8000));
            listOfISGO.Add(new Planet("PlanetUranus", "uranus.mesh", 0, 0, manager, 10000));
            listOfISGO.Add(new Planet("PlanetNeptune", "neptune.mesh", 0, 0, manager, 12000));
            #endregion
        }

		private void createSun() {
			listOfISGO.Add(new Sun("Sun","sun.mesh",0, Mogre.Vector3.ZERO,manager));
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
