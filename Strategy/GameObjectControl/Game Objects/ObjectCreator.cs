using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
using Strategy.GameMaterial;
using Strategy.GameObjectControl.RuntimeProperty;


namespace Strategy.GameObjectControl.Game_Objects {

	enum IsgoType { StaticObject, Sun }

	public class ObjectCreator {

		protected List<SolarSystem> solarSystems;
		protected Dictionary<string, Team> teams;

		protected ObjectLoader loader;


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
		/// <param Name="mission">Name of mission</param>
		public void InitializeWorld(string mission) {

			loader = new ObjectLoader("../../Media/Mission/MyMission.xml", teams, solarSystems);
			loader.Load(mission);

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

	}
}
