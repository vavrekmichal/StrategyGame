using System;
using System.Collections.Generic;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;
using Strategy.TeamControl;
using Strategy.FightMgr;
using Strategy.GameObjectControl.Game_Objects.GameSave;

namespace Strategy.GameObjectControl {
	/// <summary>
	/// Checks and updates the control elements of the game and allow access to them.
	/// </summary>
	public class GameObjectManager {

		protected ObjectCreator objectCreator;
		protected IMoveManager moveMgr;
		protected IFightManager fightMgr;
		protected PropertyManager propertyMgr;
		protected TeamManager teamMgr;
		protected GroupManager groupMgr;
		protected SolarSystemManager solarSystemMgr;
		protected HitTest hitTest;
		protected GameSerializer gameSerializer;

		const string groupSelectedSound = "power1.wav";

		#region singleton and constructor
		private static GameObjectManager instance;

		/// <summary>
		/// Returns the singleton instance of the GameObjectManager (if is the instance null, so initializes).
		/// </summary>
		/// <returns>Returning the singleton instance.</returns>
		public static GameObjectManager GetInstance() {
			if (instance == null) {
				instance = new GameObjectManager();
			}
			return instance;
		}

		/// <summary>
		/// Initializes the object and all control elements of the game.
		/// </summary>
		private GameObjectManager() {
			teamMgr = new TeamManager();
			fightMgr = new FightManager();
			objectCreator = new ObjectCreator();
			moveMgr = new MoveManager();	
			groupMgr = new GroupManager();
			propertyMgr = new PropertyManager();
			hitTest = new HitTest();
			solarSystemMgr = new SolarSystemManager();
			gameSerializer = new GameSerializer();
		}
		#endregion

		/// <summary>
		/// Returns instance of GroupManager (if it is not initialize throws a exception).
		/// </summary>
		public GroupManager GroupManager {
			get {
				if (groupMgr == null) {
					throw new NullReferenceException("GroupManager is not initialized.");
				}
				return groupMgr; ;
			}
		}

		/// <summary>
		/// Returns instance of IGameObjectCreator (if it is not initialize throws a exception).
		/// </summary>
		public IGameObjectCreator IGameObjectCreator {
			get {
				if (objectCreator == null) {
					throw new NullReferenceException("IGameObjectCreator is not initialized.");
				}
				return objectCreator; ;
			}
		}

		/// <summary>
		/// Returns instance of HitTest (if it is not initialize throws a exception). 
		/// </summary>
		public HitTest HitTest {
			get {
				if (hitTest == null) {
					throw new NullReferenceException("HitTest is not initialized.");
				}
				return hitTest; ;
			}
		}

		/// <summary>
		/// Returns instance of IMoveManager (if it is not initialize throws a exception). 
		/// </summary>
		public IMoveManager IMoveManager {
			get {
				if (moveMgr == null) {
					throw new NullReferenceException("MoveManager is not initialized.");
				}
				return moveMgr;
			}
		}

		/// <summary>
		/// Returns instance of IFightManager (if it is not initialize throws a exception). 
		/// </summary>
		public IFightManager IFightManager {
			get {
				if (fightMgr == null) {
					throw new NullReferenceException("IFightManager is not initialized.");
				}
				return fightMgr;
			}
		}

		/// <summary>
		/// Returns instance of PropertyManager (if it is not initialize throws a exception). 
		/// </summary>
		public PropertyManager PropertyManager {
			get {
				if (propertyMgr == null) {
					throw new NullReferenceException("PropertyManager is not initialized.");
				}
				return propertyMgr;
			}
		}

		/// <summary>
		/// Returns instance of TeamManager (if it is not initialize throws a exception). 
		/// </summary>
		public TeamManager TeamManager {
			get {
				if (teamMgr == null) {
					throw new NullReferenceException("TeamManager is not initialized.");
				}
				return teamMgr;
			}
		}

		/// <summary>
		/// Returns instance of SolarSystemManager (if it is not initialize throws a exception).
		/// </summary>
		public SolarSystemManager SolarSystemManager {
			get {
				if (solarSystemMgr == null) {
					throw new NullReferenceException("SolarSystemManager is not initialized.");
				}
				return solarSystemMgr;
			}
		}

