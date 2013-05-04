using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using System.Collections;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;
using System.Reflection;

namespace Strategy.GameObjectControl.GroupMgr {
	public abstract class IGroup<T> : IEnumerable where T : class {

		protected List<T> groupMembers;
		protected Property<Team> owner;

		public IGroup(Team own) {
			groupMembers = new List<T>();
			owner = new Property<Team>(own);
		}

		public int Count {
			get { return groupMembers.Count; }
		}

		public virtual bool hasMember(T m) {
			foreach (var member in groupMembers) {
				if (member == m) {
					return true;
				}
			}
			return false;
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

		protected void addObjectPropertyToDict(Dictionary<string, object> isgoPropDict, Dictionary<object, EditablePair<string, int>> summaryDict) {
			foreach (KeyValuePair<string, object> property in isgoPropDict) {		//object is real Property<T>	
				if (summaryDict.ContainsKey(property.Value)) {
					EditablePair<string, int> pair = summaryDict[property.Value];
					summaryDict[property.Value].Item2++;
				} else {
					summaryDict.Add(property.Value, new EditablePair<string, int>(property.Key, 1));
				}
			}
		}

		protected Dictionary<string, object> createCommonPropDict(Dictionary<object, EditablePair<string, int>> summaryDict, Dictionary<string, object> isgoPropDict) {
			int groupCount = groupMembers.Count;
			var result = new Dictionary<string, object>();
			var notEveryone = new List<string>();
			foreach (KeyValuePair<object, EditablePair<string, int>> propPair in summaryDict) {
				//PropertyEnum name = isgoPropDict.FirstOrDefault(x => x.Value == propPair.Key).Key; //gets string name searched by object
				string name = propPair.Value.Item1;
				if (groupCount == propPair.Value.Item2) {
					result.Add(name, propPair.Key);
				} else {
					if (!notEveryone.Contains(name)) {
						result.Add(name, new Property<char>('?'));
						notEveryone.Add(name);
					}
				}
			}

			return result;
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

	#region GroupMovables

	public class GroupMovables : IGroup<IMovableGameObject> {

		private Dictionary<string, object> groupBonuses;

		private bool isSelected;

		public GroupMovables() : this(new Team("")) { }
		public GroupMovables(TeamControl.Team own)
			: base(own) {
			countBasicBonuses();
		}

		private object callPropertyMathViaReflection(Property<object>.Operator op, object p1, object p2) {
			var type = p1.GetType().GetGenericArguments()[0];

			// Using reflection to call function simpleMath with generic parameter
			MethodInfo method = p1.GetType().GetMethod("simpleMath");
			List<object> args = new List<object>();
			args.Add(op);
			args.Add(p2);
			return method.Invoke(p1, args.ToArray());

		}

		/// <summary>
		/// Called when group is changed from informative to selected. Function count basic bonuses for selected
		/// group (Attack and Deffence bonus +1 per each 3 members) and collect bonuses from members (onGroupAdd).
		/// Finally set bonus dictionary for each member of group.
		/// </summary>
		public void select() {
			if (isSelected) {
				return;
			}
			isSelected = true;

			// Basic bonuses to the Attack and the Deffence ( 1 per each 3 members)
			countBasicBonuses();

			//var v = callPropertyMathViaReflection(Property<object>.Operator.Plus, new Property<int>(6), new Property<int>(12));

			// Collect bonuses
			foreach (IMovableGameObject imgo in groupMembers) {
				var imgoBonus = imgo.onGroupAdd();
				foreach (var bonusPair in imgoBonus) {
					if (groupBonuses.ContainsKey(bonusPair.Key)) {

						// Find type and add value. If Properties has not same type then isn't added
						var type = bonusPair.Value.GetType();
						if (type.GetGenericTypeDefinition() == typeof(Property<>) &&
							type == groupBonuses[bonusPair.Key].GetType()) {

							groupBonuses[bonusPair.Key] = callPropertyMathViaReflection(Property<object>.Operator.Plus, groupBonuses[bonusPair.Key], bonusPair.Value);
						}
					} else {
						groupBonuses.Add(bonusPair.Key, bonusPair.Value);
					}
				}
				imgo.setGroupBonuses(groupBonuses);
			}
		}

		/// <summary>
		/// Function colects all answers and returns with the highest priority 
		/// </summary>
		/// <param name="reason">Reason of calling function</param>
		/// <param name="point">Position of mouse when was clicked</param>
		/// <param name="hitObject">Result of hit test</param>
		/// <param name="isFriendly">When was clicked on object - team test</param>
		/// <param name="isMovableGameObject">When was clicked on object - movable test</param>
		/// <returns>Answers with the highest priority</returns>
		public ActionAnswer onMouseAction(Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			ActionAnswer groupAnswer = ActionAnswer.None;
			foreach (IMovableGameObject imgo in groupMembers) {
				ActionAnswer answer = imgo.onMouseAction(point, hitObject, isFriendly, isMovableGameObject);
				if (answer > groupAnswer) {
					groupAnswer = answer;
				}
			}
			return groupAnswer;
		}


		public override Dictionary<string, object> getPropertyToDisplay() {
			var propDict = new Dictionary<string, object>();
			propDict.Add(PropertyEnum.Team.ToString(), owner);
			var bonusesCopy = new Dictionary<string, object>(groupBonuses);

			const string bonus = "Bonus";
			// Group bonuses - add "Bonus" for distinguish bonus and ability
			foreach (KeyValuePair<string, object> bonusPair in groupBonuses) {

				string newKey = bonusPair.Key + bonus;
				object value = bonusPair.Value;
				propDict.Add(newKey, value);
			}

			if (groupMembers.Count == 1) {
				foreach (var pair in groupMembers[0].getPropertyToDisplay()) {
					propDict.Add(pair.Key, pair.Value);
				}
			} else {
				var summaryDict = new Dictionary<object, EditablePair<string, int>>();
				foreach (IMovableGameObject imgo in groupMembers) {
					addObjectPropertyToDict(imgo.getPropertyToDisplay(), summaryDict);
				}
				foreach (var pair in createCommonPropDict(summaryDict, groupMembers[0].getPropertyToDisplay())) {
					propDict.Add(pair.Key, pair.Value);
				}
			}


			return propDict;
		}

		public override void removeMember(IMovableGameObject m) {
			base.removeMember(m);
			var imgoBonus = m.onGroupAdd();
			foreach (var bonusPair in imgoBonus) {
				if (groupBonuses.ContainsKey(bonusPair.Key)) {
					// Add find type and add value
					groupBonuses[bonusPair.Key] =
						callPropertyMathViaReflection(Property<object>.Operator.Minus, groupBonuses[bonusPair.Key], bonusPair.Value);
				}
			}
		}

		/// <summary>
		/// Function  calculate basic bonuses and save them in groupBonuses
		/// (Attack and Deffence +1 per each 3 members)
		/// </summary>
		public void countBasicBonuses() {
			groupBonuses = new Dictionary<string, object>();

			// Quantitative bonus
			var quantitativeBonus = 1 + groupMembers.Count / 3;
			groupBonuses.Add(PropertyEnum.Attack.ToString(), new Property<int>(quantitativeBonus));
			groupBonuses.Add(PropertyEnum.Deffence.ToString(), new Property<int>(quantitativeBonus));
			groupBonuses.Add(PropertyEnum.Speed.ToString(), new Property<float>(0));

		}

		public bool IsSelected {
			get { return isSelected; }
		}

		public Dictionary<string, object> GroupBonusDict {
			get { return groupBonuses; }
			set { groupBonuses = value; }
		}
	}
	#endregion

	#region GroupStatic

	class GroupStatics : IGroup<IStaticGameObject> {
		public GroupStatics() : base(new Team("None")) { }

		public GroupStatics(TeamControl.Team own) : base(own) { }

		public override Dictionary<string, object> getPropertyToDisplay() {
			var propDict = new Dictionary<string, object>();
			propDict.Add(PropertyEnum.Team.ToString(), owner);
			if (groupMembers.Count == 1) {
				foreach (var pair in groupMembers[0].getPropertyToDisplay()) {
					propDict.Add(pair.Key, pair.Value);
				}
			} else {
				var summaryDict = new Dictionary<object, EditablePair<string, int>>();
				foreach (IStaticGameObject isgo in groupMembers) {
					addObjectPropertyToDict(isgo.getPropertyToDisplay(), summaryDict);
				}
				foreach (var pair in createCommonPropDict(summaryDict, groupMembers[0].getPropertyToDisplay())) {
					propDict.Add(pair.Key, pair.Value);
				}
			}

			return propDict;
		}


	}

}
	#endregion

