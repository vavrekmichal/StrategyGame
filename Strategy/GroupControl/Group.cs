using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using System.Collections;

namespace Strategy.GroupControl{
	class GroupMovables :IEnumerable{
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

        public void nonVisibleMove(float f) {
        }

        public IEnumerator GetEnumerator() {
            return groupMembers.GetEnumerator();
        }
	}

    class GroupStatics :IEnumerable{
        private int number;
        private List<IStaticGameObject> groupMembers;

        public GroupStatics() {
            groupMembers = new List<IStaticGameObject>();
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

        public void rotate(float f, int activeSolarSystem) {
            foreach (IStaticGameObject isgo in groupMembers) {
                if (isgo.getSolarSystem == activeSolarSystem) {
                    isgo.rotate(f);
                } else {
                    isgo.nonActiveRotate(f);
                }
                
            }
        }


        public IEnumerator GetEnumerator() {
            return groupMembers.GetEnumerator();
        }
    }
}
