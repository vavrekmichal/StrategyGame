using System.Collections.Generic;
using System.Linq;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.TeamControl;
using Mogre;
using Strategy.GameObjectControl.Game_Objects;


namespace Strategy.GameObjectControl.GroupMgr {
	/// <summary>
	/// Controls objects and collects them to groups. Also selecting groups and adds/removes members.
	/// </summary>
	public class GroupManager {

		protected Dictionary<IMovableGameObject, GroupMovables> imgoGroupDict;

		private TargetedGroupManager targetedMgr;

		/// <summary>
		/// Creates TergetedGroupManager.
		/// </summary>
		public GroupManager() {
			targetedMgr = new TargetedGroupManager();
			imgoGroupDict = new Dictionary<IMovableGameObject, GroupMovables>();
		}


		/// <summary>
		/// Updates TargetedGroupManager.
		/// </summary>
		/// <param Name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			targetedMgr.Update(delay);
		}

		/// <summary>
		/// Removes static game object from group, from solar system and calls Destroy to remove object from the game.
		/// </summary>
		/// <param Name="isgo">IStaticGameObject to remove from the game.</param>
		public void DestroyGameObject(IStaticGameObject isgo) {
			RemoveFromGroup(isgo);
			isgo.Destroy();
			Game.SolarSystemManager.RemoveObjectFromSolarSystem(isgo);
		}
		/// <summary>
		/// Removes movable game object from group, from solar system and calls Destroy to remove object from the game.
		/// </summary>
		/// <param Name="gameObject">IMovableGameObject to remove from the game.</param>
		public void DestroyGameObject(IMovableGameObject imgo) {
			RemoveFromGroup(imgo);
			imgo.Destroy();
			Game.SolarSystemManager.RemoveObjectFromSolarSystem(imgo);
		}


		/// <summary>
		/// Attempts to remove IStaticGameObject from the targeted group.
		/// </summary>
		/// <param Name="isgo">IStaticGameObject to remove from the targeted group.</param>
		public void RemoveFromGroup(IStaticGameObject isgo) {
			if (!targetedMgr.TargetedIsMovable) {
				GroupStatics group = targetedMgr.GetAtctiveStaticGroup();

				if (group.HasMember(isgo)) {
					group.RemoveMember(isgo);
					if (group.Count == 0) {
						group = new GroupStatics();
					}
				}
			}
		}

		/// <summary>
		/// Attempts to remove IMovableGameObject from its group.
		/// </summary>
		/// <param Name="imgo">IMovableGameObject to remove from the group</param>
		public void RemoveFromGroup(IMovableGameObject imgo) {
			if (imgoGroupDict.ContainsKey(imgo)) {
				imgoGroupDict[imgo].RemoveMember(imgo);
				imgoGroupDict.Remove(imgo);
			}
		}		

		/// <summary>
		/// Sets all select group as new empty groups and shows empty (by targetedMgr).
		/// </summary>
		public void DeselectGroup() {
			targetedMgr.Clear();
			targetedMgr.ShowTargetedGroup();
		}

		/// <summary>
		/// Creates group (without calling Select) from given List with IStaticGameObjects.
		/// Objects from player team has greater priority then others, so if list contains 
		/// any so the others will not be selected.
		/// </summary>
		/// <param Name="isgoList">The List with IStaticGameObjects</param>
		public void CreateInfoGroup(List<IStaticGameObject> isgoList) {
			if (isgoList.Count > 0) {
				var group = new GroupStatics(isgoList[0].Team);
				group.InsertMemeber(isgoList[0]);	// Insert first
				var inGroup = isgoList[0];
				if (isgoList.Count > 1) {		// Check if there is more object
					for (int i = 1; i < isgoList.Count; i++) {
						if (inGroup.Team == isgoList[i].Team) {
							group.InsertMemeber(isgoList[i]); // Insert player isgo	
						} else {
							if (isgoList[i].Team.Name == Game.PlayerName) { // In some of elements in isgoList is players -> has greater priority
								group = new GroupStatics(isgoList[i].Team);
								group.InsertMemeber(isgoList[i]);	// Insert firt
								inGroup = isgoList[i];
							}
						}
					}
				}
				targetedMgr.TargetGroup(group);
			} else {
				targetedMgr.TargetGroup(new GroupStatics());
			}
			targetedMgr.ShowTargetedGroup();
		}

