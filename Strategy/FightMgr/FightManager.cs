using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.FightMgr {
	class FightManager : IFightManager{

		private Dictionary<GroupMovables, ActionAnswer> fightsDict; // Dictionary with information about group action (Occupy/Attack)

		private List<Occupation> occupationList; // Every occupation 

		private Dictionary<IMovableGameObject,GroupMovables> onWayToTargetDict; // Dictionary with object on the way

		private Dictionary<IMovableGameObject, object> attackersTarget; // Dictionary with information about witch obejct attack an other
		
		public FightManager() {
			attackersTarget = new Dictionary<IMovableGameObject, object>();
			onWayToTargetDict = new Dictionary<IMovableGameObject, GroupMovables>();
			fightsDict = new Dictionary<GroupMovables, ActionAnswer>();
			occupationList = new List<Occupation>();
		}

		public void update(float delay){
			var occupCopy = new List<Occupation>(occupationList);
			foreach (var occ in occupCopy) {
				if (occ.check(delay)) {
					occupationList.Remove(occ);
				}
			}
		}

		public void attack(GroupMovables group, object gameObject) {
			if (gameObject is IStaticGameObject) {
				Console.WriteLine("Utocis na static");
			}
			if (gameObject is IMovableGameObject) {
				Console.WriteLine("Utocis na movable");
			}
		}

		public void occupy(GroupMovables group, object gameObject) {

			var castedIsgo = gameObject as IStaticGameObject;
			if (castedIsgo != null) {
				
				if (castedIsgo.OccupyTime < 0) {
					Console.WriteLine("Nelze obsadit");
					return;					
				}
				Console.WriteLine("Obsazujes static");
			} else {
				var castedImgo = gameObject as IMovableGameObject;
				
				if (castedImgo.OccupyTime < 0) {
					Console.WriteLine("Nelze obsadit");
					return;
				}
				Console.WriteLine("Obsazujes movable");
			}

			
			if (fightsDict.ContainsKey(group)) {
				return;
			}

			// Object can be occupied
			Game.IMoveManager.goToTarget(group, gameObject, this);
			fightsDict.Add(group, ActionAnswer.Occupy);

			foreach (IMovableGameObject imgo in group) {
				attackersTarget.Add(imgo, gameObject);
				onWayToTargetDict.Add(imgo, group);
			}
		}

		public void movementFinished(IMovableGameObject imgo) {
			var onWayCopy = new Dictionary<IMovableGameObject, GroupMovables>(onWayToTargetDict);
			var group = onWayCopy[imgo];
			var gameObject = attackersTarget[imgo];
			var moveMgr = Game.IMoveManager;
			foreach (IMovableGameObject item in onWayCopy[imgo]) {
				onWayToTargetDict.Remove(item);
				attackersTarget.Remove(item);
				moveMgr.unlogFromFinishMoveReciever(item);
			}

			if (fightsDict[onWayCopy[imgo]] == ActionAnswer.Attack) {
				//TODO attack
			} else {
				occupationList.Add(new Occupation(group, gameObject, TimeSpan.FromSeconds(2)));
			}
			fightsDict.Remove(onWayCopy[imgo]);
		}

		public void movementInterupted(IMovableGameObject imgo) {
			if (onWayToTargetDict.ContainsKey(imgo)) {
				onWayToTargetDict.Remove(imgo);
				attackersTarget.Remove(imgo);
			}
		}
	}
}
