using System.Collections.Generic;
using System.Linq;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
using Mogre;
using Strategy.GameObjectControl.Game_Objects;


namespace Strategy.GameObjectControl.GroupMgr {
	class GroupManager {

		protected Dictionary<IMovableGameObject, GroupMovables> imgoGroupDict;

		protected Dictionary<int, SolarSystem> solarSystemDict;
		protected int lastSolarSystem = 0;

		public bool isMovableGroupActive; // Active is movable group

		private GroupMovables selectedGroupM; //not implemented ...will be actual selected group - need rectangular Select
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
		/// Called on frame Update
		/// </summary>
		/// <param Name="delay">Delay between frames</param>
		public void Update(float delay) {
			foreach (KeyValuePair<int, SolarSystem> solarSys in solarSystemDict) {
				solarSys.Value.Update(delay);
			}
			Gate.UpdateTravelers(delay);
		}

		/// <summary>
		/// Function removes gameObject from group and calls Destroy to remove object from game
		/// </summary>
		/// <param Name="isgo">IStaticGameObject to remove from the game</param>
		public void DestroyGameObject(IStaticGameObject isgo) {
			RemoveFromGroup(isgo);
			isgo.Destroy();
			foreach (var solSysPair in solarSystemDict) {
				if (solSysPair.Value.HasISGO(isgo)) {
					solSysPair.Value.RemoveISGO(isgo);
					break;
				}
			}
		}
		/// <summary>
		/// Function removes gameObject from group and calls Destroy to remove object from game
		/// </summary>
		/// <param Name="gameObject">IMovableGameObject to remove from the game</param>
		public void DestroyGameObject(IMovableGameObject imgo) {
			RemoveFromGroup(imgo);
			imgo.Destroy();
			foreach (var solSysPair in solarSystemDict) {
				if (solSysPair.Value.HasIMGO(imgo)) {
					solSysPair.Value.RemoveIMGO(imgo);
					break;
				}
			}
		}


		/// <summary>
		/// Function attempts to remove IStaticGameObject from selectedGroup
		/// </summary>
		/// <param Name="isgo">IStaticGameObject to remove from the group</param>
		public void RemoveFromGroup(IStaticGameObject isgo) {
			if ((!isMovableGroupActive) && selectedGroupS.HasMember(isgo)) {
				selectedGroupS.RemoveMember(isgo);
				if (selectedGroupS.Count == 0) {
					selectedGroupS = new GroupStatics();
				}
			}
		}

		/// <summary>
		/// Function attempts to remove IMovableGameObject from its group
		/// </summary>
		/// <param Name="imgo">IMovableGameObject to remove from the group</param>
		public void RemoveFromGroup(IMovableGameObject imgo) {
			if (imgoGroupDict.ContainsKey(imgo)) {
				imgoGroupDict[imgo].RemoveMember(imgo);
				imgoGroupDict.Remove(imgo);
			}
		}
		public void CreateTraveler(int solarSystemNumberTo, object imgo) {
			Gate.CreateTraveler(GetActiveSolarSystem(), GetSolarSystem(solarSystemNumberTo), imgo);
		}


		public List<Traveler> GetTravelers() {
			return Gate.GetTravelers();
		}

		#region solarSyst

		public int GetSolarSystemsNumber(IGameObject igo) {
			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				for (int i = 0; i < solarSystemDict.Count; i++) {
					if (solarSystemDict[i].HasIMGO(imgo)) {
						return i;
					}
				}
			} else {
				var isgo = igo as IStaticGameObject;
				for (int i = 0; i < solarSystemDict.Count; i++) {
					if (solarSystemDict[i].HasISGO(isgo)) {
						return i;
					}
				}
			}

			return -1;
		}

		/// <summary>
		/// Get SolarSystem from ObjectCreator as List and creates Dictionary. 
		/// Also initializes HitTest
		/// </summary>
		public void CreateSolarSystems(List<SolarSystem> solSystList) {

			foreach (SolarSystem solarSyst in solSystList) {
				solarSystemDict.Add(lastSolarSystem, solarSyst);
				lastSolarSystem++;
			}
		}

