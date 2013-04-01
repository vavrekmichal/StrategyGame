using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GroupControl.Game_Objects;
using Strategy.GroupControl.Game_Objects.GameActions;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
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
		protected Property<float> flySpeed;  // The speed at which the object is moving
		protected List<IGameAction> listOfAction; //TODO not implemented
		protected LinkedList<Vector3> flyList; //walking points in linked list
		protected float distance = 0.0f;              //The distance the object has left to travel
		protected Vector3 direction = Vector3.ZERO;   // The direction the object is moving
		protected Vector3 destination = Vector3.ZERO; // The destination the object is moving towards

		protected Vector3 modelDirection = Vector3.NEGATIVE_UNIT_Z;

		public MovableGameObject() {
			flyList = new LinkedList<Vector3>();
			listOfAction = new List<IGameAction>();
		}

		protected abstract void onDisplayed();

		#region virtual methods

		public virtual ActionAnswer onMouseAction(ActionReason reason, Vector3 point, object hitTestResult) {
			return ActionAnswer.Move;
		}

		public virtual void addNextLocation(Vector3 pointToGo) {
			flyList.AddLast(pointToGo);
		}

		public virtual void addNextLocation(LinkedList<Vector3> positionList) {
			foreach (var pointToGo in positionList) {
				flyList.AddLast(pointToGo);
			}
		}

		public virtual void setNextLocation(Vector3 pointToGo) {
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(pointToGo);
			moving = false;
		}

		public virtual void setNextLocation(LinkedList<Vector3> positionList) {
			flyList = positionList;
			moving = false;
		}

		public virtual void jumpNextLocation(Vector3 pointToGo) {
			if (sceneNode == null) {
				position = pointToGo;
			} else {
				sceneNode.Position=pointToGo;
			}
		}

		protected virtual bool nextLocation() {
			if (flyList.Count == 0) {
				return false;
			} else {
				return true;
			}
		}

		public virtual void move(float f) {
			if (!moving) {
				if (nextLocation()) {
					moving = true;

					//Update the destination using the walklist.
					destination = flyList.First.Value; //get the next destination.
					//update the direction and the distance
					direction = destination - sceneNode.Position;
					distance = direction.Normalise();
					Vector3 src = getDirection(sceneNode.Orientation);
					if ((1.0f + src.DotProduct(direction)) < 0.0001f) {
						sceneNode.Yaw(180.0f);
					} else {
						direction.y = 0; //rotation fix
						Quaternion quat = src.GetRotationTo(direction);
						sceneNode.Rotate(quat);
					}
				} else { //nothing to do so stay in position

				}
			} else {
				if (!colision()) { //Protector's not in colision
					float move = flySpeed.Value * f;
					distance -= move;
					if (distance <= .0f) { //reach destination
						sceneNode.Position = destination;
						direction = Vector3.ZERO;
						moving = false;
						flyList.RemoveFirst(); //remove that node from the front of the list
					} else {
						sceneNode.Translate(direction * move);
						Vector3 src = getDirection(sceneNode.Orientation);
						if ((1.0f + src.DotProduct(direction)) < 0.0001f) { //watch to right direction
							sceneNode.Yaw(180.0f);
						} else {
							Quaternion quat = src.GetRotationTo(direction);
							sceneNode.Rotate(quat);
						}
					}
				} else {
					moving = false;

				}
			}
		}

		public virtual void nonActiveMove(float f) {
			if (!moving) {
				if (nextLocation()) {
					moving = true;
					//Update the destination using the walklist.
					destination = flyList.First.Value; //get the next destination.
					//update the direction and the distance
					direction = destination - position;
					distance = direction.Normalise();
				} else { //nothing to do so stay in position

				}
			} else {
				float move = flySpeed.Value * f;
				distance -= move;
				if (distance <= .0f) { //reach destination
					position = destination;
					direction = Vector3.ZERO;
					moving = false;
					flyList.RemoveFirst(); //remove that node from the front of the list
				} else {
					position = position + (direction * move);

				}
			}
		}

		/// <summary>
		/// The colision() control if object can move forward 
		/// </summary>
		/// <returns>true -> Protector cannot move forward / false -> Protector can</returns>
		public bool colision() {
			Ray ray = new Ray(sceneNode.Position, getDirection(sceneNode.Orientation));
			var mRaySceneQuery = manager.CreateRayQuery(ray);
			RaySceneQueryResult result = mRaySceneQuery.Execute();
			const float farfarAway = 30;

			foreach (var item in result) {
				if ((item.distance < farfarAway) && (item.distance > 0)) { //meet something 
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
		public virtual void changeVisible(bool visible) {   //now creating
			if (visible) {

				if (entity == null) { //control if the entity is inicialized
					entity = manager.CreateEntity(name, mesh);
				}

				sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);
			} else {
				position = sceneNode.Position;
				manager.DestroySceneNode(sceneNode);
			}
		}


		/// <summary>
		/// The getDirection() function transform Quaternion to Vector3 and return 
		/// Vector3 with direction from Quaternion
		/// </summary>
		/// <param name="q">Quaternion to transform</param>
		/// <returns>Vector3 with direction</returns>
		private Vector3 getDirection(Quaternion q) {
			Vector3 v; //facing in +z
			v = q * modelDirection;  //transform the vector by the objects rotation.
			return v;
		}


		public string Name {
			get {
				return name;
			}
			set {
				name = value;
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
			get { if (sceneNode == null) { return position; } else { return sceneNode.Position; }
			}
		}

		public Vector3 Direction {
			get { return direction; }
		}


		
	}
}
