using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GroupControl.Game_Objects;
using Strategy.GroupControl.Game_Objects.GameActions;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	abstract class MovableGameObject : IMovableGameObject{
        protected string name;
		protected Team movableObjectTeam;

        protected Mogre.Entity entity;
        protected Mogre.SceneNode sceneNode;
        protected string mesh;
        protected Mogre.Vector3 position;
        protected string team;
        protected Mogre.SceneManager manager;

		protected bool moving;
		protected float flySpeed = 80.0f;  // The speed at which the object is moving
		protected List<IGameAction> listOfAction; //TODO not implemented
		protected LinkedList<Vector3> flyList; //walking points in linked list
		protected float distance = 0.0f;              //The distance the object has left to travel
		protected Vector3 direction = Vector3.ZERO;   // The direction the object is moving
		protected Vector3 destination = Vector3.ZERO; // The destination the object is moving towards

		public MovableGameObject() {
			flyList = new LinkedList<Vector3>();
			listOfAction = new List<IGameAction>();
		}



		#region virtual methods

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
					LinkedListNode<Vector3> tmp;
					moving = true;

					//Update the destination using the walklist.
					destination = flyList.First.Value; //get the next destination.
					tmp = flyList.First; //save the node that held it
					flyList.RemoveFirst(); //remove that node from the front of the list
					flyList.AddLast(tmp);  //add it to the back of the list.
					//update the direction and the distance
					direction = destination - sceneNode.Position;
					distance = direction.Normalise();
					Vector3 src = getDirection(sceneNode.Orientation);
					if ((1.0f + src.DotProduct(direction)) < 0.0001f) {
						sceneNode.Yaw(180.0f);
					} else {
						Quaternion quat = src.GetRotationTo(direction);
						sceneNode.Rotate(quat);
					}
				} else { //nothing to do so stay in position
					
				}
			} else { //Protector's in motion

				if (!colision()) { //Protector's not in colision
					float move = flySpeed * f;
					distance -= move;
					if (distance <= .0f) { //reach destination
						sceneNode.Position = destination;
						direction = Vector3.ZERO;
						moving = false;
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

		}

		/// <summary>
		/// The colision() control of Protector can move forward of if
		/// it catch player (Ninja)
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

                sceneNode.Pitch(new Mogre.Degree(-90f));
                sceneNode.AttachObject(entity);
            } else {
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
			var v = new Vector3(1, 0, 0); //facing in +z
			v = q * v;  //transform the vector by the objects rotation.
			return v;
		}


		public string Name {
			get {
				return name;
			}
			set {
				name = Name;
			}
		}


		public Team Team {
			get {
				return movableObjectTeam;
			}
			set {
				movableObjectTeam = Team;
			}
		}
	}
}
