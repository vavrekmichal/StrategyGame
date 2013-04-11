using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.GameGUI;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl {
	class GameObjectManager {

		protected ObjectCreator objectCreator;
		protected GUIControler guiControler;
		protected IMoveManager moveMgr;
		protected PropertyManager propertyMgr;
		protected TeamManager teamMgr;
		protected GroupManager groupMgr;
		protected HitTest hitTest;

		#region singleton and constructor
		private static GameObjectManager instance;

		/// <summary>
		/// Singleton instance
		/// </summary>
		/// <returns>returning singleton instance</returns>
		public static GameObjectManager getInstance(SceneManager sceneMgr) {
			if (instance == null) {
				instance = new GameObjectManager(sceneMgr);
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
		/// private constructor
		/// </summary>
		private GameObjectManager(SceneManager sceneMgr) {
			teamMgr = TeamManager.getInstance();
			objectCreator = ObjectCreator.getInstance(sceneMgr);
			moveMgr = MoveManager.getInstance();
			groupMgr = GroupManager.getInstance();
			propertyMgr = new PropertyManager("StartMission");
			hitTest = new HitTest();
		}
		#endregion


		#region private

		#endregion



		#region public
		//SETHITTEST
		public void setGUI(GUIControler gui) {
			guiControler = gui;
		}

		public void update(float delay) {
			teamMgr.update();
			groupMgr.update(delay);
			moveMgr.update(delay);
		}

		/// <summary>
		/// inicialization of managers, hittest...
		/// </summary>
		/// <param name="missionName">Name of choosen mission</param>
		public void inicialization(string missionName) {
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
		/// <param name="selectedObjects">objects in clicked area</param>
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
		/// <param name="clickedPoint">mouse position</param>
		/// <param name="selectedObjects">objects in clicked area</param>
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
				isFriendly = teamMgr.areFriendly(groupMgr.getActiveTeam(), targetTeam);

			}
			var answer = groupMgr.onRightMouseClick(clickedPoint, hitObject, isFriendly, isIMGO);

			switch (answer) {
				case ActionAnswer.Move:
					moveMgr.goToLocation(groupMgr.getActiveMovableGroup(), clickedPoint);
					break;
				case ActionAnswer.MoveTo:
					moveMgr.goToTarget(groupMgr.getActiveMovableGroup(), hitTest.getISGO(hitObject.Name));
					break;

			}
			groupMgr.showSelectedInfoGroup(); //TODO delete volano jen jako gui update pro kontrolu

		}


		#endregion
	}
}
