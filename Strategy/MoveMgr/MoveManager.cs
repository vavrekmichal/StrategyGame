using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.MoveMgr {
	class MoveManager : IMoveManager {

		private const int randConst = 40;

		private Dictionary<IMovableGameObject, IStaticGameObject> moveMgrControledDict;

		private static MoveManager instance;

		public static MoveManager getInstance() {
			if (instance == null) {
				instance = new MoveManager();
			}
			return instance;
		}

		private MoveManager() {
			moveMgrControledDict = new Dictionary<IMovableGameObject, IStaticGameObject>();
		}

		private Mogre.Vector3 randomizeVector(Mogre.Vector3 v) {
			Random r = new Random();
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
		/// <param name="point1">first point</param>
		/// <param name="point2">second point</param>
		/// <param name="squaredDistance">squared distance (no root needed)</param>
		/// <returns>distance is lower (true) or not</returns>
		private bool checkDistance(Mogre.Vector3 point1, Mogre.Vector3 point2, double squaredDistance) {
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
		/// <param name="imgo">object which reached destination</param>
		/// <param name="isgo">target of move</param>
		private void reachedDestiantion(IMovableGameObject imgo, IStaticGameObject isgo) { //TODO implemenyt
			if (imgo.Visible) {

				imgo.stop();
				var answer = isgo.reactToInitiative(ActionReason.targetInDistance, imgo);
				switch (answer) {
					default:
						break;
				}

			}
		}

		/// <summary>
		/// Create List of positions around given point.
		/// </summary>
		/// <param name="count">number of positions</param>
		/// <param name="position">center position</param>
		/// <returns>List with positions around given position</returns>
		private List<Mogre.Vector3> preparePositions(int count, Mogre.Vector3 position) {

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
							positionToGo = randomizeVector(positionToGo);
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

		public void goToLocation(GroupMovables group, Mogre.Vector3 to) {
			var destinations = preparePositions(group.Count, to);
			foreach (IMovableGameObject imgo in group) {
				imgo.setNextLocation(destinations[0]);
				destinations.RemoveAt(0);
			}
		}

		public void goToLocation(IMovableGameObject imgo, Mogre.Vector3 to) {
			var a = new LinkedList<Mogre.Vector3>();
			a.AddLast(to);
			imgo.setNextLocation(a);
		}



		public void runAwayFrom(GroupMovables group, Mogre.Vector3 from) {
			throw new NotImplementedException();
		}

		public void update(float delay) {
			List<IMovableGameObject> toRemove = new List<IMovableGameObject>();
			foreach (var trev in moveMgrControledDict) {
				double sqPickUpDist = trev.Value.PickUpDistance * trev.Value.PickUpDistance;
				if (checkDistance(trev.Key.Position, trev.Value.Position, sqPickUpDist)) {
					toRemove.Add(trev.Key);
				}
			}
			foreach (IMovableGameObject imgo in toRemove) {
				reachedDestiantion(imgo, moveMgrControledDict[imgo]);
			}
		}

		public void goToTarget(GroupMovables group, IStaticGameObject target) {
			List<Mogre.Vector3> destinations = preparePositions(group.Count, target.Position);
			foreach (IMovableGameObject imgo in group) {
				imgo.goToTarget(destinations[0], this);
				destinations.RemoveAt(0);
				moveMgrControledDict.Add(imgo, target);
			}

		}


		public void interuptMove(IMovableGameObject imgo) {
			moveMgrControledDict.Remove(imgo);
		}
	}
}
