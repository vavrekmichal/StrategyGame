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
	class FightManager : IFightManager {

		private Dictionary<GroupMovables, ActionAnswer> offensiveActionDict; // Dictionary with information about group action (Occupy/Attack)

		private List<Occupation> occupationList; // Every occupation 
		private List<Fight> fightList;

		private Dictionary<IMovableGameObject, GroupMovables> onWayToTargetDict; // Dictionary with object on the way

		private Dictionary<IMovableGameObject, IGameObject> attackersTarget; // Dictionary with information about witch obejct attack an other

		public FightManager() {
			attackersTarget = new Dictionary<IMovableGameObject, IGameObject>();
			onWayToTargetDict = new Dictionary<IMovableGameObject, GroupMovables>();
			offensiveActionDict = new Dictionary<GroupMovables, ActionAnswer>();
			occupationList = new List<Occupation>();
			fightList = new List<Fight>();
		}

		public void Update(float delay) {
			var occupCopy = new List<Occupation>(occupationList);
			foreach (var occ in occupCopy) {
				if (occ.Check(delay)) {
					occupationList.Remove(occ);
				}
			}

			var fightCopy = new List<Fight>(fightList);
			foreach (var fight in fightCopy) {
				if (fight.Check(delay)) {
					fightList.Remove(fight);
				}
			}
		}

		public void Attack(GroupMovables group, IGameObject gameObject) {
			if (gameObject is IStaticGameObject) {
				Console.WriteLine("Utocis na static");
			}
			if (gameObject is IMovableGameObject) {
				Console.WriteLine("Utocis na movable");
			}

			if (fightList.Count>5) {
				Console.WriteLine("Kurva");
			}

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
		/// Occupy controls if target can be occupied or if is now currently occupied.
		/// If it is possible so attackers are sent to the destination and are registered.
		/// </summary>
		/// <param name="group">Attackers</param>
		/// <param name="gameObject">Target</param>
		public void Occupy(GroupMovables group, IGameObject gameObject) {

			if (gameObject.OccupyTime < 0) {
				Console.WriteLine("Nelze obsadit");
				return;
			}

			// The object is already occupied by this group 
			if (offensiveActionDict.ContainsKey(group) && offensiveActionDict[group] == ActionAnswer.Occupy && attackersTarget[group[0]] == gameObject) {
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

		public void MovementFinished(IMovableGameObject imgo) {
			var onWayCopy = new Dictionary<IMovableGameObject, GroupMovables>(onWayToTargetDict);
			var group = onWayCopy[imgo];
			var gameObject = attackersTarget[imgo];
			var moveMgr = Game.IMoveManager;
			foreach (IMovableGameObject item in onWayCopy[imgo]) {
				onWayToTargetDict.Remove(item);
				attackersTarget.Remove(item);
				moveMgr.UnlogFromFinishMoveReciever(item);
			}

			if (offensiveActionDict[onWayCopy[imgo]] == ActionAnswer.Attack) {
				//TODO attack
				//gameObject.TakeDamage(1000);

				// List with GameObject in gameObject's ShoutDistance and recursively called to the other
				//var objectsInShoutDistance = new List<IGameObject>();
				//gameObject.Shout(objectsInShoutDistance);
				//var groupDeff = Game.GroupManager.CreateSelectedGroup(objectsInShoutDistance);

				// zkontroluj jestli se na to uz nejak neutoci a kdyztak jen addToGroup a bude hotovo.
				fightList.Add(new Fight(group, gameObject));

			} else {
				occupationList.Add(new Occupation(group, gameObject));
			}
			offensiveActionDict.Remove(onWayCopy[imgo]);
		}

		public void MovementInterupted(IMovableGameObject imgo) {

			if (onWayToTargetDict.ContainsKey(imgo)) {
				offensiveActionDict.Remove(onWayToTargetDict[imgo]);
				onWayToTargetDict.Remove(imgo);
				attackersTarget.Remove(imgo);
			}
		}

		private bool CheckActionPossibility(GroupMovables group, IGameObject gameObject) {
			foreach (var item in fightList) {
				if (item.ContainsAttackingGroup(group, gameObject)) {
					Console.WriteLine("Presne je to ona");
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
