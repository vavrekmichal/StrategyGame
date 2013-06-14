using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.Game_Objects;

namespace Strategy.MoveMgr {
	class MoveManager : IMoveManager {

		private const int randConst = 30;

		private Dictionary<IMovableGameObject, object> moveMgrControledDict;
		private Dictionary<IMovableGameObject, IFinishMovementReciever> finishMoveRecDict;

		public MoveManager() {
			finishMoveRecDict = new Dictionary<IMovableGameObject, IFinishMovementReciever>();
			moveMgrControledDict = new Dictionary<IMovableGameObject, object>();
		}


		private static Random r = new Random();
		private Mogre.Vector3 RandomizeVector(Mogre.Vector3 v) {

			int i = r.Next(4);
			switch (i) {
				case 0: v.x += randConst;
					break;
				case 1: v.x -= randConst;
					break;
				case 2: v.z += randConst;
					break;
				case 3: v.z -= randConst;
					break;
			}
			return v;
		}

		/// <summary>
		/// Returns true when distance between given points is lower then given squaredDistance
		/// </summary>
		/// <param Name="point1">First point</param>
		/// <param Name="point2">Second point</param>
		/// <param Name="squaredDistance">Squared distance (no root needed)</param>
		/// <returns>Distance is lower (true) or not</returns>
		private bool CheckDistance(Mogre.Vector3 point1, Mogre.Vector3 point2, double squaredDistance) {
			double xd = point2.x - point1.x;
			double yd = point2.z - point1.z;
			double distance = xd * xd + yd * yd;
			if (distance > squaredDistance) {
				return false;
			} else {
				return true;
			}
		}

		/// <summary>
		/// Called by MoveMgr when destiantion is reached
		/// </summary>
		/// <param Name="imgo">Object which reached destination</param>
		/// <param Name="isgo">Target of move</param>
		private void ReachedDestiantion(IMovableGameObject imgo, IGameObject gameObject) {

			if (finishMoveRecDict.ContainsKey(imgo)) {
				finishMoveRecDict[imgo].MovementFinished(imgo);
				finishMoveRecDict.Remove(imgo);
			}

			if (imgo.Visible) {
				imgo.Stop();
				gameObject.TargetInSight(imgo);

			}

			
		}

		/// <summary>
		/// Create List of positions around given point.
		/// </summary>
		/// <param Name="count">Number of positions</param>
		/// <param Name="position">Center position</param>
		/// <returns>List with positions around given position</returns>
		private List<Mogre.Vector3> PreparePositions(int count, Mogre.Vector3 position) {

			List<Mogre.Vector3> positionList = new List<Mogre.Vector3>();
			List<Mogre.Vector3> collision = new List<Mogre.Vector3>();
			Mogre.Vector3 positionToGo = position;

			for (int i = 0; i < count; i++) {
				if (collision.Contains(positionToGo)) {
					bool isTaken = true;
					positionToGo = position;

					while (isTaken) {
						if (!collision.Contains(positionToGo)) {
							collision.Add(positionToGo);
							isTaken = false;
						} else {
							positionToGo = RandomizeVector(positionToGo);
						}
					}
					positionList.Add(positionToGo);
				} else {
					collision.Add(position);
					positionList.Add(position);
				}
			}
			return positionList;
		}

		public void GoToLocation(GroupMovables group, Mogre.Vector3 to) {
			var destinations = PreparePositions(group.Count, to);
			foreach (IMovableGameObject imgo in group) {
				imgo.SetNextLocation(destinations[0]);
				destinations.RemoveAt(0);
			}
		}

		public void GoToLocation(IMovableGameObject imgo, Mogre.Vector3 to) {
			var a = new LinkedList<Mogre.Vector3>();
			a.AddLast(to);
			imgo.SetNextLocation(a);
		}




		public void RunAwayFrom(GroupMovables group, Mogre.Vector3 from) {
			throw new NotImplementedException();
		}

		public void Update() {
			var copy = new Dictionary<IMovableGameObject, object>(moveMgrControledDict);

			foreach (KeyValuePair<IMovableGameObject, object> trev in copy) {
				float pickUpDistance;
				Mogre.Vector3 position;

				// trev.Value is IStaticGameObject or IMovableGameObject
				var castedIgo = trev.Value as IGameObject;
				if (castedIgo != null) {
					pickUpDistance = castedIgo.PickUpDistance;
					position = castedIgo.Position;
				} else {
					throw new InvalidCastException();
				}

				// Count distance between objects and compare with pickUpDistance (squared)
				double sqPickUpDist = pickUpDistance * pickUpDistance;
				if (CheckDistance(trev.Key.Position, position, sqPickUpDist)) {
					ReachedDestiantion(trev.Key, (IGameObject)trev.Value);
				}
			}

		}

		public void GoToTarget(GroupMovables group, IGameObject gameObject, IFinishMovementReciever reciever) {
			GoToTargetGroup(group, gameObject);
			foreach (IMovableGameObject imgo in group) {
				finishMoveRecDict.Add(imgo, reciever);
			}

		}

		public void GoToTarget(GroupMovables group, IGameObject gameObject) {
			GoToTargetGroup(group, gameObject);
		}

		private void GoToTargetGroup(GroupMovables group, IGameObject target) {
			List<Mogre.Vector3> destinations = PreparePositions(group.Count, target.Position);
			foreach (IMovableGameObject imgo in group) {
				//imgo.GoToTarget(destinations[0]);
				imgo.GoToTarget(target);
				destinations.RemoveAt(0);
				moveMgrControledDict.Add(imgo, target);
			}

		}

		public void MovementFinished(IMovableGameObject imgo) {
			throw new NotImplementedException();
		}

		public void MovementInterupted(IMovableGameObject imgo) {
			moveMgrControledDict.Remove(imgo);
			if (finishMoveRecDict.ContainsKey(imgo)) {
				finishMoveRecDict[imgo].MovementInterupted(imgo);
				finishMoveRecDict.Remove(imgo);
			}
		}

		public void UnlogFromFinishMoveReciever(IMovableGameObject imgo) {
			finishMoveRecDict.Remove(imgo);
		}


		public void GoToTarget(IMovableGameObject imgo, IGameObject gameObject, IFinishMovementReciever reciever) {
			imgo.GoToTarget(gameObject);
			moveMgrControledDict.Add(imgo, gameObject);

			finishMoveRecDict.Add(imgo, reciever);

		}
	}
}
