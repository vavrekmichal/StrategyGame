using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
using Strategy.GameMaterial;
using Strategy.GameObjectControl.RuntimeProperty;


namespace Strategy.GameObjectControl.Game_Objects {

	enum IsgoType { StaticObject, Sun }

	public class ObjectCreator : IGameObjectCreator {

		protected List<SolarSystem> solarSystems;
		protected Dictionary<string, Team> teams;

		protected NGLoader loader;


		/// <summary>
		/// Public constructor
		/// </summary>
		/// <param Name="manager">Mogre SceneManager</param>
		public ObjectCreator() {
			teams = new Dictionary<string, Team>();
			solarSystems = new List<SolarSystem>();
		}


		/// <summary>
		/// Inicialization of game World
		/// </summary>
		/// <param Name="missionFilePath">Path to file with mission data</param>
		public void InitializeWorld(string missionFilePath) {

			loader = new NGLoader(missionFilePath, teams, solarSystems);
			loader.Load(missionFilePath);

			solarSystems[0].ShowSolarSystem();

		}


		public IStaticGameObject CreateIsgo(string typeName, object[] args, SolarSystem solSyst) {	// prepared...never used
			var isgo =  loader.CreateISGO(typeName, args);
			solSyst.AddISGO(isgo);
			Game.HitTest.RegisterISGO(isgo);
			return isgo;
		}

		public IMovableGameObject CreateImgo(string typeName, object[] args, SolarSystem solSyst) {	// prepared...never used
			var imgo = loader.CreateIMGO(typeName, args);
			solSyst.AddIMGO(imgo);
			Game.HitTest.RegisterIMGO(imgo);
			return imgo;
		}

		public string GetUnusedName(string name) {
			return loader.GetUnusedName(name);
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

	}
}
