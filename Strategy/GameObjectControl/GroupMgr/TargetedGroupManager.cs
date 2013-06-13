using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameObjectControl.GroupMgr {
	class TargetedGroupManager {

		GroupMovables groupMovables;
		GroupStatics groupStatics;

		bool targetedIsMovalbe = false;

		List<TargetPointer> pointerList = new List<TargetPointer>();

		public void TargetGroup(GroupStatics group) {
			targetedIsMovalbe = false;
			groupStatics = group;
			DestroyPointers();

			foreach (IGameObject item in group) {
				pointerList.Add(new TargetPointer(item));
			}
		}

		public void TargetGroup(GroupMovables group) {
			targetedIsMovalbe = true;
			groupMovables = group;
			DestroyPointers();

			foreach (IGameObject item in group) {
				pointerList.Add(new TargetPointer(item));
			}
		}

		public void Clear() {
			DestroyPointers();
			groupMovables = null;
			groupStatics = null;
			targetedIsMovalbe = false;
		}

		public void Update(float delay) {
			foreach (var pointer in pointerList) {
				pointer.Update(delay);
			}
		}

		public void ShowTargetedGroup() {
			if (targetedIsMovalbe) {
				Game.IGameGUI.ShowTargeted(groupMovables);
			} else {
				Game.IGameGUI.ShowTargeted(groupStatics);
			}
		}

		public GroupMovables GetActiveMovableGroup() {
			if (targetedIsMovalbe) {
				return groupMovables;
			} else {
				return null;
			}

		}

		public GroupStatics GetAtctiveStaticGroup() {
			if (targetedIsMovalbe) {
				return null;
			} else {
				return groupStatics;
			}
		}

		public bool TargetedIsMovable {
			get { return targetedIsMovalbe; }
		}


		public TeamControl.Team ActiveTeam {
			get {
				if (targetedIsMovalbe) {
					return groupMovables.Team;
				} else {
					return groupStatics.Team;
				}
			}
		}

		private void DestroyPointers() {
			foreach (var pointer in pointerList) {
				pointer.Destroy();
			}
			pointerList = new List<TargetPointer>();
		}

	}
}
