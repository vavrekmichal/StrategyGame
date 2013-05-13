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
		/// <param Name="manager">Mogre SceneManager</param>
		public ObjectCreator(Mogre.SceneManager manager) {
			this.manager = manager;
			teams = new Dictionary<string, Team>();
			solarSystems = new List<SolarSystem>();
		}


		/// <summary>
		/// Inicialization of game World
		/// </summary>
		/// <param Name="mission">Name of mission</param>
		public void InitializeWorld(string mission, PropertyManager propMan) {

			createMaterials();

			loader = new ObjectLoader("../../Media/Mission/MyMission.xml", manager, teams, materialList, solarSystems);
			loader.Load(mission, propMan);

			solarSystems[0].ShowSolarSystem();

		}


		public IStaticGameObject CreateIsgo(string typeName, object[] args) {	// prepared...never used
			var isgo =  loader.CreateISGO(typeName, args);
			Game.HitTest.RegisterISGO(isgo);
			return isgo;
		}

		public IMovableGameObject CreateImgo(string typeName, object[] args) {	// prepared...never used
			var imgo = loader.CreateIMGO(typeName, args);
			Game.HitTest.RegisterIMGO(imgo);
			return imgo;
		}

		
		public List<SolarSystem> GetInicializedSolarSystems() {
			return solarSystems;
		}

		public Dictionary<string, Team> GetTeams() {
			return teams;
		}

		public Dictionary<Team, List<Team>> GetTeamsRelations() {
			return loader.GetTeamsRelations();
		}


		//TODO TOTO TU NESMI ZUSTAT*/
		private void createMaterials() {
			materialList = new List<IMaterial>() { new Wolenium(), new Wolenarium(), new Class1() };
		}

	}
}