		/// <summary>
		/// Returns instance of SolarSystemManager (if it is not initialize throws a exception).
		/// </summary>
		public GameSerializer GameSerializer {
			get {
				if (gameSerializer == null) {
					throw new NullReferenceException("GameSerializer is not initialized.");
				}
				return gameSerializer;
			}
		}

		#region private

		/// <summary>
		/// Ensures delete of the death game object and prints information about its death to game console.
		/// </summary>
		/// <param name="igo">The death game object.</param>
		/// <param name="m">The arguments contains information about death.</param>
		private void OnDieEvent(IGameObject igo, MyDieArgs m) {
			Game.PrintToGameConsole(igo.Name + " destroyed (Team " + igo.Team.Name + ").");
			RemoveObject(igo);
		}
		#endregion

		#region public

		/// <summary>
		/// Updates the control elements of the game (updates all game objects -SolarSystemManager, fights,
		/// groups and controlled movements).
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			solarSystemMgr.Update(delay);
			fightMgr.Update(delay);
			groupMgr.Update(delay);
			moveMgr.Update();
		}
		
		/// <summary>
		/// Removes the object from the game (from TeamManager and from GroupManager).
		/// </summary>
		/// <param name="gameObject">The removing game object.</param>
		public void RemoveObject(IGameObject gameObject) {

			var castedImgo = gameObject as IMovableGameObject;
			if (castedImgo != null) {
				// Removes movebla object
				teamMgr.RemoveFromOwnTeam(castedImgo);
				groupMgr.DestroyGameObject(castedImgo);

			} else {
				// Removes static object
				var castedIsgo = gameObject as IStaticGameObject;
				teamMgr.RemoveFromOwnTeam(castedIsgo);
				groupMgr.DestroyGameObject(castedIsgo);
			}
		}

		/// <summary>
		/// Changes the team of the given gameObject. Sets the newTeam as the objects team (ChangeTeam) and 
		/// removes it from actual group (has new owner). 
		/// </summary>
		/// <param name="gameObject">The object which is changing team.</param>
		/// <param name="newTeam">The new team of the object.</param>
		public void ChangeObjectsTeam(IGameObject gameObject, Team newTeam) {
			var castedImgo = gameObject as IMovableGameObject;
			if (castedImgo != null) {
				groupMgr.RemoveFromGroup(castedImgo);
				teamMgr.ChangeTeam(castedImgo, newTeam);
			} else {
				var castedIsgo = gameObject as IStaticGameObject;
				groupMgr.RemoveFromGroup(castedIsgo);
				teamMgr.ChangeTeam(castedIsgo, newTeam);
			}
		}

		/// <summary>
		/// Destroys the current game (mission) and sets a new empty control elements of the game.
		/// </summary>
		public void DestroyGame() {
			// Destroys targeted group (destroys pointers).
			groupMgr.UntargetGroup();

			// Destroys all object in the game.
			foreach (var item in objectCreator.GetInicializedSolarSystems()) {
				item.Destroy();
			}

			// Creates a new empty control elements of the game.
			teamMgr = new TeamManager();
			fightMgr = new FightManager();
			objectCreator = new ObjectCreator();
			moveMgr = new MoveManager();
			groupMgr = new GroupManager();
			propertyMgr = new PropertyManager();
			hitTest = new HitTest();
			solarSystemMgr = new SolarSystemManager();
		}

		/// <summary>
		/// Initializes control elements of the game. Loads given mission, creates all required game objects 
		/// (and sets DieHandler) and registers them to control elements.
		/// </summary>
		/// <param name="missionName">The name of the choosen mission.</param>
		public void Initialize(string missionName) {
			// Creates game objects
			objectCreator.InitializeWorld(missionName);

			// Sets DieHandler
			foreach (var solarSys in objectCreator.GetInicializedSolarSystems()) {
				foreach (var gameObject in solarSys.GetIMGOs()) {
					gameObject.Value.DieHandler += OnDieEvent;
				}
				foreach (var gameObject in solarSys.GetISGOs()) {
					gameObject.Value.DieHandler += OnDieEvent;
				}
			}
			// Registers SolarSystems
			solarSystemMgr.CreateSolarSystems(objectCreator.GetInicializedSolarSystems());
			// Initializes HitTest
			hitTest.CreateHitTestMap(objectCreator.GetInicializedSolarSystems());
			// Registers team
			teamMgr.Initialize(objectCreator.GetTeams(), objectCreator.GetTeamsRelations());
			groupMgr.UntargetGroup();
			// Initializes movements and fights (occupations) from XML file
			moveMgr.Initialize(objectCreator.GetLoadedMovements());
			fightMgr.Initialize( objectCreator.GetLoadedOccupations(),objectCreator.GetLoadedFights());

			gameSerializer.Initialize(missionName);
		}