		/// <summary>
		/// Creates group (without calling Select) from given List with IMovableGameObjects.
		/// Objects from player team has greater priority then others, so if list contains 
		/// any so the others will not be selected.
		/// </summary>
		/// <param Name="isgoList">The List with IMovableGameObjects</param>
		public void CreateInfoGroup(List<IMovableGameObject> imgoList) {
			bool inSameGroup = true;

			imgoList = checkPlayersObjects(imgoList);

			// Tests if all imgo are in same selected group
			var firstGroup = GetGroup(imgoList.First());

			for (int i = 1; i < imgoList.Count; i++) {
				var memberGroup = GetGroup(imgoList[i]);
				if (firstGroup != memberGroup) {
					// Different groups -> new group must be created (unselected)
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
						if (imgoList[i].Team.Name == Game.PlayerName && group.Team.Name != Game.PlayerName) {
							group = new GroupMovables(imgoList[i].Team);
							group.InsertMemeber(imgoList[i]);	// Insert first
						} else {
							if ( group.Team == imgoList[i].Team) {
								group.InsertMemeber(imgoList[i]);
							}
						}
					}
				}
				targetedMgr.TargetGroup(group);
			} else {

				// Objects are in same group. Now must be checked if group is complete.
				if (firstGroup.Count == imgoList.Count) {
					// Absolutly same group just selected the group
					targetedMgr.TargetGroup(firstGroup);
				} else {
					// Subset of group => copy parameters (group is not selected, parameters will only be showed)
					var group = new GroupMovables(imgoList.First().Team);
					// Copy bonuses, bonuses will have actual value (changes in the original will be also in this info group)
					group.GroupBonusDict = firstGroup.GroupBonusDict;

					foreach (var imgo in imgoList) {
						group.InsertMemeber(imgo);
					}
					targetedMgr.TargetGroup(group);
				}
			}
			targetedMgr.ShowTargetedGroup();
		}

		/// <summary>
		/// Checks every IMovableGameObject in imgoList if is from player's team.
		/// When is founded any player object so it returns just player's objects, 
		/// else it return whole imgoList.
		/// </summary>
		/// <param name="imgoList">The List with checking IMovableObjects.</param>
		/// <returns>Returns checked list with whole imgoList or just with player objects.</returns>
		private List<IMovableGameObject> checkPlayersObjects(List<IMovableGameObject> imgoList) {
			var resultList = new List<IMovableGameObject>();

			foreach (var imgo in imgoList) {
				if (imgo.Team.Name == Game.PlayerName) {
					resultList.Add(imgo);
				}
			}

			if (resultList.Count == 0) {
				resultList = imgoList;
			}
			return resultList;
		}


		/// <summary>
		/// Removes objects from their group and creates new one.
		/// The List can contains some IStaticGameObjects so they must be removed.
		/// New created group is selected (Select - bonuses are counted and are setted).
		/// </summary>
		/// <param name="igoList">The List with IStaticGameObjects and IMovableGameObjects</param>
		/// <returns>Returns created group with IMovableGameObjects from the igoList</returns>
		public GroupMovables CreateSelectedGroupMovable(List<IGameObject> igoList) {
			var toRecount = new List<GroupMovables>();
			var group = new GroupMovables(igoList[0].Team);
			foreach (var igo in igoList) {
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

		/// <summary>
		/// Removes objects from their group and creates new one.
		/// List can contains some IMovableGameObjects so they must be removed.
		/// </summary>
		/// <param name="igoList">The List with IStaticGameObjects and IMovableGameObjects</param>
		/// <returns>Returns created group with IStaticGameObjects from the igoList</returns>
		public GroupStatics CreateSelectedGroupStatic(List<IGameObject> igoList) {
			var group = new GroupStatics(igoList[0].Team);
			foreach (var igo in igoList) {
				var imgo = igo as IStaticGameObject;
				if (imgo != null) {
					group.InsertMemeber(imgo);
				}
			}
			return group;
		}

		/// <summary>
		/// Finds required group by given object. If it is not exists so the new is created
		/// just with given object.
		/// </summary>
		/// <param name="imgo">The object which the group is looking for.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Returns active team (Team of targeted group).
		/// </summary>
		public Team ActiveTeam {
			get {
				return targetedMgr.ActiveTeam;
			}
		}


		/// <summary>
		/// Checks if player can controls selected group. If the group is new so the function remove
		/// all object from their actual groups and recount them. Finally call Select (count bonuses, etc.)
		/// </summary>
		/// <param Name="clickedPoint">The mouse position.</param>
		/// <param name="hitObject">The result of a HitTest.</param>
		/// <param name="isFriendly">The information if the hitted object is friendly.</param>
		/// <param name="isMovableGameObject">The information if the hitted object is movable.</param>
		/// <returns>Returns group answer collected from each member of group</returns>
		public ActionAnswer SelectInfoGroup(Mogre.Vector3 clickedPoint, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			if (targetedMgr.TargetedIsMovable) {
				GroupMovables group = targetedMgr.GetActiveMovableGroup();

				if (group.Team.Name == Game.PlayerName) {

					// All members of group can die so movable group is deactive.
					if (group.Count == 0) {
						targetedMgr.Clear();
						return ActionAnswer.None;
					}

					// Check if actual group is selectedGroupM
					if (!(imgoGroupDict.ContainsKey(group[0]) && group == imgoGroupDict[group[0]])) {

						// Group is unselect
						var toRecount = new List<GroupMovables>();
						foreach (IMovableGameObject imgo in group) {
							if (imgoGroupDict.ContainsKey(imgo)) {

								// Add object to toRecount 
								if (!toRecount.Contains(imgoGroupDict[imgo])) {
									toRecount.Add(imgoGroupDict[imgo]);
								}

								// Remove from old group and set new oneto Dict
								imgoGroupDict[imgo].RemoveMember(imgo);
								imgoGroupDict[imgo] = group;
							} else {
								imgoGroupDict.Add(imgo, group);
							}
						}

						// Recount all modified groups
						foreach (var groupRec in toRecount) {
							// Recount just basic bonuses, others are removed when the source of them is removed.
							groupRec.CountBasicBonuses();
						}
						group.Select();
					}
					return group.OnMouseAction(clickedPoint, hitObject, isFriendly, isMovableGameObject);
				}
			}
			return ActionAnswer.None;
		}

		/// <summary>
		/// Returns current targeted movable group from TargetedGroupManager.
		/// </summary>
		/// <returns>Returns current targeted movable group.</returns>
		public GroupMovables GetActiveMovableGroup() {
			return targetedMgr.GetActiveMovableGroup();
		}

		/// <summary>
		/// Selects given group (counts bonuses, sets bonuses, etc.)/
		/// </summary>
		/// <param name="group">The selecting group.</param>
		public void SelectGroup(GroupMovables group) {
			targetedMgr.TargetGroup(group);
			targetedMgr.ShowTargetedGroup();
		}

		/// <summary>
		/// Inserts given IMovableGameObject to the group. Also recounts old group.
		/// </summary>
		/// <param name="group">The member which will be inserted to this group.</param>
		/// <param name="gameObject">The object which will be inesrted.</param>
		public void AddToGroup(GroupMovables group, IGameObject gameObject) {
			var imgo = gameObject as IMovableGameObject;
			if (imgo != null) {
				if (imgoGroupDict.ContainsKey(imgo)) {
					var removeFromGroup = imgoGroupDict[imgo];
					removeFromGroup.RemoveMember(imgo);
					removeFromGroup.CountBasicBonuses();
					imgoGroupDict[imgo] = group;
				} else {
					imgoGroupDict.Add(imgo, group);
				}
				group.InsertMemeber(imgo);
			}
		}
	}
}
