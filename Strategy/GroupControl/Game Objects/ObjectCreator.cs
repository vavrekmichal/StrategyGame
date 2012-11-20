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

		public static ObjectCreator getInstance(Mogre.SceneManager manager) {
			if (instance == null) {
				instance = new ObjectCreator(manager);
			}
			return instance;
		}

		private ObjectCreator(Mogre.SceneManager manager) {
			this.manager = manager;
			listOfISGO = new List<IStaticGameObject>();
			listOfIMGO = new List<IMovableGameObject>();
		}

		public void initializeWorld(string mission){
			//XML Reader on mission here
			createISGO();
			createSun();
		}

		public void getInicializedObjects(out List<IStaticGameObject> s, out List<IMovableGameObject> m){
			s= listOfISGO;
			m= listOfIMGO;
		}

		private void createISGO() {
			listOfISGO.Add( new Planet("Planet" + RandomUtil.GetRandomString(), "mercury.mesh",0, 0, new Mogre.Vector3(200, 0, 200), manager));
		}

		private void createSun() {
			listOfISGO.Add(new Sun("Sun","earth.mesh",0, Mogre.Vector3.ZERO,manager));
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
