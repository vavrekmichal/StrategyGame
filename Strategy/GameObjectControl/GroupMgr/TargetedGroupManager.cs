using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameObjectControl.GroupMgr {
	/// <summary>
	/// Contols targeted group. Creates indicators and shows the group to GUI.
	/// </summary>
	class TargetedGroupManager {

		GroupMovables groupMovables;
		GroupStatics groupStatics;

		bool targetedIsMovalbe = false;

		List<TargetPointer> pointerList = new List<TargetPointer>();

		/// <summary>
		/// Creates a TargetPointer for each member of the group and destroys old pointers.
		/// Also indicates that the static group is targeted.
		/// </summary>
		/// <param name="group">The targeted group.</param>
		public void TargetGroup(GroupStatics group) {
			targetedIsMovalbe = false;
			groupStatics = group;
			DestroyPointers();

			foreach (IGameObject item in group) {
				pointerList.Add(new TargetPointer(item));
			}
		}

		/// <summary>
		/// Creates a TargetPointer for each member of the group and destroys old pointers.
		/// Also indicates that the movable group is targeted.
		/// </summary>
		/// <param name="group">The targeted group.</param>
		public void TargetGroup(GroupMovables group) {
			targetedIsMovalbe = true;
			groupMovables = group;
			DestroyPointers();

			foreach (IGameObject item in group) {
				pointerList.Add(new TargetPointer(item));
			}
		}

		/// <summary>
		/// Clears the TargetedGroupManager. Unsets static and movable group
		/// and destroys TargetPointers.
		/// </summary>
		public void Clear() {
			DestroyPointers();
			groupMovables = null;
			groupStatics = null;
			targetedIsMovalbe = false;
		}

		/// <summary>
		/// Updates all TargetPointers.
		/// </summary>
		/// <param name="delay"></param>
		public void Update(float delay) {
			foreach (var pointer in pointerList) {
				pointer.Update(delay);
			}
		}

		/// <summary>
		/// Shows targeted group by GUI (static or movable by the setted indicator).
		/// </summary>
		public void ShowTargetedGroup() {
			if (targetedIsMovalbe) {
				Game.IGameGUI.ShowTargeted(groupMovables);
			} else {
				Game.IGameGUI.ShowTargeted(groupStatics);
			}
		}

		/// <summary>
		/// Returns the movable group if is really targeted.
		/// </summary>
		/// <returns>Returns the movable group.</returns>
		public GroupMovables GetActiveMovableGroup() {
			if (targetedIsMovalbe) {
				return groupMovables;
			} else {
				return null;
			}

		}

		/// <summary>
		/// Returns the static group if is really targeted.
		/// </summary>
		/// <returns>Returns the static group.</returns>
		public GroupStatics GetAtctiveStaticGroup() {
			if (targetedIsMovalbe) {
				return null;
			} else {
				return groupStatics;
			}
		}

		/// <summary>
		/// Returns if the targeted group is movable.
		/// </summary>
		public bool TargetedIsMovable {
			get { return targetedIsMovalbe; }
		}

		/// <summary>
		/// Returns the Team of the targeted group.
		/// </summary>
		public TeamControl.Team ActiveTeam {
			get {
				if (targetedIsMovalbe) {
					return groupMovables.Team;
				} else {
					return groupStatics.Team;
				}
			}
		}

		/// <summary>
		/// Destroys all TargetPointers.
		/// </summary>
		private void DestroyPointers() {
			foreach (var pointer in pointerList) {
				pointer.Destroy();
			}
			pointerList = new List<TargetPointer>();
		}

	}
}
