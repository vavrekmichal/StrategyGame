using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.FightMgr {

	/// <summary>
	/// Controls every fight and occupation. Creating them, updating them and ending them.
	/// </summary>
	class FightManager : IFightManager {

		// Dictionary with information about group action (Occupy/Attack)
		private Dictionary<GroupMovables, ActionAnswer> offensiveActionDict; 

		// Occupations
		private List<Occupation> occupationList;
 
		// Fights
		private List<Fight> fightList;

		// Objects on the way
		private Dictionary<IMovableGameObject, GroupMovables> onWayToTargetDict;

		// Informations about which object attack an other
		private Dictionary<IMovableGameObject, IGameObject> attackersTarget; 

		/// <summary>
		/// Creates instance of FightManager and initializes objects.
		/// </summary>
		public FightManager() {
			attackersTarget = new Dictionary<IMovableGameObject, IGameObject>();
			onWayToTargetDict = new Dictionary<IMovableGameObject, GroupMovables>();
			offensiveActionDict = new Dictionary<GroupMovables, ActionAnswer>();
			occupationList = new List<Occupation>();
			fightList = new List<Fight>();
		}

		#region public 

		/// <summary>
		/// Updates all fights and occupations.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			var occupCopy = new List<Occupation>(occupationList);
			foreach (var occ in occupCopy) {
				if (occ.Check(delay)) {
					occupationList.Remove(occ);
				}
			}

			var fightCopy = new List<Fight>(fightList);
			foreach (var fight in fightCopy) {
				if (fight.CheckDistance()) {
					fightList.Remove(fight);
				}
			}
		}

		/// <summary>
		/// Send attacker to fight destiantion when it is possible (CheckActionPossibility).
		/// </summary>
		/// <param name="group">The attacking group.</param>
		/// <param name="gameObject">The attacked object.</param>
		public void Attack(GroupMovables group, IGameObject gameObject) {

			// Check if target is already under attack of this group or if the target is under attack so
			// group is added to deffenders.
			if (!CheckActionPossibility(group, gameObject)) {
				return;
			}

			Game.IMoveManager.GoToTarget(group, gameObject, this);
			offensiveActionDict.Add(group, ActionAnswer.Attack);

			foreach (IMovableGameObject imgo in group) {
				attackersTarget.Add(imgo, gameObject);
				onWayToTargetDict.Add(imgo, group);
			}
		}

		/// <summary>
		/// Controls if target can be occupied or if is now currently occupied.
		/// If it is possible so attackers are sent to the destination and are registered.
		/// </summary>
		/// <param name="group">The attacking group.</param>
		/// <param name="gameObject">The occupation target.</param>
		public void Occupy(GroupMovables group, IGameObject gameObject) {

			if (gameObject.OccupyTime < 0) {
				return;
			}

			// The object is already occupied by this group 
			if (offensiveActionDict.ContainsKey(group) && 
				offensiveActionDict[group] == ActionAnswer.Occupy && 
				attackersTarget[group[0]] == gameObject) {
				return;
			}

			if (!CheckActionPossibility(group, gameObject)) {
				return;
			}

			// Object can be occupied
			Game.IMoveManager.GoToTarget(group, gameObject, this);
			offensiveActionDict.Add(group, ActionAnswer.Occupy);

			foreach (IMovableGameObject imgo in group) {
				attackersTarget.Add(imgo, gameObject);
				onWayToTargetDict.Add(imgo, group);
			}
		}

		/// <summary>
		/// Recieves information when object reached the destination. Checks if object attacking or occupying and 
		/// creates the appropriate class.
		/// </summary>
		/// <param name="imgo">The object in destiantion.</param>
		public void MovementFinished(IMovableGameObject imgo) {
			var onWayCopy = new Dictionary<IMovableGameObject, GroupMovables>(onWayToTargetDict);
			var group = onWayCopy[imgo];
			var gameObject = attackersTarget[imgo];
			var moveMgr = Game.IMoveManager;
			// Remove all object which goint to same target from watch lists.
			foreach (IMovableGameObject item in onWayCopy[imgo]) {
				onWayToTargetDict.Remove(item);
				attackersTarget.Remove(item);
				moveMgr.UnlogFromFinishMoveReciever(item);
			}

			if (offensiveActionDict[onWayCopy[imgo]] == ActionAnswer.Attack) {
				// Create Fight
				fightList.Add(new Fight(group, gameObject));
			} else {
				// Create Occupation
				occupationList.Add(new Occupation(group, gameObject));
			}
			offensiveActionDict.Remove(onWayCopy[imgo]);
		}

		/// <summary>
		/// Unlogs given object from watch lists.
		/// </summary>
		/// <param name="imgo">The object which interrupted the movement.</param>
		public void MovementInterupted(IMovableGameObject imgo) {
			if (onWayToTargetDict.ContainsKey(imgo)) {
				offensiveActionDict.Remove(onWayToTargetDict[imgo]);
				onWayToTargetDict.Remove(imgo);
				attackersTarget.Remove(imgo);
			}
		}
		#endregion
		
		/// <summary>
		/// Checks if given group already fighting or occupying given object.
		/// </summary>
		/// <param name="group">The checking group.</param>
		/// <param name="gameObject">The target of the given group</param>
		/// <returns>Return if the group can do its action.</returns>
		private bool CheckActionPossibility(GroupMovables group, IGameObject gameObject) {
			foreach (var item in fightList) {
				if (item.ContainsAttackingGroup(group, gameObject)) {
					return false;
				}
				if (item.Contains(gameObject)) {
					item.AddGroup(group);
					return false;
				}
			}
			foreach (var item in occupationList) {
				if (item.Contains(gameObject)) {
					item.AddGroup(group);
					return false;

				}
			}
			return true;
		}
	}
}
