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

		protected Dictionary<IMovableGameObject, GroupMovables> imgoGroupDict;

		protected Dictionary<int, SolarSystem> solarSystemDict;
		protected int lastSolarSystem = 0;

		public bool isMovableGroupActive; // Active is movable group

		private GroupMovables selectedGroupM; //not implemented ...will be actual selected group - need rectangular select
		private GroupStatics selectedGroupS;

		private int activeSolarSystem = 0; // Now active solarSystem

		/// <summary>
		/// Public constructor
		/// </summary>
		public GroupManager() {
			solarSystemDict = new Dictionary<int, SolarSystem>();
			imgoGroupDict = new Dictionary<IMovableGameObject, GroupMovables>();
		}


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
				group.insertMemeber(isgoList[0]);	// Insert first
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
			bool inSameGroup = true;
			isMovableGroupActive = true;

			// Test if all imgo are in same selected group
			var firstGroup = getGroup(imgoList.First());

			for (int i = 1; i < imgoList.Count; i++) {
				var memberGroup = getGroup(imgoList[i]);
				if (firstGroup != memberGroup) {
					// Different groups => new group must be created (unselected)
					inSameGroup = false;
					break;
				}
			}

			if (!inSameGroup) {

				// Create new uselect group (selected object was not in same group)
				var group = new GroupMovables(imgoList.First().Team);
				group.insertMemeber(imgoList[0]);

				if (imgoList.Count > 1) {		// Check if there is more object
					for (int i = 1; i < imgoList.Count; i++) {
						if (group.OwnerTeam.Name == Game.playerName && group.OwnerTeam == imgoList[i].Team) {
							group.insertMemeber(imgoList[i]); // Insert player's imgo	
						} else {
							// Element of isgoList is in players's Team => has greater priority and must be selected
							if (imgoList[i].Team.Name == Game.playerName) {
								group = new GroupMovables(imgoList[i].Team);
								group.insertMemeber(imgoList[i]);	// Insert firt
							}
						}
					}
				}
				selectedGroupM = group;
			} else {

				// Objects are in same group. Now must be checked if group is complete.
				if (firstGroup.Count == imgoList.Count) {
					// Absolutly sam group just selected the group
					selectedGroupM = firstGroup;
				} else {
					// Subset of group => copy parameters (group is not selected, parameters will only be showed)
					var group = new GroupMovables(imgoList.First().Team);
					// Copy bonuses, bonuses will have actual value (changes in the original will be also in this info group)
					group.GroupBonusDict = firstGroup.GroupBonusDict;
					
					foreach (var imgo in imgoList) {
						group.insertMemeber(imgo);
					}
					selectedGroupM = group;
				}

			}
		}

		public GroupMovables getGroup(IMovableGameObject imgo) {
			GroupMovables group;
			if (imgoGroupDict.ContainsKey(imgo)) {
				group = imgoGroupDict[imgo];
			} else {
				group = new GroupMovables(imgo.Team);
				imgoGroupDict.Add(imgo, group);
			}
			return group;
		}


		public void showSelectedInfoGroup() {
			if (isMovableGroupActive) {
				Game.GUIManager.showTargeted(selectedGroupM);
			} else {
				Game.GUIManager.showTargeted(selectedGroupS);
			}
		}

		public Team ActiveTeam {
			get {
				if (isMovableGroupActive) {
					return selectedGroupM.OwnerTeam;
				} else {
					return selectedGroupS.OwnerTeam;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="clickedPoint">Mouse position</param>
		/// <param name="hitObject">HitTest result</param>
		/// <param name="isFriendly">If HitTest returns object => TeamTest result</param>
		/// <param name="isImgo">If HitTest returns object => MovableTest result</param>
		/// <returns>Returns group answer collected from each member of group</returns>
		public ActionAnswer onRightMouseClick(Mogre.Vector3 clickedPoint, MovableObject hitObject, bool isFriendly, bool isImgo) {
			//TODO comment
			if (isMovableGroupActive && selectedGroupM.OwnerTeam.Name == Game.playerName) {

				// Check if actual group is selectedGroupM
				if (!(imgoGroupDict.ContainsKey(selectedGroupM[0]) && selectedGroupM == imgoGroupDict[selectedGroupM[0]])) {

					// Group is unselect
					var toRecount = new List<GroupMovables>();
					foreach (IMovableGameObject imgo in selectedGroupM) {
						if (imgoGroupDict.ContainsKey(imgo)) {

							// Add object to toRecount 
							if (!toRecount.Contains(imgoGroupDict[imgo])) {
								toRecount.Add(imgoGroupDict[imgo]);
							}

							// Remove from old group and set new oneto Dict
							imgoGroupDict[imgo].removeMember(imgo);
							imgoGroupDict[imgo] = selectedGroupM;
						} else {
							imgoGroupDict.Add(imgo, selectedGroupM);
						}
					}

					// Recount all modified groups
					foreach (var group in toRecount) {
						// Count just basic bonuses, others are removed when the source of them is removed.
						group.countBasicBonuses();
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
