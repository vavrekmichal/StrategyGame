using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.FightMgr;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;
using Strategy.GameMaterial;
using Strategy.TeamControl;
using Strategy.GameGUI;
using Mogre;


namespace Strategy.GameObjectControl {
	class GroupManager {

		//protected HitTest hitTest;

		protected List<GroupMovables> groupMList;

		protected Dictionary<int, SolarSystem> solarSystemBetter;
		protected int lastSolarSystem = 0;

		public bool isMovableGroupActive; //active is movable group

		private GroupMovables selectedGroupM; //not implemented ...will be actual selected group - need rectangular select
		private GroupStatics selectedGroupS;

		private int activeSolarSystem = 0; //now active solarSystem


		#region singlton and constructor
		private static GroupManager instance;
		/// <summary>
		/// Singleton constructor
		/// </summary>
		/// <returns>instance of GroupManager</returns>
		public static GroupManager getInstance() {
			if (instance == null) {
				instance = new GroupManager();
			}
			return instance;
		}


		/// <summary>
		/// Private constructor
		/// </summary>
		private GroupManager() {
			solarSystemBetter = new Dictionary<int, SolarSystem>();
			groupMList = new List<GroupMovables>();
		}
		#endregion


		/// <summary>
		/// Called on frame update
		/// </summary>
		/// <param name="delay">delay between frames</param>
		public void update(float delay) {
			foreach (KeyValuePair<int, SolarSystem> solarSys in solarSystemBetter) {
				solarSys.Value.update(delay);
			}
			Gate.updateTravelers(delay);
		}

		public void createTraveler(int solarSystemNumberTo, object imgo) {
			Gate.createTraveler(getActiveSolarSystem(), getSolarSystem(solarSystemNumberTo), imgo);
		}

		public List<Traveler> getTravelers() {
			return Gate.getTravelers();
		}

		#region solarSyst

		/// <summary>
		/// Get SolarSystem from ObjectCreator as List and creates Dictionary. 
		/// Also initializes HitTest
		/// </summary>
		public void createSolarSystems(List<SolarSystem> solSystList) {
			//inicialization

			foreach (SolarSystem solarSyst in solSystList) {
				solarSystemBetter.Add(lastSolarSystem, solarSyst);
				lastSolarSystem++;
			}
			//hitTest.createHitTestMap(solSystList);
		}

		/// <summary>
		/// Show given solar system and hide actual
		/// </summary>
		/// <param name="newSolarSystem">integer of showing solar system</param>
		public void changeSolarSystem(int newSolarSystem) {
			//better system
			deselectGroup();
			solarSystemBetter[activeSolarSystem].hideSolarSystem();
			solarSystemBetter[newSolarSystem].showSolarSystem();
			//end of it

			activeSolarSystem = newSolarSystem; //set new active solar system  

			GUIControler.getInstance().setSolarSystemName(getSolarSystemName(activeSolarSystem)); //TODO to tu asi nechchi
		}

		public List<string> getAllSolarSystemNames() {
			var list = new List<string>();
			foreach (var ss in solarSystemBetter) {
				list.Add(ss.Value.Name);
			}
			return list;
		}


		public string getSolarSystemName(int numberOfSolarSystem) {
			return solarSystemBetter[numberOfSolarSystem].Name;
		}

		public SolarSystem getActiveSolarSystem() {
			return solarSystemBetter[activeSolarSystem];
		}

		public SolarSystem getSolarSystem(int numberOfSolarSystem) {
			return solarSystemBetter[numberOfSolarSystem];
		}

		#endregion

		/// <summary>
		/// Set all select group as new empty
		/// </summary>
		public void deselectGroup() {
			GroupMovables groupM = new GroupMovables();
			GroupStatics groupS = new GroupStatics();
			isMovableGroupActive = false;
			showSelectedInfoGroup();
		}

		/// <summary>
		/// Creates group (without calling group.select()) from given List with IStaticGameObject
		/// Object from player team has greater priority then others
		/// </summary>
		/// <param name="isgoList">List with IStaticGameObject</param>
		public void createInfoGroup(List<IStaticGameObject> isgoList) {
			isMovableGroupActive = false;
			if (isgoList.Count > 0) {
				var group = new GroupStatics(isgoList[0].Team);
				group.insertMemeber(isgoList[0]);	//insert firt
				var inGroup = isgoList[0];
				if (isgoList.Count > 1) {		//check if there is more object
					for (int i = 1; i < isgoList.Count; i++) {
						if (inGroup.Team.Name == Game.playerName && inGroup.Team == isgoList[i].Team) {
							group.insertMemeber(isgoList[i]); //insert player's isgo	
						} else {
							if (isgoList[i].Team.Name == Game.playerName) { //in some of elements in isgoList is players's -> has greater priority
								group = new GroupStatics(isgoList[i].Team);
								group.insertMemeber(isgoList[i]);	//insert firt
								inGroup = isgoList[i];
							}
						}
					}
				}
				selectedGroupS = group;
			} else {
				selectedGroupS = new GroupStatics();
			}

		}

