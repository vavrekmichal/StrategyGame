using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;
using System.Collections;
using Mogre;
using Strategy.TeamControl;

namespace Strategy.GroupControl {
	public abstract class IGroup<T> : IEnumerable {
		protected int number;
		protected List<T> groupMembers;
		protected Team owner;
		

		public IGroup(Team own) {
			groupMembers = new List<T>();
			owner = own;
		}

		public void setNumberOfGroup(int i) { //not Count
			number = i;
		}

		public int getNumberOfGroup() {
			return number;
		}

		public int Count {
			get { return groupMembers.Count; }
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

		public Team OwnerTeam {
			get { return owner; }
		}

	}



	public class GroupMovables : IGroup<IMovableGameObject> {

		public GroupMovables() : base(new Team("None")) { }
		public GroupMovables(TeamControl.Team own) : base(own) { }

		public void move(float f) {
			foreach (IMovableGameObject obj in groupMembers) {
				obj.move(f);
			}
		}

		public void nonVisibleMove(float f) {
		}

		public ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			ActionAnswer groupAnswer = ActionAnswer.None;
			foreach (IMovableGameObject imgo  in groupMembers) {
				ActionAnswer answer = imgo.onMouseAction(reason, point, hitObject, isFriendly, isMovableGameObject);//TODO Team Control
				if (answer>groupAnswer) {
					groupAnswer = answer;
				}
			}
			return groupAnswer;
		}


	}

	class GroupStatics : IGroup<IStaticGameObject> {
		public GroupStatics(): base(new Team("None")) { }
		public GroupStatics(TeamControl.Team own) : base(own) { }
	}
}
