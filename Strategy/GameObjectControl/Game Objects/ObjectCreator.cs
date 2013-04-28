using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
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


		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param name="manager">Mogre SceneManager</param>
		public ObjectCreator(Mogre.SceneManager manager) {
			this.manager = manager;
			teams = new Dictionary<string, Team>();
			solarSystems = new List<SolarSystem>();
		}


		/// <summary>
		/// Inicialization of game World
		/// </summary>
		/// <param name="mission">Name of mission</param>
		public void initializeWorld(string mission, PropertyManager propMan) {

			createMaterials();

			loader = new ObjectLoader("../../Media/Mission/MyMission.xml", manager, teams, materialList, solarSystems);
			loader.load(mission, propMan);

			solarSystems[0].showSolarSystem();

		}


		public IStaticGameObject createIsgo(string typeName, object[] args) {	// prepared...never used
			var isgo =  loader.createISGO(typeName, args);
			Game.HitTest.registerISGO(isgo);
			return isgo;
		}

		public IMovableGameObject createImgo(string typeName, object[] args) {	// prepared...never used
			var imgo = loader.createIMGO(typeName, args);
			Game.HitTest.registerIMGO(imgo);
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
