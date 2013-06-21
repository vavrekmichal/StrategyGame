using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects.GameLoad;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;


namespace Strategy.GameObjectControl.Game_Objects {
	/// <summary>
	/// Distinguishes two basic kinds of IStaticGameObject
	/// </summary>
	enum IsgoType { StaticObject, Sun }

	/// <summary>
	/// Loads and creates objects from given XML file (XmlLoader).
	/// Also implements IGameObjectCreator interface, so can creates objects during the game (RunTimeCreator).
	/// 
	/// </summary>
	public class ObjectCreator : IGameObjectCreator {

		protected List<SolarSystem> solarSystems;
		protected Dictionary<string, Team> teams;

		//protected NGLoader loader;
		protected XmlLoader xmlLoader;

		/// <summary>
		/// Initializes ObjectCrator
		/// </summary>
		public ObjectCreator() {
			teams = new Dictionary<string, Team>();
			solarSystems = new List<SolarSystem>();
		}


		/// <summary>
		/// Initializes the mission from given file (missionFilePath).
		/// Creates xmlLoader and loads given mission.
		/// Also sets the first SolarSystem as active.
		/// </summary>
		/// <param name="missionFilePath">Tha path to the mission.</param>
		public void InitializeWorld(string missionFilePath) {

			xmlLoader = new XmlLoader(missionFilePath, teams, solarSystems);
			xmlLoader.LoadMission();

			solarSystems[0].ShowSolarSystem();
		}

		/// <summary>
		/// Creates IStaticGameObject by given typeName and argument. Inserts the object to given SolarSystem and registers it in HitTest.
		/// </summary>
		/// <param name="typeName">The type of the creating object.</param>
		/// <param name="args">The arguments of the creating object.</param>
		/// <param name="solSyst">The creating object SolarSystem.</param>
		/// <returns>Returns created IStaticGameObject.</returns>
		public IStaticGameObject CreateIsgo(string typeName, object[] args, SolarSystem solSyst) {	// prepared...never used
			IStaticGameObject isgo = xmlLoader.CreateISGO(typeName, args);
			solSyst.AddISGO(isgo);
			Game.HitTest.RegisterISGO(isgo);
			return isgo;
		}

		/// <summary>
		/// Creates IMovableGameObject by given typeName and argument. Inserts the object to given SolarSystem and registers it in HitTest.
		/// </summary>
		/// <param name="typeName">The type of the creating object.</param>
		/// <param name="args">The arguments of the creating object.</param>
		/// <param name="solSyst">The creating object SolarSystem.</param>
		/// <returns>Returns created IMovableGameObject.</returns>
		public IMovableGameObject CreateImgo(string typeName, object[] args, SolarSystem solSyst) {	// prepared...never used
			IMovableGameObject imgo = xmlLoader.CreateIMGO(typeName, args);
			solSyst.AddIMGO(imgo);
			Game.HitTest.RegisterIMGO(imgo);
			return imgo;
		}

		/// <summary>
		/// Gets unused name from loader an returns it.
		/// </summary>
		/// <param name="name">The base of a object name.</param>
		/// <returns>Returns unused name.</returns>
		public string GetUnusedName(string name) {
			return xmlLoader.GetUnusedName(name);
		}

		/// <summary>
		/// Returns initialized solar systems by loader.
		/// </summary>
		/// <returns>Returns initialized solar systems.</returns>
		public List<SolarSystem> GetInicializedSolarSystems() {
			return solarSystems;
		}

		/// <summary>
		/// Returns initialized teams by loader.
		/// </summary>
		/// <returns>Returns initialized teams.</returns>
		public Dictionary<string, Team> GetTeams() {
			return teams;
		}

		/// <summary>
		/// Returns initialized relations between teams by XmlLoader.
		/// </summary>
		/// <returns>Returns initialized relations between teams.</returns>
		public Dictionary<Team, List<Team>> GetTeamsRelations() {
			return xmlLoader.GetTeamsRelations();
		}

		/// <summary>
		/// Returns loaded moving objects
		/// </summary>
		/// <returns>Returns dictionary with moving targets.</returns>
		public Dictionary<string, string> GetLoadedMovements() {
			return xmlLoader.GetLoadedMovements();
		}
	}
}
