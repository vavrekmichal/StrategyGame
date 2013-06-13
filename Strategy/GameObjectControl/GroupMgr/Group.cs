using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using System.Collections;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;
using System.Reflection;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects;

namespace Strategy.GameObjectControl.GroupMgr {

	/// <summary>
	/// Stores game objects and completes their bonuses and actions. Also passes information between members
	/// (leader commands, etc.)
	/// </summary>
	/// <typeparam name="T">The type of the group members.</typeparam>
	public abstract class IGroup<T> : IEnumerable
		where T : class, IGameObject {

		protected List<T> groupMembers;
		protected Property<Team> team;
		protected List<IGameAction> groupIGameActionList;

		/// <summary>
		/// Initializes Group and sets team as owner of this group.
		/// </summary>
		/// <param name="owner">The team which owns this group.</param>
		public IGroup(Team owner) {
			groupMembers = new List<T>();
			team = new Property<Team>(owner);
			groupIGameActionList = new List<IGameAction>();
		}

		/// <summary>
		/// When group has one member, collects all its game actions and adds them to group game actions.
		/// Else returns just group game actions.
		/// </summary>
		/// <returns>Returns the list with game actions.</returns>
		public List<IGameAction> GetGroupIGameActions() {
			var result = new List<IGameAction>(groupIGameActionList);
			if (groupMembers.Count ==1) {
				foreach (var action in groupMembers[0].GetIGameActions()) {
					result.Add(action);
				}
			}
			return result;
		}

		/// <summary>
		/// Returns a current number of members.
		/// </summary>
		public int Count {
			get { return groupMembers.Count; }
		}

		/// <summary>
		/// Checks if the group contains the member.
		/// </summary>
		/// <param name="m">The name of the searching member.</param>
		/// <returns>Return if the group contarins the member.</returns>
		public virtual bool HasMember(T m) {
			foreach (var member in groupMembers) {
				if (member == m) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Inserts member to the group.
		/// </summary>
		/// <param name="m">The inserting member.</param>
		public virtual void InsertMemeber(T m) {
			groupMembers.Add(m);
		}

		/// <summary>
		/// Removes member from the group.
		/// </summary>
		/// <param name="m">The removing member.</param>
		public virtual void RemoveMember(T m) {
			groupMembers.Remove(m);
		}

		/// <summary>
		/// Returns groupMembers enumarator.
		/// </summary>
		/// <returns>Returns groupMembers enumarator.</returns>
		public IEnumerator GetEnumerator() {
			return groupMembers.GetEnumerator();
		}

		/// <summary>
		/// Returns dictionary with the group Properties. The virtual class returns empty.
		/// </summary>
		/// <returns>Returns dictionary with the group Properties</returns>
		public virtual Dictionary<string, object> GetPropertyToDisplay() {
			return new Dictionary<string, object>();
		}

		/// <summary>
		/// Adds given Property to the dictionary. Stores Properties (object) to given dictionary (summaryDict)
		/// and keeps their name and frequency.
		/// </summary>
		/// <param name="isgoPropDict">The inserting dictionary.</param>
		/// <param name="summaryDict">The dictionary to which it is inserted. (will contain (Property,[name, frequency])</param>
		protected static void AddPropertyToDict(Dictionary<string, object> isgoPropDict, Dictionary<object, EditablePair<string, int>> summaryDict) {
			foreach (KeyValuePair<string, object> property in isgoPropDict) {		//object is real Property<T>	
				if (summaryDict.ContainsKey(property.Value)) {
					EditablePair<string, int> pair = summaryDict[property.Value];
					summaryDict[property.Value].Item2++;
				} else {
					summaryDict.Add(property.Value, new EditablePair<string, int>(property.Key, 1));
				}
			}
		}

		/// <summary>
		/// Finds all common Properties and inserts them to result dictionary, the rest of the Properties is replaced by the new 
		/// Property with same name and '?' as value.
		/// </summary>
		/// <param name="summaryDict">The dictionary with all group Properties and thies names and frequency (Property,[name, frequency]).</param>
		/// <returns></returns>
		protected Dictionary<string, object> CreateCommonPropDict(Dictionary<object, EditablePair<string, int>> summaryDict) {
			int groupCount = groupMembers.Count;
			var result = new Dictionary<string, object>();
			var notEveryone = new List<string>();
			foreach (KeyValuePair<object, EditablePair<string, int>> propPair in summaryDict) {
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

		/// <summary>
		/// The group indexer.
		/// </summary>
		/// <param name="i">The order of the member.</param>
		/// <returns>Returns a member of the i-th place.</returns>
		public T this[int i] {
			get {
				return groupMembers[i];
			}
			set {
				groupMembers[i] = value;
			}
		}

		/// <summary>
		/// Returns a reference to the group owner team.
		/// </summary>
		public Team Team {
			get { return team.Value; }
		}

	}

	#region GroupMovables

	/// <summary>
	/// IGroup specified on IMovbleGameObject with extra functions.
	/// </summary>
	public class GroupMovables : IGroup<IMovableGameObject> {

		private Dictionary<string, object> groupBonuses;

		private bool isSelected;

		/// <summary>
		/// Initializes group and sets given Team as the group owner
		/// </summary>
		/// <param name="own"></param>
		public GroupMovables(TeamControl.Team own)
			: base(own) {
			CountBasicBonuses();
			groupIGameActionList.Add(new DieAction<IMovableGameObject>(groupMembers));
			groupIGameActionList.Add(new StopMoveAction(groupMembers));

		}

		/// <summary>
		/// Uses reflection to caluclate result of math operation with two generic Properties (represents as object).
		/// </summary>
		/// <param name="op">The math operator.</param>
		/// <param name="p1">The firts Property</param>
		/// <param name="p2">The second Property.</param>
		/// <returns>Returns result of the operation.</returns>
		private object CallPropertyMathViaReflection(Property<object>.Operator op, object p1, object p2) {
			var type = p1.GetType().GetGenericArguments()[0];

			// Using reflection to call function SimpleMath with generic parameter
			MethodInfo method = p1.GetType().GetMethod("SimpleMath");
			List<object> args = new List<object>();
			args.Add(op);
			args.Add(p2);
			return method.Invoke(p1, args.ToArray());

		}

		/// <summary>
		/// Counts basic bonuses for selected group (Attack and Deffence bonus +1 per each 3 members) 
		/// and collects bonuses from members (onGroupAdd).
		/// Finally sets a bonus dictionary for each member of the group.
		/// </summary>
		public void Select() {
			if (isSelected) {
				return;
			}
			isSelected = true;

			// Basic bonuses to the Attack and the Deffence ( 1 per each 3 members)
			CountBasicBonuses();

			// Collect bonuses
			foreach (IMovableGameObject imgo in groupMembers) {
				var imgoBonus = imgo.OnGroupAdd();
				foreach (var bonusPair in imgoBonus) {
					if (groupBonuses.ContainsKey(bonusPair.Key)) {

						// Find type and add value. If Properties has not same type then isn't added
						var type = bonusPair.Value.GetType();
						if (type.GetGenericTypeDefinition() == typeof(Property<>) &&
							type == groupBonuses[bonusPair.Key].GetType()) {

							groupBonuses[bonusPair.Key] = CallPropertyMathViaReflection(
								Property<object>.Operator.Plus, groupBonuses[bonusPair.Key], bonusPair.Value);
						}
					} else {
						groupBonuses.Add(bonusPair.Key, bonusPair.Value);
					}
				}
				imgo.SetGroupBonuses(groupBonuses);
			}
		}

		/// <summary>
		/// Counts and sets a bonuses again (Select).
		/// </summary>
		public void Reselect() {
			isSelected = false;
			Select();
		}

		/// <summary>
		/// Colects all answers and returns the answer with the highest priority. 
		/// </summary>
		/// <param name="point">The position of mouse when was clicked.</param>
		/// <param name="hitObject">The result of a HitTest.</param>
		/// <param name="isFriendly">The information if the hitted object is friendly.</param>
		/// <param name="isMovableGameObject">The information if the hitted object is movable.</param>
		/// <returns></returns>
		public ActionAnswer OnMouseAction(Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			ActionAnswer groupAnswer = ActionAnswer.None;
			foreach (IMovableGameObject imgo in groupMembers) {
				ActionAnswer answer = imgo.OnMouseAction(point, hitObject, isFriendly, isMovableGameObject);
				if (answer > groupAnswer) {
					groupAnswer = answer;
				}
			}
			return groupAnswer;
		}

		/// <summary>
		/// Overrides base function and collects Properties from each member of the group.
		/// Common Properties lets unchanged, other replaced by new with '?' value.
		/// </summary>
		/// <returns>Returns dictionary with all Properties (common unganed, others with '?' value).</returns>
		public override Dictionary<string, object> GetPropertyToDisplay() {
			var propDict = new Dictionary<string, object>();
			propDict.Add(PropertyEnum.Team.ToString(), team);
			var bonusesCopy = new Dictionary<string, object>(groupBonuses);

			const string bonus = "Bonus";

			// Group bonuses - add "Bonus" for distinguish bonus and ability
			foreach (KeyValuePair<string, object> bonusPair in groupBonuses) {
				string newKey = bonusPair.Key + bonus;
				object value = bonusPair.Value;
				propDict.Add(newKey, value);
			}

			if (groupMembers.Count == 1) {
				// Group has just one member
				foreach (var pair in groupMembers[0].GetPropertyToDisplay()) {
					propDict.Add(pair.Key, pair.Value);
				}
			} else {
				var summaryDict = new Dictionary<object, EditablePair<string, int>>();
				foreach (IMovableGameObject imgo in groupMembers) {
					AddPropertyToDict(imgo.GetPropertyToDisplay(), summaryDict);
				}
				foreach (var pair in CreateCommonPropDict(summaryDict)) {
					propDict.Add(pair.Key, pair.Value);
				}
			}
			return propDict;
		}

		/// <summary>
		/// Removes member from the group and also removes its bonuses.
		/// </summary>
		/// <param name="m">The removing member.</param>
		public override void RemoveMember(IMovableGameObject m) {
			base.RemoveMember(m);
			var imgoBonus = m.OnGroupAdd();
			foreach (var bonusPair in imgoBonus) {
				if (groupBonuses.ContainsKey(bonusPair.Key)) {
					// Add find type and add value
					groupBonuses[bonusPair.Key] =
						CallPropertyMathViaReflection(Property<object>.Operator.Minus, groupBonuses[bonusPair.Key], bonusPair.Value);
				}
			}
		}

		/// <summary>
		/// Calculates basic bonuses and saves them in groupBonuses.
		/// (Attack and Deffence +1 per each 3 members)
		/// </summary>
		public void CountBasicBonuses() {
			groupBonuses = new Dictionary<string, object>();

			// Quantitative bonuses
			var quantitativeBonus = 1 + groupMembers.Count / 3;
			groupBonuses.Add(PropertyEnum.Attack.ToString(), new Property<int>(quantitativeBonus));
			groupBonuses.Add(PropertyEnum.Deffence.ToString(), new Property<int>(quantitativeBonus));
			groupBonuses.Add(PropertyEnum.Speed.ToString(), new Property<float>(0));

		}

		/// <summary>
		/// Indicates if the group is in selected mode (is not just info group),
		/// it means the bonuses are counted and setted.
		/// </summary>
		public bool IsSelected {
			get { return isSelected; }
		}

		/// <summary>
		/// Gets or sets dictionary with group bonuses.
		/// </summary>
		public Dictionary<string, object> GroupBonusDict {
			get { return groupBonuses; }
			set { groupBonuses = value; }
		}
	}
	#endregion

	#region GroupStatic

	/// <summary>
	/// IGroup specified on IStaticGameObject with extra functions.
	/// </summary>
	public class GroupStatics : IGroup<IStaticGameObject> {

		/// <summary>
		/// Creates a group without team and members.
		/// </summary>
		public GroupStatics() : base(new Team("None")) { }

		/// <summary>
		/// Initializes group and adds group game action (DieAction).
		/// </summary>
		/// <param name="own">The owner of the group.</param>
		public GroupStatics(TeamControl.Team own)
			: base(own) {
			groupIGameActionList.Add(new DieAction<IStaticGameObject>(groupMembers));
		}
		
		/// <summary>
		/// Collects group Properties, check if they are common (real valeu/'?')
		/// and returns a dictionary which contains them.
		/// If the group has just one member so the group returns its Properties 
		/// (doesn't controls common Properties).
		/// </summary>
		/// <returns></returns>
		public override Dictionary<string, object> GetPropertyToDisplay() {
			var propDict = new Dictionary<string, object>();
			propDict.Add(PropertyEnum.Team.ToString(), team);
			if (groupMembers.Count == 1) {
				foreach (var pair in groupMembers[0].GetPropertyToDisplay()) {
					propDict.Add(pair.Key, pair.Value);
				}
			} else {
				// Collects all Properties and checks if they are common or not.
				var summaryDict = new Dictionary<object, EditablePair<string, int>>();
				foreach (IStaticGameObject isgo in groupMembers) {
					AddPropertyToDict(isgo.GetPropertyToDisplay(), summaryDict);
				}
				foreach (var pair in CreateCommonPropDict(summaryDict)) {
					propDict.Add(pair.Key, pair.Value);
				}
			}
			return propDict;
		}
	}
}
	#endregion

