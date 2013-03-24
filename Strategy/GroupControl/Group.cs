using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using System.Collections;

namespace Strategy.GroupControl{
	abstract class IGroup<T>: IEnumerable{
		protected int number;
		protected List<T> groupMembers;

		public IGroup() {
			groupMembers = new List<T>();
		}

		public void setNumberOfGroup(int i) {
			number = i;
		}

		public int getNumberOfGroup() {
			return number;
		}

		public int Count {
			get { return groupMembers.Count;}
		}

		public void insertMemeber(T m) {
			groupMembers.Add(m);
		}

		public void removeMember(T m) {
			groupMembers.Remove(m);
		}

        public IEnumerator GetEnumerator() {
            return groupMembers.GetEnumerator();
        }

		public T this[int i] {
			get {
				return groupMembers[i];
			}
			set {
				groupMembers[i] = value;
			}
		}
	}



	class GroupMovables :IGroup<IMovableGameObject>{

		public void move(float f) {
			foreach (IMovableGameObject obj in groupMembers) {
				obj.move(f);
			}
		}

        public void nonVisibleMove(float f) {
        }


	}

	class GroupStatics : IGroup<IStaticGameObject> {
        
    }
}
