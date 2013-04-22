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
	public abstract class MovableGameObject : IMovableGameObject {
		protected string name;
		protected Team movableObjectTeam;

		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected Mogre.Vector3 position;
		protected string team;
		protected Mogre.SceneManager manager;

		protected bool moving;
		protected Dictionary<PropertyEnum, object> propertyDict;
		protected Dictionary<PropertyEnum, object> propertyBonusDict;
		protected List<IGameAction> listOfAction; //TODO not implemented
		protected LinkedList<Vector3> flyList; // Walking points in linked list
		protected float distance = 0.0f;              // The distance the object has left to travel
		protected Vector3 direction = Vector3.ZERO;   // The direction the object is moving
		protected Vector3 destination = Vector3.ZERO; // The destination the object is moving towards

		protected IMoveManager interuptionReciever;	// Calls when object must go to another position (no to the target)
		protected bool isMovingToTarget = false;			// Target move indicator

		protected Vector3 modelDirection = Vector3.NEGATIVE_UNIT_Z;

		private bool isVisible;
		private int collisionCount = 0;
		private bool detourReached = false;
		private int colliisionConst = 100;



		public MovableGameObject() {
			isVisible = false;
			flyList = new LinkedList<Vector3>();
			listOfAction = new List<IGameAction>();
			propertyDict = new Dictionary<PropertyEnum, object>();
			propertyDict.Add(PropertyEnum.Hp, new Property<int>(100));
			propertyBonusDict = GroupMovables.baseTemplateBonusDict;
		}

		/// <summary>
		/// Calls when object is showed by SolarSystem
		/// </summary>
		protected abstract void onDisplayed();

		/// <summary>
		/// Checks if object go to target when is position changed
		/// </summary>
		protected void positionToMoveChanged() {
			if (isMovingToTarget) {
				interuptionReciever.interuptMove(this);
				isMovingToTarget = false;
			}
		}

		#region virtual methods


		public virtual ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			return ActionAnswer.Move;
		}

		/// <summary>
		/// Add new position to flyList (on first place)
		/// </summary>
		/// <param name="pointToGo">Position</param>
		public virtual void addNextLocation(Vector3 pointToGo) {
			flyList.AddFirst(pointToGo);
		}

		/// <summary>
		/// Add all positions from given LinkedList to flyList
		/// </summary>
		/// <param name="positionList">LinkedList with positions</param>
		public virtual void addNextLocation(LinkedList<Vector3> positionList) {
			positionToMoveChanged();
			foreach (var pointToGo in positionList) {
				flyList.AddLast(pointToGo);
			}
		}

		/// <summary>
		/// Creates new LinkedList and set one position to go.
		/// </summary>
		/// <param name="pointToGo">Position</param>
		public virtual void setNextLocation(Vector3 pointToGo) {
			positionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(pointToGo);
			moving = false;
		}

		/// <summary>
		/// Set given LinkedList as flyList
		/// </summary>
		/// <param name="positionList">LinkedList with positions</param>
		public virtual void setNextLocation(LinkedList<Vector3> positionList) {
			positionToMoveChanged();
			flyList = positionList;
			moving = false;
		}

		/// <summary>
		/// Gives MovableGameObject to given position
		/// </summary>
		/// <param name="pointToGo">Position</param>
		public virtual void jumpNextLocation(Vector3 pointToGo) {
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
		protected virtual bool nextLocation() {
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
		/// <param name="delay">Delay between frames</param>
		public virtual void move(float delay) {
			if (!moving) {
				if (nextLocation()) {
					moving = true;
					destination = flyList.First.Value; // Get the next destination.

					// Update the direction and the distance
					direction = destination - sceneNode.Position;
					distance = direction.Normalise();
					Vector3 src = getDirection(sceneNode.Orientation);
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
				if (!colision()) { // Object's not in colision

					float move = getProperty<float>(PropertyEnum.Speed).Value * delay;
					distance -= move;
					if (distance <= .0f) { // Reach destination
						sceneNode.Position = destination;
						detourReached = true;
						direction = Vector3.ZERO;
						moving = false;
						flyList.RemoveFirst(); // Remove that node from the front of the list
					} else {
						sceneNode.Translate(direction * move);
						Vector3 src = getDirection(sceneNode.Orientation);
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
					if (collisionCount>colliisionConst) {
						stop();
					}
					moving = false;
					if (collisionCount > 0 && !detourReached) {
						flyList.RemoveFirst();
					}
					var r = new Random().Next(2);
					if (r==0) {
						r = -1;
					}
					collisionCount++;
					Vector3 orto = new Vector3(direction.z*r, 0, -direction.x*r) * collisionCount*3 + sceneNode.Position;
					orto.y = 0;
					detourReached = false;
					addNextLocation(orto);

				}
			}
		}

		/// <summary>
		/// Function moves with SceneNode. It is called in non-active mode it means MovableGameObject
		/// is in hidden SolarSystem (SceneNode is null). Function controls distance from destination,
		/// collisions, etc.
		/// </summary>
		/// <param name="delay">Delay between last two frames</param>
		public virtual void nonActiveMove(float delay) {
			if (!moving) {
				if (nextLocation()) {
					moving = true;
					// Update the destination using the walklist.
					destination = flyList.First.Value; //get the next destination.
					// Update the direction and the distance
					direction = destination - position;
					distance = direction.Normalise();
				} else { // Nothing to do so stay in position

				}
			} else {
				float move = getProperty<float>(PropertyEnum.Speed).Value * delay;
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
		public bool colision() {
			Ray ray = new Ray(sceneNode.Position, getDirection(sceneNode.Orientation));
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


		public virtual void shout() {

		}

		#endregion
		/// <summary>
		/// Called when object will be invisible
		/// </summary>
		public virtual void changeVisible(bool visible) { 
			if (visible&& !this.isVisible) {

				// Control if the entity is inicialized
				if (entity == null) {
					entity = manager.CreateEntity(name, mesh);
				}

				sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);
				this.isVisible = true;
			} else {
				if (this.isVisible) {
				position = sceneNode.Position;
				manager.DestroySceneNode(sceneNode);
				sceneNode = null;
				this.isVisible = false;
				}
			}
		}

		public virtual void setGroupBonuses(Dictionary<PropertyEnum, object> bonusDict) {
			propertyBonusDict = bonusDict;
		}


		public virtual Dictionary<PropertyEnum, object> onGroupAdd() {
			// Empty dictionary = no bonuses for other members of group
			return new Dictionary<PropertyEnum, object>();
		}


		public Dictionary<PropertyEnum, object> getPropertyToDisplay() {
			return propertyDict;
		}

		/// <summary>
		/// The getDirection() function transform Quaternion to Vector3 and return 
		/// Vector3 with direction from Quaternion
		/// </summary>
		/// <param name="q">Quaternion to transform</param>
		/// <returns>Vector3 with direction</returns>
		private Vector3 getDirection(Quaternion q) {
			Vector3 v; // Facing in +z
			v = q * modelDirection;  // Transform the vector by the objects rotation.
			return v;
		}

		public Property<T> getProperty<T>(PropertyEnum propertyName) {
			if (!propertyDict.ContainsKey(propertyName)) {
				throw new PropertyMissingException("Object "+ Name+" doesn't have property "+ propertyName+".");
			}
			var prop = (Property<T>)propertyDict[propertyName];
			return prop;
		}

		protected void setProperty(PropertyEnum name, object prop) {
			if (!(prop.GetType().GetGenericTypeDefinition()==typeof(Property<>))) {
				throw new ArgumentException("Given object is not Property<T>, it is " + prop.GetType());
			}
			if (propertyDict.ContainsKey(name)) {
				propertyDict[name]= prop;
			} else {
				propertyDict.Add(name, prop);
			}
		}

		protected T getPropertyValueFromBonusDict<T>(PropertyEnum name) {
			if (!propertyBonusDict.ContainsKey(name)) {
				return default(T);
			}
			var prop = ((Property<T>)propertyBonusDict[name]).Value;
			return prop;
		}


		/// <summary>
		/// Sets flyList and move interupt reciever.
		/// </summary>
		/// <param name="positionList">New LinkedList with positions</param>
		/// <param name="moveCntr">Move interupt reciever</param>
		public void goToTarget(LinkedList<Vector3> positionList, IMoveManager moveCntr) {
			positionToMoveChanged();
			flyList = positionList;
			moving = false;
			isMovingToTarget = true;
			interuptionReciever = moveCntr;
		}

		/// <summary>
		/// Sets new LinkedList with one new position to go. And move interupt reciever is setted.
		/// </summary>
		/// <param name="placeToGo">Position</param>
		/// <param name="moveCntr">Move interupt reciever</param>
		public void goToTarget(Vector3 placeToGo, IMoveManager moveCntr) {
			positionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(placeToGo);
			moving = false;
			isMovingToTarget = true;
			interuptionReciever = moveCntr;
		}

		/// <summary>
		/// Stops object's moving
		/// </summary>
		public void stop() {
			positionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			moving = false;
		}


		public virtual int AttackPower {
			get { return 0; }
		}

		public virtual int DeffPower {
			get { return 0; }
		}


		public bool Visible {
			get { return isVisible; }
		}


		public string Name {
			get {
				return name;
			}
		}


		public Team Team {
			get {
				return movableObjectTeam;
			}
			set {
				movableObjectTeam = value;
			}
		}


		public Vector3 Position {
			get {
				if (sceneNode == null) {
					return position; 
				} else { 
					return sceneNode.Position; 
				}
			}
		}

		public Vector3 Direction {
			get { return direction; }
		}


		public int Hp {
			get { return ((Property<int>)propertyDict[PropertyEnum.Hp]).Value; }
		}


	}
}
