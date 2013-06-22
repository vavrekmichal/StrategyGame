using System;
using System.Collections.Generic;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects;

namespace Strategy.MoveMgr {
	/// <summary>
	/// Implements IMoveManager and is implements as the propotype. (No intelligent in the movements).
	/// </summary>
	class MoveManager : IMoveManager {

		private const int randConst = 30;

		private Dictionary<IMovableGameObject, IGameObject> moveMgrControledDict;
		private Dictionary<IMovableGameObject, IFinishMovementReciever> finishMoveRecDict;

		/// <summary>
		/// Initializes the MoveManager.
		/// </summary>
		public MoveManager() {
			finishMoveRecDict = new Dictionary<IMovableGameObject, IFinishMovementReciever>();
			moveMgrControledDict = new Dictionary<IMovableGameObject, IGameObject>();
		}

		#region Public

		/// <summary>
		/// Initializes the loaded movements.
		/// </summary>
		/// <param name="loadedMovements"></param>
		public void Initialize(Dictionary<string, string> loadedMovements) {
			if (loadedMovements == null) {
				return;
			}
			foreach (var item in loadedMovements) {
				if (Game.HitTest.IsObjectControllable(item.Key) && Game.HitTest.IsObjectMovable(item.Key)) {
					var movingObj = Game.HitTest.GetIMGO(item.Key);
					IGameObject target;
					if (Game.HitTest.IsObjectControllable(item.Value)) {
						target = Game.HitTest.GetGameObject(item.Value);
					} else {
						continue;
					}
					GoToTargetIMGO(movingObj, target);
				}
			}

		}

		/// <summary>
		/// Sends objects in the group to the given position. The objects are not sends to the same position,
		/// the positions are prepeted around the given center point.
		/// </summary>
		/// <param name="group">The moving group.</param>
		/// <param name="to">The point of the moving.</param>
		public void GoToLocation(GroupMovables group, Mogre.Vector3 to) {
			var destinations = PreparePositions(group.Count, to);
			foreach (IMovableGameObject imgo in group) {
				imgo.SetNextLocation(destinations[0]);
				destinations.RemoveAt(0);
			}
		}

		/// <summary>
		/// Sends object to the given position. 
		/// </summary>
		/// <param name="imgo">The moving game object.</param>
		/// <param name="to">The point of the moving.</param>
		public void GoToLocation(IMovableGameObject imgo, Mogre.Vector3 to) {
			var a = new LinkedList<Mogre.Vector3>();
			a.AddLast(to);
			imgo.SetNextLocation(a);
		}

		/// <summary>
		/// Updates all controlled movements if objects have reached the destination yet. When the destination
		/// is reached so sends the information to the controller.
		/// </summary>
		public void Update() {
			var copy = new Dictionary<IMovableGameObject, IGameObject>(moveMgrControledDict);

			foreach (KeyValuePair<IMovableGameObject, IGameObject> trev in copy) {
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

				// Counts distance between objects and compare with pickUpDistance (squared)
				double sqPickUpDist = pickUpDistance * pickUpDistance;
				if (CheckDistance(trev.Key.Position, position, sqPickUpDist)) {
					ReachedDestiantion(trev.Key, (IGameObject)trev.Value);
				}
			}

		}

		/// <summary>
		/// Just recieves the information and does nothing.
		/// </summary>
		/// <param name="imgo">The object which reached the destination.</param>
		public void MovementFinished(IMovableGameObject imgo) { }

		/// <summary>
		/// Removes the object from controlled movements and if something controls the movement
		/// so it sends the information to it.
		/// </summary>
		/// <param name="imgo">The object which interupted the movement.</param>
		public void MovementInterupted(IMovableGameObject imgo) {
			moveMgrControledDict.Remove(imgo);
			if (finishMoveRecDict.ContainsKey(imgo)) {
				finishMoveRecDict[imgo].MovementInterupted(imgo);
				finishMoveRecDict.Remove(imgo);
			}
		}

		/// <summary>
		/// Removes the given movable object from the dictionary which contains controled movements.
		/// </summary>
		/// <param name="imgo"></param>
		public void UnlogFromFinishMoveReciever(IMovableGameObject imgo) {
			if (finishMoveRecDict.ContainsKey(imgo)) {
				finishMoveRecDict.Remove(imgo);
			}
		}

