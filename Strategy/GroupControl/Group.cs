using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GroupControl{
	class GroupMovables {
		private int number;
		private List<IMovableGameObject> groupMembers;

		public GroupMovables() {
			groupMembers = new List<IMovableGameObject>();
		}

		public void setNumberOfGroup(int i) {
			number = i;
		}

		public int getNumberOfGroup() {
			return number;
		}

		public void insertMemeber(IMovableGameObject m) {
			groupMembers.Add(m);
		}

		public void removeMember(IMovableGameObject m) {
			groupMembers.Remove(m);
		}

		public void move(float f) {
			foreach (IMovableGameObject obj in groupMembers) {
				obj.move(f);
			}
		}
	}

	class GroupStatics {
		private int number;
		private List<IStaticGameObject> groupMembers;
		private Sun sun;

		public GroupStatics() {
			groupMembers = new List<IStaticGameObject>();
		}

		public void setSun(Sun s) {
			sun = s;
		}
		public void setNumberOfGroup(int i) {
			number = i;
		}

		public int getNumberOfGroup() {
			return number;
		}

		public void insertMemeber(IStaticGameObject m) {
			groupMembers.Add(m);
		}

		public void removeMember(IStaticGameObject m) {
			groupMembers.Remove(m);
		}

		public void rotate() {
			sun.rotate();
			foreach (IStaticGameObject obj in groupMembers) {
				//rotate...not implemented
			}
		}
	}
}
