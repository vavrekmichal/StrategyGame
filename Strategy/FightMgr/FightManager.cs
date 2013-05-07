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

		private Dictionary<GroupMovables, ActionAnswer> fightsDict; // Dictionary with information about group action (Occupy/Attack)

		private List<Occupation> occupationList; // Every occupation 

		private Dictionary<IMovableGameObject, GroupMovables> onWayToTargetDict; // Dictionary with object on the way

		private Dictionary<IMovableGameObject, IGameObject> attackersTarget; // Dictionary with information about witch obejct attack an other

		public FightManager() {
			attackersTarget = new Dictionary<IMovableGameObject, IGameObject>();
			onWayToTargetDict = new Dictionary<IMovableGameObject, GroupMovables>();
			fightsDict = new Dictionary<GroupMovables, ActionAnswer>();
			occupationList = new List<Occupation>();
		}

		public void Update(float delay) {
			var occupCopy = new List<Occupation>(occupationList);
			foreach (var occ in occupCopy) {
				if (occ.Check(delay)) {
					occupationList.Remove(occ);
				}
			}
		}

		public void Attack(GroupMovables group, IGameObject gameObject) {
			if (gameObject is IStaticGameObject) {
				Console.WriteLine("Utocis na static");
			}
			if (gameObject is IMovableGameObject) {
				Console.WriteLine("Utocis na movable");
				//Game.removeObject(gameObject);
				gameObject.TakeDamage(1000);
			}
		}

		public void Occupy(GroupMovables group, IGameObject gameObject) {


			if (gameObject.OccupyTime < 0) {
				Console.WriteLine("Nelze obsadit");
				return;
			}

			if (fightsDict.ContainsKey(group)) {
				return;
			}

			// Object can be occupied
			Game.IMoveManager.GoToTarget(group, gameObject, this);
			fightsDict.Add(group, ActionAnswer.Occupy);

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

			if (fightsDict[onWayCopy[imgo]] == ActionAnswer.Attack) {
				//TODO attack
			} else {
				occupationList.Add(new Occupation(group, gameObject));
			}
			fightsDict.Remove(onWayCopy[imgo]);
		}

		public void MovementInterupted(IMovableGameObject imgo) {
			if (onWayToTargetDict.ContainsKey(imgo)) {
				onWayToTargetDict.Remove(imgo);
				attackersTarget.Remove(imgo);
			}
		}
	}
}
