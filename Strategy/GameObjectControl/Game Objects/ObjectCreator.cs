using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
using Mogre;
using Strategy.GameMaterial;
using Strategy.GameObjectControl.RuntimeProperty;


namespace Strategy.GameObjectControl.Game_Objects {

	enum IsgoType { StaticObject, Sun }

	class ObjectCreator {

		protected List<IMaterial> materialList;
		protected List<SolarSystem> solarSystems;
		protected Mogre.SceneManager manager;
		protected Dictionary<string, Team> teams;

		protected ObjectLoader loader;

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
			teams = new Dictionary<string, Team>();
			solarSystems = new List<SolarSystem>();
		}
		#endregion

		/// <summary>
		/// inicialization of game World
		/// </summary>
		/// <param name="mission">name of mission</param>
		public void initializeWorld(string mission, PropertyManager propMan) {
			//visual part 
			createMaterials();

			loader = new ObjectLoader("../../Media/Mission/MyMission.xml", manager, teams, materialList, solarSystems);
			loader.load(mission, propMan);
			//createObjectMap(); //map for hittest
			solarSystems[0].showSolarSystem();

		}


		public IStaticGameObject createIsgo(string typeName, object[] args) {	//prepared...never used
			var isgo =  loader.createISGO(typeName, args);
			HitTest.getInstance().registerISGO(isgo);
			return isgo;
		}

		public IMovableGameObject createImgo(string typeName, object[] args) {	//prepared...never used
			var imgo = loader.createIMGO(typeName, args);
			HitTest.getInstance().registerIMGO(imgo);
			return imgo;
		}

		
		public List<SolarSystem> getInicializedSolarSystems() {
			return solarSystems;
		}

		public Dictionary<string, Team> getTeams() {
			return teams;
		}

		public Dictionary<Team, List<Team>> getTeamsRelations() {
			return loader.getTeamsRelations();
		}


		//TODO TOTO TU NESMI ZUSTAT*/
		private void createMaterials() {
			materialList = new List<IMaterial>() { new Wolenium(), new Wolenarium(), new Class1() };
		}

	}
}
