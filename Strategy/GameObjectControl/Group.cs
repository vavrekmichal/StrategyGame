using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using System.Collections;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl {
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

		public virtual void insertMemeber(T m) {
			groupMembers.Add(m);
		}

		public virtual void removeMember(T m) {
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

		public static Dictionary<string, object> baseTemplateBonusDict;		//static template for creating new Group with setted basic bonuses

		/// <summary>
		/// Static constructor setted baseTemp.. dictionary with basic stats
		/// </summary>
		static GroupMovables() {
			baseTemplateBonusDict = new Dictionary<string, object>();
			baseTemplateBonusDict.Add("attack", new Property<int>(1));
			baseTemplateBonusDict.Add("deffence", new Property<int>(1));
			baseTemplateBonusDict.Add("speed", new Property<int>(1));
		}

		private Dictionary<string, object> groupBonuses;

		public GroupMovables() : this(new Team("")) { }
		public GroupMovables(TeamControl.Team own)
			: base(own) {
			groupBonuses = new Dictionary<string, object>();

			groupBonuses.Add("attack", new Property<int>(1));
			groupBonuses.Add("deffence", new Property<int>(1));
			groupBonuses.Add("speed", new Property<int>(1));



		}

		public void move(float f) {
			foreach (IMovableGameObject obj in groupMembers) {
				obj.move(f);
			}
		}

		public void nonVisibleMove(float f) {
		}

		public void select() {		//called when group is changed from informative to selected
			//Need colect bonuses from count and from members
			((Property<int>)groupBonuses["attack"]).Value += 1;
			foreach (IMovableGameObject imgo in groupMembers) {
				imgo.setGroupBonuses(groupBonuses);
			}
		}

		public ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			ActionAnswer groupAnswer = ActionAnswer.None;
			foreach (IMovableGameObject imgo in groupMembers) {
				ActionAnswer answer = imgo.onMouseAction(reason, point, hitObject, isFriendly, isMovableGameObject);//TODO Team Control
				if (answer > groupAnswer) {
					groupAnswer = answer;
				}
			}
			return groupAnswer;
		}


	}

	class GroupStatics : IGroup<IStaticGameObject> {
		public GroupStatics() : base(new Team("None")) { }
		public GroupStatics(TeamControl.Team own) : base(own) { }
	}
}