		/// <summary>
		/// Processes the list and creates  the group from it. The movable object has a bigger priority,
		/// so when the list contains both types (IMovableGameObject and IStaticGameObject) the movable 
		/// group is created.
		/// </summary>
		/// <param Name="selectedObjects">Objects in clicked area</param>
		public void OnLeftClick(List<Mogre.MovableObject> selectedObjects) {
			bool isMovableSelected = false;
			List<IMovableGameObject> imgoList = new List<IMovableGameObject>();
			List<IStaticGameObject> isgoList = new List<IStaticGameObject>();

			foreach (var gameObject in selectedObjects) {
				if (!hitTest.IsObjectControllable(gameObject.Name)) continue;
				if (hitTest.IsObjectMovable(gameObject.Name)) {
					isMovableSelected = true;
					imgoList.Add(hitTest.GetIMGO(gameObject.Name));
				} else {
					if (isMovableSelected == false) {
						isgoList.Add(hitTest.GetISGO(gameObject.Name));
					}
				}
			}

			// Movables has bigger priority.
			if (isMovableSelected) {
				groupMgr.CreateInfoGroup(imgoList);
			} else {
				groupMgr.CreateInfoGroup(isgoList);
			}
		}

		/// <summary>
		/// Selectes the targeted group and finds out info about object where the mouse clicked.
		/// After that calls OnRightMouseClick on group and depending on the response is called 
		/// the appropriate action. 
		/// </summary>
		/// <param Name="clickedPoint">The mouse position.</param>
		/// <param Name="selectedObjects">The objects in clicked area.</param>
		public void OnRightClick(Mogre.Vector3 clickedPoint, List<Mogre.MovableObject> selectedObjects) {

			Mogre.MovableObject hitObject;
			bool isFriendly = true;
			bool isIMGO = true;

			if (selectedObjects.Count == 0) {
				// Nothing at clicked area.
				hitObject = null;
			} else {
				hitObject = selectedObjects[0];
				Team targetTeam;
				// The clicked object is not controllable.
				if (!hitTest.IsObjectControllable(hitObject.Name)) return;

				if (hitTest.IsObjectMovable(hitObject.Name)) {
					// The clicked object is movable.
					targetTeam = hitTest.GetIMGO(hitObject.Name).Team;
				} else {
					// The clicked object is static.
					targetTeam = hitTest.GetISGO(hitObject.Name).Team;
					isIMGO = false;
				}

				// Checks if the object is friendly.
				isFriendly = teamMgr.AreFriendly(groupMgr.ActiveTeam, targetTeam);
			}

			// Gets the selected group answer on mouse action (clicked position).
			var answer = groupMgr.SelectInfoGroup(clickedPoint, hitObject, isFriendly, isIMGO);

			Game.IEffectPlayer.PlayEffect(groupSelectedSound); // Plays effect

			// Chooses the next action by the group reply.
			switch (answer) {
				case ActionAnswer.Move:
					Game.PrintToGameConsole("Group from team " + groupMgr.GetActiveMovableGroup().Team.Name + " moving to " + clickedPoint.ToString()); //todo delete
					moveMgr.GoToLocation(groupMgr.GetActiveMovableGroup(), clickedPoint);
					break;
				case ActionAnswer.MoveTo:
					moveMgr.GoToTarget(groupMgr.GetActiveMovableGroup(), hitTest.GetGameObject(hitObject.Name));
					break;
				case ActionAnswer.Attack:
					fightMgr.Attack(groupMgr.GetActiveMovableGroup(), hitTest.GetGameObject(hitObject.Name));
					break;
				case ActionAnswer.Occupy:
					fightMgr.Occupy(groupMgr.GetActiveMovableGroup(), hitTest.GetGameObject(hitObject.Name));
					break;
			}
		}
		#endregion
	}
}
