using System;
using System.Collections.Generic;
using Mogre;
using Strategy.Exceptions;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	public abstract class MovableGameObject : GameObject, IMovableGameObject {



		protected bool moving;

		protected Dictionary<string, object> propertyBonusDict;
		protected List<IGameAction> listOfAction; //TODO not implemented
		protected LinkedList<Vector3> flyList; // Walking points in linked list
		protected float distance = 0.0f;              // The distance the object has left to Travel
		protected Vector3 direction = Vector3.ZERO;   // The direction the object is moving
		protected Vector3 destination = Vector3.ZERO; // The destination the object is moving towards

		protected bool isMovingToTarget = false;			// Target move indicator

		protected Vector3 modelDirection = Vector3.NEGATIVE_UNIT_Z;


		private int collisionCount = 0;
		private bool detourReached = false;
		private int colliisionConst = 100;



		public MovableGameObject() {
			isVisible = false;
			flyList = new LinkedList<Vector3>();
			listOfAction = new List<IGameAction>();
			propertyDict = new Dictionary<PropertyEnum, object>();
			propertyDictUserDefined = new Dictionary<string, object>();
			propertyDict.Add(PropertyEnum.Hp, new Property<int>(100));
			propertyBonusDict = new Dictionary<string, object>();
		}

		

		/// <summary>
		/// Checks if object go to target when is position changed
		/// </summary>
		protected void PositionToMoveChanged() {
			if (isMovingToTarget) {
				Game.IMoveManager.MovementInterupted(this);
				isMovingToTarget = false;
			}
		}

		#region virtual methods


		public virtual ActionAnswer OnMouseAction(Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			return ActionAnswer.Move;
		}

		/// <summary>
		/// Add new position to flyList (on first place)
		/// </summary>
		/// <param Name="pointToGo">Position</param>
		public virtual void AddNextLocation(Vector3 pointToGo) {
			flyList.AddFirst(pointToGo);
		}

		/// <summary>
		/// Add all positions from given LinkedList to flyList
		/// </summary>
		/// <param Name="positionList">LinkedList with positions</param>
		public virtual void AddNextLocation(LinkedList<Vector3> positionList) {
			PositionToMoveChanged();
			foreach (var pointToGo in positionList) {
				flyList.AddLast(pointToGo);
			}
		}

		/// <summary>
		/// Creates new LinkedList and Set one position to go.
		/// </summary>
		/// <param Name="pointToGo">Position</param>
		public virtual void SetNextLocation(Vector3 pointToGo) {
			PositionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(pointToGo);
			moving = false;
		}

		/// <summary>
		/// Set given LinkedList as flyList
		/// </summary>
		/// <param Name="positionList">LinkedList with positions</param>
		public virtual void SetNextLocation(LinkedList<Vector3> positionList) {
			PositionToMoveChanged();
			flyList = positionList;
			moving = false;
		}

		/// <summary>
		/// Gives MovableGameObject to given position
		/// </summary>
		/// <param Name="pointToGo">Position</param>
		public virtual void JumpNextLocation(Vector3 pointToGo) {
			if (sceneNode == null) {
				position = pointToGo;
			} else {
				sceneNode.Position = pointToGo;
			}
		}

		/// <summary>
		/// Function check if exist next position to go.
		/// </summary>
		/// <returns>Exist = true</returns>
		protected virtual bool NextLocation() {
			if (flyList.Count == 0) {
				collisionCount = 0;
				return false;
			} else {
				return true;
			}
		}

		/// <summary>
		/// Function moves with SceneNode. It is called in active mode it means MovableGameObject
		/// is in acitve SolarSystem (SceneNode is setted). Function controls distance from destination,
		/// collisions, etc.
		/// </summary>
		/// <param Name="delay">Delay between frames</param>
		public virtual void Move(float delay) {
			if (!moving) {
				if (NextLocation()) {

					moving = true;
					destination = flyList.First.Value; // Get the next destination.

					// Update the direction and the distance
					direction = destination - sceneNode.Position;
					distance = direction.Normalise();
					Vector3 src = GetDirection(sceneNode.Orientation);
					if ((1.0f + src.DotProduct(direction)) < 0.0001f) {
						sceneNode.Yaw(180.0f);
					} else {
						direction.y = 0; // Rotation fix
						Quaternion quat = src.GetRotationTo(direction);
						sceneNode.Rotate(quat);
					}
				} else { // Nothing to do so stay in position

				}
			} else {
				if (!Colision()) { // Object's not in colision
					float move = GetPropertyValue<float>(PropertyEnum.Speed) * delay;
					distance -= move;
					if (distance <= .0f) { // Reach destination
						sceneNode.Position = destination;
						detourReached = true;
						direction = Vector3.ZERO;
						moving = false;
						flyList.RemoveFirst(); // Remove that node from the front of the list
					} else {
						sceneNode.Translate(direction * move);
						Vector3 src = GetDirection(sceneNode.Orientation);
						if ((1.0f + src.DotProduct(direction)) < 0.0001f) { // Watch to right direction
							sceneNode.Yaw(180.0f);
						} else {
							direction.y = 0; // Rotation fix
							Quaternion quat = src.GetRotationTo(direction);
							sceneNode.Rotate(quat);
						}
					}
				} else {
					// Collision solver
					if (collisionCount > colliisionConst) {
						Stop();
					}
					moving = false;
					if (collisionCount > 0 && !detourReached) {
						flyList.RemoveFirst();
					}
					var r = new Random().Next(2);
					if (r == 0) {
						r = -1;
					}
					collisionCount++;
					Vector3 orto = new Vector3(direction.z * r, 0, -direction.x * r) * collisionCount * 3 + sceneNode.Position;
					orto.y = 0;
					detourReached = false;
					AddNextLocation(orto);

				}
			}
		}

		/// <summary>
		/// Function moves with SceneNode. It is called in non-active mode it means MovableGameObject
		/// is in hidden SolarSystem (SceneNode is null). Function controls distance from destination,
		/// collisions, etc.
		/// </summary>
		/// <param Name="delay">Delay between last two frames</param>
		public virtual void NonActiveMove(float delay) {
			if (!moving) {
				if (NextLocation()) {
					moving = true;
					// Update the destination using the walklist.
					destination = flyList.First.Value; //get the next destination.
					// Update the direction and the distance
					direction = destination - position;
					distance = direction.Normalise();
				} else { // Nothing to do so stay in position

				}
			} else {
				float move = GetPropertyValue<float>(PropertyEnum.Speed) * delay;
				distance -= move;
				if (distance <= .0f) { // Reach destination
					position = destination;
					direction = Vector3.ZERO;
					moving = false;
					flyList.RemoveFirst(); // Remove that node from the front of the list
				} else {
					position = position + (direction * move);

				}
			}
		}

		/// <summary>
		/// The colision() control if object can move forward 
		/// </summary>
		/// <returns>True -> Protector cannot move forward / false -> Protector can</returns>
		public bool Colision() {
			Ray ray = new Ray(sceneNode.Position, GetDirection(sceneNode.Orientation));
			var mRaySceneQuery = manager.CreateRayQuery(ray);
			RaySceneQueryResult result = mRaySceneQuery.Execute();
			const float farfarAway = 30;

			foreach (var item in result) {
				if ((item.distance < farfarAway) && (item.distance > 0)) { // Meet something 
					return true;
				}
			}
			return false;
		}


		public virtual void Shout() {

		}

		#endregion
		

		public virtual void SetGroupBonuses(Dictionary<string, object> bonusDict) {
			propertyBonusDict = bonusDict;
		}


		public virtual Dictionary<string, object> OnGroupAdd() {
			// Empty dictionary = no bonuses for other members of group
			return new Dictionary<string, object>();
		}

		/// <summary>
		/// The getDirection() function transform Quaternion to Vector3 and return 
		/// Vector3 with direction from Quaternion
		/// </summary>
		/// <param Name="q">Quaternion to transform</param>
		/// <returns>Vector3 with direction</returns>
		private Vector3 GetDirection(Quaternion q) {
			Vector3 v; // Facing in +z
			v = q * modelDirection;  // Transform the vector by the objects rotation.
			return v;
		}



		protected T GetPropertyValue<T>(PropertyEnum name) {
			Property<T> bonus;
			if (!propertyBonusDict.ContainsKey(name.ToString())) {
				bonus = new Property<T>(default(T));
			} else {
				bonus = ((Property<T>)propertyBonusDict[name.ToString()]);
			}

			// Base Dictionary
			Property<T> property = GetProperty<T>(name);
			var op = Property<T>.Operator.Plus;
			return bonus.SimpleMath(op, property).Value;
		}

		protected T GetPropertyValue<T>(string name) {
			Property<T> bonus;
			if (!propertyBonusDict.ContainsKey(name)) {
				bonus = new Property<T>(default(T));
			} else {
				bonus = ((Property<T>)propertyBonusDict[name]);
			}

			// UserDefined Dictionary
			Property<T> property = GetProperty<T>(name);
			var op = Property<T>.Operator.Plus;
			return bonus.SimpleMath(op, property).Value;
		}


		/// <summary>
		/// Sets flyList and move interupt reciever.
		/// </summary>
		/// <param Name="positionList">New LinkedList with positions</param>
		/// <param Name="moveCntr">Move interupt reciever</param>
		public void GoToTarget(LinkedList<Vector3> positionList) {
			PositionToMoveChanged();
			flyList = positionList;
			moving = false;
			isMovingToTarget = true;
		}

		/// <summary>
		/// Sets new LinkedList with one new position to go. And move interupt reciever is setted.
		/// </summary>
		/// <param Name="placeToGo">Position</param>
		/// <param Name="moveCntr">Move interupt reciever</param>
		public void GoToTarget(Vector3 placeToGo) {
			PositionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(placeToGo);
			moving = false;
			isMovingToTarget = true;
		}

		/// <summary>
		/// Stops object's moving
		/// </summary>
		public void Stop() {
			PositionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			moving = false;
		}


		public virtual int AttackPower {
			get { return 0; }
		}

		public virtual int DeffPower {
			get { return 0; }
		}

		public Vector3 Direction {
			get { return direction; }
		}


		public override int Hp {
			get { return ((Property<int>)propertyDict[PropertyEnum.Hp]).Value; }
		}

		public override float PickUpDistance {
			get { return 80; }
		}

		public override float OccupyDistance {
			get { return 90; }
		}

		public override int OccupyTime {
			get { return 10; }
		}

		public override ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}
	}
}