		/// <summary>
		/// Show given solar system and hide actual
		/// </summary>
		/// <param Name="newSolarSystem">Integer of showing solar system</param>
		public void ChangeSolarSystem(int newSolarSystem) {

			solarSystemDict[activeSolarSystem].HideSolarSystem();
			solarSystemDict[newSolarSystem].ShowSolarSystem();

			activeSolarSystem = newSolarSystem; // Set new active solar system  
			DeselectGroup();

		}

		public List<string> GetAllSolarSystemNames() {
			var list = new List<string>();
			foreach (var ss in solarSystemDict) {
				list.Add(ss.Value.Name);
			}
			return list;
		}


		public string GetSolarSystemName(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem].Name;
		}

		public SolarSystem GetActiveSolarSystem() {
			return solarSystemDict[activeSolarSystem];
		}

		public SolarSystem GetSolarSystem(int numberOfSolarSystem) {
			return solarSystemDict[numberOfSolarSystem];
		}

		#endregion

		/// <summary>
		/// Set all Select group as new empty
		/// </summary>
		public void DeselectGroup() {
			GroupMovables groupM = new GroupMovables();
			GroupStatics groupS = new GroupStatics();
			isMovableGroupActive = false;
			ShowSelectedInfoGroup();
		}

		/// <summary>
		/// Creates group (without calling group.Select()) from given List with IStaticGameObject
		/// Object from player team has greater priority then others
		/// </summary>
		/// <param Name="isgoList">List with IStaticGameObject</param>
		public void CreateInfoGroup(List<IStaticGameObject> isgoList) {
			isMovableGroupActive = false;
			if (isgoList.Count > 0) {
				var group = new GroupStatics(isgoList[0].Team);
				group.InsertMemeber(isgoList[0]);	// Insert first
				var inGroup = isgoList[0];
				if (isgoList.Count > 1) {		// Check if there is more object
					for (int i = 1; i < isgoList.Count; i++) {
						if (inGroup.Team.Name == Game.playerName && inGroup.Team == isgoList[i].Team) {
							group.InsertMemeber(isgoList[i]); // Insert player's isgo	
						} else {
							if (isgoList[i].Team.Name == Game.playerName) { // In some of elements in isgoList is players's -> has greater priority
								group = new GroupStatics(isgoList[i].Team);
								group.InsertMemeber(isgoList[i]);	// Insert firt
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
		/// Creates group (without calling group.Select()) from given List with IMovableGameObject
		/// Object from player team has greater priority then others
		/// </summary>
		/// <param Name="isgoList">List with IMovableGameObject</param>
		public void CreateInfoGroup(List<IMovableGameObject> imgoList) {
			bool inSameGroup = true;
			isMovableGroupActive = true;

			imgoList = checkPlayersObjects(imgoList);

			// Test if all imgo are in same selected group
			var firstGroup = GetGroup(imgoList.First());

			for (int i = 1; i < imgoList.Count; i++) {
				var memberGroup = GetGroup(imgoList[i]);
				if (firstGroup != memberGroup) {
					// Different groups => new group must be created (unselected)
					inSameGroup = false;
					break;
				}
			}

			if (!inSameGroup) {

				// Create new uselect group (selected object was not in same group)
				var group = new GroupMovables(imgoList.First().Team);
				group.InsertMemeber(imgoList[0]);

				if (imgoList.Count > 1) {		// Check if there is more object
					for (int i = 1; i < imgoList.Count; i++) {
						if (group.OwnerTeam.Name == Game.playerName && group.OwnerTeam == imgoList[i].Team) {
							group.InsertMemeber(imgoList[i]); // Insert player's imgo	
						} else {
							// Element of isgoList is in players's Team => has greater priority and must be selected
							if (imgoList[i].Team.Name == Game.playerName) {
								group = new GroupMovables(imgoList[i].Team);
								group.InsertMemeber(imgoList[i]);	// Insert firt
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
						group.InsertMemeber(imgo);
					}
					selectedGroupM = group;
				}

			}
		}

		/// <summary>
		/// Function checks every IMovableGameObject in imgoList if is from player team.
		/// When function found any player's object so it returns just player's objects, 
		/// else it return whole imgoList.
		/// </summary>
		/// <param name="imgoList">List with checking IMovableObjects</param>
		/// <returns>Returns checked list with whole imgoList or just with player's objects</returns>
		private List<IMovableGameObject> checkPlayersObjects(List<IMovableGameObject> imgoList) {

			var resultList = new List<IMovableGameObject>();

			foreach (var imgo in imgoList) {
				if (imgo.Team.Name == Game.playerName) {
					resultList.Add(imgo);
				}
			}

			if (resultList.Count == 0) {
				resultList = imgoList;
			}
			return resultList;
		}


		/// <summary>
		/// Function remove objects from their group and creates new one.
		/// List can contains some IStaticGameObjects so they must be removed.
		/// </summary>
		/// <param name="igos">List with IStaticGameObjects and IMovableGameObjects</param>
		/// <returns>Created group with IMovableGameObjects from igos List</returns>
		public GroupMovables CreateSelectedGroupMovable(List<IGameObject> igos) {
			var toRecount = new List<GroupMovables>();
			var group = new GroupMovables(igos[0].Team);
			foreach (var igo in igos) {
				var imgo = igo as IMovableGameObject;
				if (imgo != null) {
					group.InsertMemeber(imgo);
					if (imgoGroupDict.ContainsKey(imgo)) {
						// Add object to toRecount 
						if (!toRecount.Contains(imgoGroupDict[imgo])) {
							toRecount.Add(imgoGroupDict[imgo]);
						}
						// Remove from old group and set new one to Dict
						imgoGroupDict[imgo].RemoveMember(imgo);
						imgoGroupDict[imgo] = group;
					} else {
						imgoGroupDict.Add(imgo, group);
					}
				}
			}

			// Recount all modified groups
			foreach (var groupRec in toRecount) {
				// Recount just basic bonuses, others are removed when the source of them is removed.
				groupRec.CountBasicBonuses();
			}
			group.Select();
			return group;
		}

		public GroupStatics CreateSelectedGroupStatic(List<IGameObject> igos) {
			var group = new GroupStatics(igos[0].Team);
			foreach (var igo in igos) {
				var imgo = igo as IStaticGameObject;
				if (imgo != null) {
					group.InsertMemeber(imgo);
				}
			}
			return group;
		}


		public GroupMovables GetGroup(IMovableGameObject imgo) {
			GroupMovables group;
			if (imgoGroupDict.ContainsKey(imgo)) {
				group = imgoGroupDict[imgo];
			} else {
				group = new GroupMovables(imgo.Team);
				imgoGroupDict.Add(imgo, group);
			}
			return group;
		}


		public void ShowSelectedInfoGroup() {
			if (isMovableGroupActive) {
				Game.GUIManager.ShowTargeted(selectedGroupM);
			} else {
				Game.GUIManager.ShowTargeted(selectedGroupS);
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
		/// Function checks if player can controls selected group. If the group is new so the function remove
		/// all object from their actual groups and recount them. Finally call Select (count bonuses, etc.)
		/// </summary>
		/// <param Name="clickedPoint">Mouse position</param>
		/// <param Name="hitObject">HitTest result</param>
		/// <param Name="isFriendly">If HitTest returns object => TeamTest result</param>
		/// <param Name="isImgo">If HitTest returns object => MovableTest result</param>
		/// <returns>Returns group answer collected from each member of group</returns>
		public ActionAnswer OnRightMouseClick(Mogre.Vector3 clickedPoint, MovableObject hitObject, bool isFriendly, bool isImgo) {
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
							imgoGroupDict[imgo].RemoveMember(imgo);
							imgoGroupDict[imgo] = selectedGroupM;
						} else {
							imgoGroupDict.Add(imgo, selectedGroupM);
						}
					}

					// Recount all modified groups
					foreach (var group in toRecount) {
						// Recount just basic bonuses, others are removed when the source of them is removed.
						group.CountBasicBonuses();
					}
					selectedGroupM.Select();
				}
				return selectedGroupM.OnMouseAction(clickedPoint, hitObject, isFriendly, isImgo);

			} else {
				return ActionAnswer.None;
			}

		}


		public GroupMovables GetActiveMovableGroup() {
			return selectedGroupM;
		}

	}


}