		/// <summary>
		/// Sends the group to the position and doesn't register the movement.
		/// </summary>
		/// <param name="group">The group which is sended to the position of the game object.</param>
		/// <param name="gameObject">The object where the group goes.</param>
		public void GoToTarget(GroupMovables group, IGameObject gameObject) {
			GoToTargetGroup(group, gameObject);
		}

		/// <summary>
		/// Sends the group to the position and registers all the objects in the group to controlled movements.
		/// </summary>
		/// <param name="group">The group which is sended to the position of the game object.</param>
		/// <param name="gameObject">The object where the group goes.</param>
		/// <param name="reciever">The IFinishMovementReciever for the movable object.</param>
		public void GoToTarget(GroupMovables group, IGameObject gameObject, IFinishMovementReciever reciever) {
			GoToTargetGroup(group, gameObject);
			foreach (IMovableGameObject imgo in group) {
				finishMoveRecDict.Add(imgo, reciever);
			}

		}

		/// <summary>
		/// Sends the movable object to the position and registers the object to controlled movements.
		/// </summary>
		/// <param name="imgo">The movable object which is sended to the position of the game object.</param>
		/// <param name="gameObject">The object where the movable object goes.</param>
		/// <param name="reciever">The IFinishMovementReciever for the movable object.</param>
		public void GoToTarget(IMovableGameObject imgo, IGameObject gameObject, IFinishMovementReciever reciever) {
			imgo.GoToTarget(gameObject);
			moveMgrControledDict.Add(imgo, gameObject);
			finishMoveRecDict.Add(imgo, reciever);
		}
		#endregion

		private static Random r = new Random();

		/// <summary>
		/// Radomizes the given vector. It means it adds a constatn to a random direction.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
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
		/// Returns true when distance between given points is lower then given squaredDistance.
		/// </summary>
		/// <param Name="point1">The first point</param>
		/// <param Name="point2">The second point</param>
		/// <param Name="squaredDistance">THe squared distance (no root needed)</param>
		/// <returns>The distance is lower (true) or not</returns>
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
		/// Recives the information that the constolled object reached the destination.
		/// Reports the information if some class wants to control this movement and 
		/// sends the information to object to which the imgo travels.
		/// </summary>
		/// <param Name="imgo">The object which reached the destination.</param>
		/// <param Name="isgo">The target of the movement.</param>
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
		/// Creates the List of positions around the given point.
		/// </summary>
		/// <param Name="count">The number of positions</param>
		/// <param Name="position">The center position</param>
		/// <returns>The List with positions around given position</returns>
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

		/// <summary>
		/// Sends the given objects in the group to the targets position. Sends them to the
		/// differents positions around the given center.
		/// </summary>
		/// <param name="group">The group which is sended to the destiantion.</param>
		/// <param name="target">The object where the group goes.</param>
		private void GoToTargetGroup(GroupMovables group, IGameObject target) {
			List<Mogre.Vector3> destinations = PreparePositions(group.Count, target.Position);
			foreach (IMovableGameObject imgo in group) {
				imgo.GoToTarget(target);
				destinations.RemoveAt(0);
				moveMgrControledDict.Add(imgo, target);
			}
		}

		/// <summary>
		/// Sends the given object to the given position. 
		/// </summary>
		/// <param name="imgo">The IMovableGameObject which is sended to the destiantion.</param>
		/// <param name="target">The object where the group goes.</param>
		private void GoToTargetIMGO(IMovableGameObject imgo, IGameObject target) {
			imgo.GoToTarget(target);
			moveMgrControledDict.Add(imgo, target);
		}

		/// <summary>
		/// Creates dictionary with all controled movements and removes the movements with IFinishMovementReciever.
		/// </summary>
		/// <returns>Returns the movements without IFinishMovementReciever.</returns>
		public Dictionary<IMovableGameObject, IGameObject> GetAllMovements() {
			var dict = new Dictionary<IMovableGameObject, IGameObject>(moveMgrControledDict);

			foreach (var item in finishMoveRecDict) {
				dict.Remove(item.Key);
			}

			return dict;
		}
	}
}
