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

		protected List<T> groupMembers;
		protected Property<Team> owner;

		public IGroup(Team own) {
			groupMembers = new List<T>();
			owner = new Property<Team>(own);
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

		public virtual Dictionary<string, object> getPropertyToDisplay() {
			return new Dictionary<string, object>();
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
			get { return owner.Value; }
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

			groupBonuses.Add("Attack", new Property<int>(1));
			groupBonuses.Add("Deffence", new Property<int>(1));
			groupBonuses.Add("Speed", new Property<int>(1));



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
			var countBonus = (int)(groupMembers.Count / 10);
			((Property<int>)groupBonuses["Attack"]).Value = 1 + countBonus;
			((Property<int>)groupBonuses["Deffence"]).Value = 1 + countBonus;
			foreach (IMovableGameObject imgo in groupMembers) {
				//somehow collect bonuses
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

		public override Dictionary<string, object> getPropertyToDisplay() {
			Dictionary<string,object> propDict;
			if (groupMembers.Count == 1) {
				propDict = new Dictionary<string, object>(groupMembers[0].getPropertyToDisplay());
			} else {
				propDict = new Dictionary<string, object>();
			}
			propDict.Add("Team", owner);
			//pokusDict.Add("HOasno", null); 

			return propDict;
		}
	}
}
