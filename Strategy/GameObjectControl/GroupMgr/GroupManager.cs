using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.FightMgr;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.MoveMgr;
using Strategy.GameMaterial;
using Strategy.TeamControl;
using Strategy.GameGUI;
using Mogre;


namespace Strategy.GameObjectControl.GroupMgr {
	class GroupManager {

		protected Dictionary<IMovableGameObject, GroupMovables> groupMovList;

		protected Dictionary<int, SolarSystem> solarSystemDict;
		protected int lastSolarSystem = 0;

		public bool isMovableGroupActive; // Active is movable group

		private GroupMovables selectedGroupM; //not implemented ...will be actual selected group - need rectangular select
		private GroupStatics selectedGroupS;

		private int activeSolarSystem = 0; // Now active solarSystem


		#region singlton and constructor
		private static GroupManager instance;
		/// <summary>
		/// Singleton constructor
		/// </summary>
		/// <returns>Instance of GroupManager</returns>
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
			solarSystemDict = new Dictionary<int, SolarSystem>();
			groupMovList = new Dictionary<IMovableGameObject, GroupMovables>();
		}
		#endregion


		/// <summary>
		/// Called on frame update
		/// </summary>
		/// <param name="delay">Delay between frames</param>
		public void update(float delay) {
			foreach (KeyValuePair<int, SolarSystem> solarSys in solarSystemDict) {
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

			foreach (SolarSystem solarSyst in solSystList) {
				solarSystemDict.Add(lastSolarSystem, solarSyst);
				lastSolarSystem++;
			}
		}

		/// <summary>
		/// Show given solar system and hide actual
		/// </summary>
		/// <param name="newSolarSystem">Integer of showing solar system</param>
		public void changeSolarSystem(int newSolarSystem) {

			solarSystemDict[activeSolarSystem].hideSolarSystem();
			solarSystemDict[newSolarSystem].showSolarSystem();

			activeSolarSystem = newSolarSystem; // Set new active solar system  
			deselectGroup();
			GUIControler.getInstance().setSolarSystemName(getSolarSystemName(activeSolarSystem)); //TODO to tu asi nechchi
		}

		public List<string> getAllSolarSystemNames() {
			var list = new List<string>();
			foreach (var ss in solarSystemDict) {
				list.Add(ss.Value.Name);
			}
			return list;
		}


		public string getSolarSystemName(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem].Name;
		}

		public SolarSystem getActiveSolarSystem() {
			return solarSystemDict[activeSolarSystem];
		}

		public SolarSystem getSolarSystem(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem];
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
				group.insertMemeber(isgoList[0]);	// Insert firt
				var inGroup = isgoList[0];
				if (isgoList.Count > 1) {		// Check if there is more object
					for (int i = 1; i < isgoList.Count; i++) {
						if (inGroup.Team.Name == Game.playerName && inGroup.Team == isgoList[i].Team) {
							group.insertMemeber(isgoList[i]); // Insert player's isgo	
						} else {
							if (isgoList[i].Team.Name == Game.playerName) { // In some of elements in isgoList is players's -> has greater priority
								group = new GroupStatics(isgoList[i].Team);
								group.insertMemeber(isgoList[i]);	// Insert firt
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

			if (imgoList.Count > 1) {		// Check if there is more object
				for (int i = 1; i < imgoList.Count; i++) {
					if (group.OwnerTeam.Name == Game.playerName && group.OwnerTeam == imgoList[i].Team) {
						group.insertMemeber(imgoList[i]); // Insert player's imgo	
					} else {
						if (imgoList[i].Team.Name == Game.playerName) { // In some of elements in isgoList is players's -> has greater priority
							group = new GroupMovables(imgoList[i].Team);
							group.insertMemeber(imgoList[i]);	// Insert firt
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
				if (!(groupMovList.ContainsKey(selectedGroupM[0]) && selectedGroupM == groupMovList[selectedGroupM[0]])) {
					var toRecount = new List<GroupMovables>();
					foreach (IMovableGameObject imgo in selectedGroupM) {
						if (groupMovList.ContainsKey(imgo)) {
							// Odeber ze stavajici a prepocti ji.
							groupMovList[imgo] = selectedGroupM;
						} else {
							groupMovList.Add(imgo, selectedGroupM);
						}
					}

					selectedGroupM.select();
				}
				
				return selectedGroupM.onMouseAction(ActionReason.onRightButtonClick, clickedPoint, hitObject, isFriendly, isImgo);
			} else {
				return ActionAnswer.None;
			}

		}


		public GroupMovables getActiveMovableGroup() {
			return selectedGroupM;
		}

	}


}
