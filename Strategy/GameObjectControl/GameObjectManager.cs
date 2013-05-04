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
	class GameObjectManager {

		protected ObjectCreator objectCreator;
		protected IMoveManager moveMgr;
		protected IFightManager fightMgr;
		protected PropertyManager propertyMgr;
		protected TeamManager teamMgr;
		protected GroupManager groupMgr;
		protected HitTest hitTest;

		#region singleton and constructor
		private static GameObjectManager instance;

		/// <summary>
		/// Singleton instance
		/// </summary>
		/// <returns>Returning singleton instance</returns>
		public static GameObjectManager getInstance(SceneManager sceneMgr, Mouse m, Keyboard k, RenderWindow mWindow) {
			if (instance == null) {
				instance = new GameObjectManager(sceneMgr, m, k, mWindow);
			}
			return instance;
		}

		public static GameObjectManager getInstance() {
			if (instance == null) {
				throw new NullReferenceException("GameObjectManager is not initialized.");
			}
			return instance;
		}

		/// <summary>
		/// Private constructor
		/// </summary>
		private GameObjectManager(SceneManager sceneMgr, Mouse m, Keyboard k, RenderWindow mWindow) {
			teamMgr = TeamManager.getInstance();
			objectCreator = new ObjectCreator(sceneMgr);
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

		#region private

		#endregion



		#region public

		public void update(float delay) {
			teamMgr.update();
			fightMgr.update(delay);
			groupMgr.update(delay);
			moveMgr.update();
		}

		public void changeObjectsTeam(object gameObject, Team newTeam) {
			var castedGO = gameObject as IMovableGameObject;
			if (castedGO != null) {
				groupMgr.removeFromGroup(castedGO);
				teamMgr.changeTeam(castedGO, newTeam);
			} else {
				var castedGOS = gameObject as IStaticGameObject;
				groupMgr.removeFromGroup(castedGOS);
				teamMgr.changeTeam(castedGOS, newTeam);
			}
		}

		/// <summary>
		/// Inicialization of managers, hittest...
		/// </summary>
		/// <param name="missionName">Name of choosen mission</param>
		public void inicialization(string missionName, GUIControler guiControler) {
			objectCreator.initializeWorld(missionName, propertyMgr);
			groupMgr.createSolarSystems(objectCreator.getInicializedSolarSystems());
			hitTest.createHitTestMap(objectCreator.getInicializedSolarSystems());
			teamMgr.setGUI(guiControler);
			teamMgr.inicialization(objectCreator.getTeams(), objectCreator.getTeamsRelations());
			guiControler.inicialization(teamMgr.playerTeam.getMaterials());
			groupMgr.deselectGroup();
		}

		/// <summary>
		/// Called from GUI when a left mouse button click or a rectangular select in game area 
		/// </summary>
		/// <param name="selectedObjects">Objects in clicked area</param>
		public void leftClick(List<Mogre.MovableObject> selectedObjects) {
			bool isMovableSelected = false;
			List<IMovableGameObject> imgoList = new List<IMovableGameObject>();
			List<IStaticGameObject> isgoList = new List<IStaticGameObject>();

			foreach (var gameObject in selectedObjects) {
				if (hitTest.isObjectMovable(gameObject.Name)) {
					isMovableSelected = true;
					imgoList.Add(hitTest.getIMGO(gameObject.Name));
				} else {
					if (isMovableSelected == false) {
						isgoList.Add(hitTest.getISGO(gameObject.Name));
					}
				}
			}

			if (isMovableSelected) {
				groupMgr.createInfoGroup(imgoList);
			} else {
				groupMgr.createInfoGroup(isgoList);
			}
			groupMgr.showSelectedInfoGroup();
		}

		/// <summary>
		/// Called from GUI when right mouse button click in game area
		/// </summary>
		/// <param name="clickedPoint">Mouse position</param>
		/// <param name="selectedObjects">Objects in clicked area</param>
		public void rightClick(Mogre.Vector3 clickedPoint, List<Mogre.MovableObject> selectedObjects) {

			Mogre.MovableObject hitObject;
			bool isFriendly = true;
			bool isIMGO = true;


			if (selectedObjects.Count == 0) {
				hitObject = null;
			} else {
				hitObject = selectedObjects[0];
				Team targetTeam;
				if (hitTest.isObjectMovable(hitObject.Name)) {
					targetTeam = hitTest.getIMGO(hitObject.Name).Team;

				} else {
					targetTeam = hitTest.getISGO(hitObject.Name).Team;
					isIMGO = false;
				}
				isFriendly = teamMgr.areFriendly(groupMgr.ActiveTeam, targetTeam);

			}
			var answer = groupMgr.onRightMouseClick(clickedPoint, hitObject, isFriendly, isIMGO);

			switch (answer) {
				case ActionAnswer.Move:
					moveMgr.goToLocation(groupMgr.getActiveMovableGroup(), clickedPoint);
					break;
				case ActionAnswer.MoveTo:
					moveMgr.goToTarget(groupMgr.getActiveMovableGroup(), hitTest.getGameObject(hitObject.Name));
					break;
				case ActionAnswer.Attack:
					fightMgr.attack(groupMgr.getActiveMovableGroup(), hitTest.getGameObject(hitObject.Name));
					break;
				case ActionAnswer.Occupy:
					fightMgr.occupy(groupMgr.getActiveMovableGroup(), hitTest.getGameObject(hitObject.Name));
					break;

			}
			groupMgr.showSelectedInfoGroup();

		}


		#endregion
	}
}
