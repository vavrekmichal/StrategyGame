using System;
using System.Collections.Generic;
using Mogre;
using Strategy.GameGUI;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;
using Strategy.TeamControl;
using MOIS;
using Strategy.FightMgr;

namespace Strategy.GameObjectControl {
	public class GameObjectManager {

		protected ObjectCreator objectCreator;
		protected IMoveManager moveMgr;
		protected IFightManager fightMgr;
		protected PropertyManager propertyMgr;
		protected TeamManager teamMgr;
		protected GroupManager groupMgr;
		protected HitTest hitTest;

		const string groupSelectedSound = "power1.wav";

		#region singleton and constructor
		private static GameObjectManager instance;

		/// <summary>
		/// Singleton instance
		/// </summary>
		/// <returns>Returning singleton instance</returns>
		public static GameObjectManager GetInstance(SceneManager sceneMgr, Mouse m, Keyboard k, RenderWindow mWindow) {
			if (instance == null) {
				instance = new GameObjectManager(sceneMgr, m, k, mWindow);
			}
			return instance;
		}

		public static GameObjectManager GetInstance() {
			if (instance == null) {
				throw new NullReferenceException("GameObjectManager is not initialized.");
			}
			return instance;
		}

		/// <summary>
		/// Private constructor
		/// </summary>
		private GameObjectManager(SceneManager sceneMgr, Mouse m, Keyboard k, RenderWindow mWindow) {
			teamMgr = TeamManager.GetInstance();
			objectCreator = new ObjectCreator();
			moveMgr = new MoveManager();
			fightMgr = new FightManager();
			groupMgr = new GroupManager();
			propertyMgr = new PropertyManager("StartMission");
			hitTest = new HitTest();
		}
		#endregion

		public GroupManager GroupManager {
			get {
				if (groupMgr == null) {
					throw new NullReferenceException("GroupManager is not initialized.");
				}
				return groupMgr; ;
			}
		}

		public HitTest HitTest {
			get {
				if (hitTest == null) {
					throw new NullReferenceException("HitTest is not initialized.");
				}
				return hitTest; ;
			}
		}

		public IMoveManager IMoveManager {
			get {
				if (moveMgr == null) {
					throw new NullReferenceException("MoveManager is not initialized.");
				}
				return moveMgr; 
			}
		}

		public IFightManager IFightManager {
			get {
				if (fightMgr == null) {
					throw new NullReferenceException("IFightManager is not initialized.");
				}
				return fightMgr;
			}
		}

		public PropertyManager PropertyManager {
			get {
				if (propertyMgr == null) {
					throw new NullReferenceException("PropertyManager is not initialized.");
				}
				return propertyMgr;
			}
		}

		public TeamManager TeamManager {
			get {
				if (teamMgr == null) {
					throw new NullReferenceException("TeamManager is not initialized.");
				}
				return teamMgr;
			}
		}

		#region private

		#endregion



		#region public

		public void Update(float delay) {
			fightMgr.Update(delay);
			groupMgr.Update(delay);
			moveMgr.Update();
		}

		private void OnDieEvent(IGameObject igo, MyDieArgs m) {
			Console.WriteLine(
				"Object Hp is lower then 0.");
			Game.PrintToGameConsole(igo.Name+" destroyed (Team "+igo.Team.Name+").");
			RemoveObject(igo);
		} 


		public void RemoveObject(IGameObject gameObject) {

			//gameObject.takeDamage(1000); //TODO asdasdas

			var castedImgo = gameObject as IMovableGameObject;
			if (castedImgo != null) {
				teamMgr.RemoveFromOwnTeam(castedImgo);
				groupMgr.DestroyGameObject(castedImgo);

			} else {
				var castedIsgo = gameObject as IStaticGameObject;
				teamMgr.RemoveFromOwnTeam(castedIsgo);
				groupMgr.DestroyGameObject(castedIsgo);
			}
		}

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
		/// Inicialization of managers, hittest...
		/// </summary>
		/// <param Name="missionName">Name of choosen mission</param>
		public void Inicialization(string missionName) {
			objectCreator.InitializeWorld(missionName);

			foreach (var solarSys in objectCreator.GetInicializedSolarSystems()) {
				foreach (var gameObject in solarSys.GetIMGOs()) {
					gameObject.Value.DieHandler += OnDieEvent;
				}
				foreach (var gameObject in solarSys.GetISGOs()) {
					gameObject.Value.DieHandler += OnDieEvent;
				}
			}

			groupMgr.CreateSolarSystems(objectCreator.GetInicializedSolarSystems());
			hitTest.CreateHitTestMap(objectCreator.GetInicializedSolarSystems());
			teamMgr.Inicialization(objectCreator.GetTeams(), objectCreator.GetTeamsRelations());
			groupMgr.DeselectGroup();
		}

		/// <summary>
		/// Called from GUI when a left mouse button click or a rectangular Select in game area 
		/// </summary>
		/// <param Name="selectedObjects">Objects in clicked area</param>
		public void OnLeftClick(List<Mogre.MovableObject> selectedObjects) {
			bool isMovableSelected = false;
			List<IMovableGameObject> imgoList = new List<IMovableGameObject>();
			List<IStaticGameObject> isgoList = new List<IStaticGameObject>();

			foreach (var gameObject in selectedObjects) {
				if (!hitTest.IsObjectControlable(gameObject.Name)) return;
				if (hitTest.IsObjectMovable(gameObject.Name)) {
					isMovableSelected = true;
					imgoList.Add(hitTest.GetIMGO(gameObject.Name));
				} else {
					if (isMovableSelected == false) {
						isgoList.Add(hitTest.GetISGO(gameObject.Name));
					}
				}
			}

			if (isMovableSelected) {
				groupMgr.CreateInfoGroup(imgoList);
			} else {
				groupMgr.CreateInfoGroup(isgoList);
			}
			groupMgr.ShowSelectedInfoGroup();
		}

		/// <summary>
		/// Called from GUI when right mouse button click in game area. Function selectes group and
		/// evaluate the place where user clicked. After that is called OnRightMouseClick on group 
		/// and depending on the response is called the appropriate action. 
		/// </summary>
		/// <param Name="clickedPoint">Mouse position</param>
		/// <param Name="selectedObjects">Objects in clicked area</param>
		public void OnRightClick(Mogre.Vector3 clickedPoint, List<Mogre.MovableObject> selectedObjects) {

			Mogre.MovableObject hitObject;
			bool isFriendly = true;
			bool isIMGO = true;


			if (selectedObjects.Count == 0) {
				hitObject = null;
			} else {
				hitObject = selectedObjects[0];
				Team targetTeam;
				if (!hitTest.IsObjectControlable(hitObject.Name)) return;
				if (hitTest.IsObjectMovable(hitObject.Name)) {
					targetTeam = hitTest.GetIMGO(hitObject.Name).Team;

				} else {
					targetTeam = hitTest.GetISGO(hitObject.Name).Team;
					isIMGO = false;
				}
				isFriendly = teamMgr.AreFriendly(groupMgr.ActiveTeam, targetTeam);
			}
			var answer = groupMgr.SelectInfoGroup(clickedPoint, hitObject, isFriendly, isIMGO);

			Game.IEffectPlayer.PlayEffect(groupSelectedSound); // Play effect

			switch (answer) {
				case ActionAnswer.Move:
					Game.PrintToGameConsole("Group from team " + groupMgr.GetActiveMovableGroup().OwnerTeam.Name + " moving to " + clickedPoint.ToString()); //todo delete
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
			groupMgr.ShowSelectedInfoGroup();

		}


		#endregion
	}
}