		/// <summary>
		/// Creates group (without calling group.select()) from given List with IMovableGameObject
		/// Object from player team has greater priority then others
		/// </summary>
		/// <param name="isgoList">List with IMovableGameObject</param>
		public void createInfoGroup(List<IMovableGameObject> imgoList) {

			var group = new GroupMovables(imgoList[0].Team);
			group.insertMemeber(imgoList[0]);
			isMovableGroupActive = true;

			if (imgoList.Count > 1) {		//check if there is more object
				for (int i = 1; i < imgoList.Count; i++) {
					if (group.OwnerTeam.Name == Game.playerName && group.OwnerTeam == imgoList[i].Team) {
						group.insertMemeber(imgoList[i]); //insert player's imgo	
					} else {
						if (imgoList[i].Team.Name == Game.playerName) { //in some of elements in isgoList is players's -> has greater priority
							group = new GroupMovables(imgoList[i].Team);
							group.insertMemeber(imgoList[i]);	//insert firt
						}
					}
				}
			}
			selectedGroupM = group;
		}

		public void showSelectedInfoGroup() {
			if (isMovableGroupActive) {
				GUIControler.getInstance().showTargeted(selectedGroupM);
			} else {
				GUIControler.getInstance().showTargeted(selectedGroupS);
			}
		}

		public Team getActiveTeam() {
			if (isMovableGroupActive) {
				return selectedGroupM.OwnerTeam;
			} else {
				return selectedGroupS.OwnerTeam;
			}
		}

		public ActionAnswer onRightMouseClick(Mogre.Vector3 clickedPoint, MovableObject hitObject, bool isFriendly, bool isImgo) {

			if (isMovableGroupActive && selectedGroupM.OwnerTeam.Name == Game.playerName) {
				return selectedGroupM.onMouseAction(ActionReason.onRightButtonClick, clickedPoint, hitObject, isFriendly, isImgo);
			} else {
				return ActionAnswer.None;
			}

		}


		public GroupMovables getActiveMovableGroup() {
			return selectedGroupM;
		}
		///// <summary>
		///// Called from GUI when right mouse button click in game area
		///// </summary>
		///// <param name="clickedPoint">mouse position</param>
		///// <param name="selectedObjects">objects in clicked area</param>
		//public void rightClick(Mogre.Vector3 clickedPoint, List<Mogre.MovableObject> selectedObjects) {
		//	if (activeMGroup) {
		//		if (selectedGroupM.OwnerTeam.Name == Game.playerName) {
		//			selectedGroupM.select();
		//			Mogre.MovableObject hitObject;
		//			bool isFriendly = true;
		//			bool isIMGO = true;
		//			if (selectedObjects.Count == 0) {
		//				hitObject = null;
		//			} else {
		//				hitObject = selectedObjects[0];
		//				Team targetTeam;
		//				if (hitTest.isObjectMovable(hitObject.Name)) {
		//					targetTeam = hitTest.getIMGO(hitObject.Name).Team;

		//				} else {
		//					targetTeam = hitTest.getISGO(hitObject.Name).Team;
		//					isIMGO = false;
		//				}
		//				isFriendly = teamMgr.areFriendly(selectedGroupM.OwnerTeam, targetTeam);

		//			}
		//			var answer = selectedGroupM.onMouseAction(ActionReason.onRightButtonClick, clickedPoint, hitObject, isFriendly, isIMGO);

		//			switch (answer) {
		//				case ActionAnswer.None:
		//					break;
		//				case ActionAnswer.Attack:
		//					break;
		//				case ActionAnswer.Move:
		//					moveMgr.goToLocation(selectedGroupM, clickedPoint);
		//					break;
		//				case ActionAnswer.MoveTo:
		//					moveMgr.goToTarget(selectedGroupM, hitTest.getISGO(hitObject.Name));
		//					break;
		//				case ActionAnswer.RunAway:
		//					break;
		//				default:
		//					break;
		//			}

		//		}
		//	} else {	//StaticGroup 
		//		if (selectedGroupS.OwnerTeam.Name == Game.playerName) { //do st.

		//		}
		//	}
		//}


	}


}
