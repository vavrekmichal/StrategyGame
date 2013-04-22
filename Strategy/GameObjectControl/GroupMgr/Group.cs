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

namespace Strategy.GameObjectControl.GroupMgr {
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

		public virtual Dictionary<PropertyEnum, object> getPropertyToDisplay() {
			return new Dictionary<PropertyEnum, object>();
		}

		protected void addObjectPropertyToDict(Dictionary<PropertyEnum, object> isgoPropDict, Dictionary<object, EditablePair<PropertyEnum, int>> summaryDict) {
			foreach (KeyValuePair<PropertyEnum, object> property in isgoPropDict) {		//object is real Property<T>	
				if (summaryDict.ContainsKey(property.Value)) {
					EditablePair<PropertyEnum, int> pair = summaryDict[property.Value];
					summaryDict[property.Value].Item2 ++;
				} else {
					summaryDict.Add(property.Value, new EditablePair<PropertyEnum, int>(property.Key, 1));
				}
			}
		}

		protected Dictionary<PropertyEnum, object> createCommonPropDict(Dictionary<object, EditablePair<PropertyEnum, int>> summaryDict, Dictionary<PropertyEnum, object> isgoPropDict) {
			int groupCount = groupMembers.Count;
			var result = new Dictionary<PropertyEnum, object>();
			var notEveryone = new List<PropertyEnum>();
			foreach (KeyValuePair<object, EditablePair<PropertyEnum, int>> propPair in summaryDict) {
				//PropertyEnum name = isgoPropDict.FirstOrDefault(x => x.Value == propPair.Key).Key; //gets string name searched by object
				PropertyEnum name = propPair.Value.Item1;
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

		public static Dictionary<PropertyEnum, object> baseTemplateBonusDict;		//static template for creating new Group with setted basic bonuses

		/// <summary>
		/// Static constructor setted baseTemp.. dictionary with basic stats
		/// </summary>
		static GroupMovables() {
			baseTemplateBonusDict = new Dictionary<PropertyEnum, object>();
			baseTemplateBonusDict.Add(PropertyEnum.Attack, new Property<int>(1));
			baseTemplateBonusDict.Add(PropertyEnum.Deffence, new Property<int>(1));
			baseTemplateBonusDict.Add(PropertyEnum.Speed, new Property<int>(1));
		}

		private Dictionary<PropertyEnum, object> groupBonuses;

		public GroupMovables() : this(new Team("")) { }
		public GroupMovables(TeamControl.Team own)
			: base(own) {
			groupBonuses = new Dictionary<PropertyEnum, object>();

			groupBonuses.Add(PropertyEnum.Attack, new Property<int>(1));
			groupBonuses.Add(PropertyEnum.Deffence, new Property<int>(1));
			groupBonuses.Add(PropertyEnum.Speed, new Property<int>(1));
		}

		public void move(float f) {
			foreach (IMovableGameObject obj in groupMembers) {
				obj.move(f);
			}
		}

		public void nonVisibleMove(float f) {
		}


		public void select() {		// Called when group is changed from informative to selected
			// Need colect bonuses from count and from members
			var countBonus = (int)(groupMembers.Count / 3);
			((Property<int>)groupBonuses[PropertyEnum.Attack]).Value = 1 + countBonus;
			((Property<int>)groupBonuses[PropertyEnum.Deffence]).Value = 1 + countBonus;
			foreach (IMovableGameObject imgo in groupMembers) {
				// Collect bonuses
				var imgoBonus = imgo.onGroupAdd();
				foreach (var bonusPair in imgoBonus) {
					if (groupBonuses.ContainsKey(bonusPair.Key)) { //todo
						// Add find type and add value
					} else {
						groupBonuses.Add(bonusPair.Key, bonusPair.Value);
					}
				}
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


		public override Dictionary<PropertyEnum, object> getPropertyToDisplay() {
			var propDict = new Dictionary<PropertyEnum, object>();
			propDict.Add(PropertyEnum.Team, owner);
			var bonusesCopy = new Dictionary<PropertyEnum, object>(groupBonuses);

			foreach (KeyValuePair<PropertyEnum, object> bonusPair in groupBonuses) {	// Group bonuses - add "Bonus" for distinguish bonus and ability
				PropertyEnum newKey = bonusPair.Key + 50;
				object value = bonusPair.Value;
				propDict.Add(newKey, value);
			}

			if (groupMembers.Count == 1) {
				foreach (var pair in groupMembers[0].getPropertyToDisplay()) {// Just copy - don't want original (team add,...)
					propDict.Add(pair.Key, pair.Value);
				}
			} else {
				var summaryDict = new Dictionary<object, EditablePair<PropertyEnum, int>>();
				foreach (IMovableGameObject imgo in groupMembers) {
					addObjectPropertyToDict(imgo.getPropertyToDisplay(), summaryDict);
				}
				foreach (var pair in createCommonPropDict(summaryDict, groupMembers[0].getPropertyToDisplay())) {
					propDict.Add(pair.Key, pair.Value);
				}
			}


			return propDict;
		}

	}
	#endregion

	#region GroupStatic

	class GroupStatics : IGroup<IStaticGameObject> {
		public GroupStatics() : base(new Team("None")) { }
		public GroupStatics(TeamControl.Team own) : base(own) { }

		public override Dictionary<PropertyEnum, object> getPropertyToDisplay() {
			var propDict = new Dictionary<PropertyEnum, object>();
			propDict.Add(PropertyEnum.Team, owner);
			if (groupMembers.Count == 1) {
				foreach (var pair in groupMembers[0].getPropertyToDisplay()) {// Just copy - don't want original (team add,...)
					propDict.Add(pair.Key, pair.Value);
				}
			} else {
				var summaryDict = new Dictionary<object, EditablePair<PropertyEnum, int>>();
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

