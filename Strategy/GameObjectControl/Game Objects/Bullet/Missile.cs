using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.FightMgr;

namespace Strategy.GameObjectControl.Game_Objects.Bullet {
	class Missile : IBullet {

		protected SceneNode sceneNode;
		protected Entity entity;
		protected Vector3 position;

		private const float farfarAway = 5;
		private const float speed = 100;
		private float distance = 0.0f;              //The distance the object has left to travel
		private Vector3 direction = Vector3.ZERO;   // The direction the object is moving
		private SolarSystem solarSystem;
		private const string mesh = "missile.mesh";
		private string name;

		private IGameObject hittedObject;

		// General form of equation of a line
		private float a;
		private float b;
		private float c;
		private float destinationDevider;

		IBulletStopReciever reciever;


		public Missile(Vector3 position, SolarSystem solSystem, Vector3 targetPosition, IBulletStopReciever rec) {
			this.position = position;
			this.name = GetUniqueName();
			this.solarSystem = solSystem;
			this.reciever = rec;

			solarSystem.AddIBullet(this);


			direction = targetPosition - position;
			distance = direction.Normalise();

			ChangeVisible(true);
			//Vector2 v = new Vector2(-(targetPosition.z - position.z), targetPosition.x - position.x); // Directional vector

			//var c = -(v.x*position.x - v.y*position.z);
			a = -(targetPosition.z - position.z);
			b = (targetPosition.x - position.x);
			c = -a * targetPosition.x - b * targetPosition.z;
			destinationDevider = (float)System.Math.Sqrt(a * a + b * b);

		}

		private static int uniquNameNumber;
		private static string GetUniqueName() {
			uniquNameNumber++;
			return typeof(Missile).ToString() + uniquNameNumber;
		}


		public int Attack {
			get { return 7; }
		}

		public string Name {
			get { return name; }
		}

		public static int AttackDistance {
			get { return 200; }
		}

		public static TimeSpan Cooldown {
			get { return TimeSpan.FromSeconds(2); }
		}

		/// <summary>
		/// Called when visibility is changed. Visible is true -> Function creates SceneNode and checks Entity (if is null -> initializes.
		/// Visible is false -> Destroy SceneNode and save actual position.
		/// </summary>
		public virtual void ChangeVisible(bool visible) {
			if (visible && sceneNode == null) {
				// Control if the entity is inicialized
				if (entity == null) {
					entity = Game.SceneManager.CreateEntity(name, mesh);
				}

				sceneNode = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);

				Vector3 src = sceneNode.Orientation * Vector3.NEGATIVE_UNIT_Z;


				if ((1.0f + src.DotProduct(direction)) < 0.0001f) {
					sceneNode.Yaw(new Angle(180.0f));
				} else {
					Quaternion quat = src.GetRotationTo(direction);
					sceneNode.Rotate(quat);
				}
			} else {
				if (sceneNode != null) {
					position = sceneNode.Position;
					Game.SceneManager.DestroySceneNode(sceneNode);
					sceneNode = null;
				}

			}
		}


		public void Update(float delay) {

			if (distance <= -100f) {
				// In destination
				reciever.BulletMiss(this);
				Destroy();
			} else {
				if (!Collision()) {
					// Movement code goes here
					var move = delay * speed;
					distance -= move;
					sceneNode.Translate(direction * move);
				} else {
					reciever.BulletHit(this, hittedObject);
					Destroy();
				}
			}
		}


		public void HiddenUpdate(float delay) {
			if (distance <= -100f) {
				// In destination
				reciever.BulletMiss(this);
				Destroy();
			} else {
				if (!HiddenCollision(delay)) {
					// Movement code goes here
					var move = delay * speed;
					distance -= move;
					position = position + (direction * move);
				} else {
					reciever.BulletHit(this, hittedObject);
					Destroy();
				}
			}
		}

		private bool Collision() {
			Ray ray = new Ray(sceneNode.Position, GetDirection(sceneNode.Orientation));
			var mRaySceneQuery = Game.SceneManager.CreateRayQuery(ray);
			RaySceneQueryResult result = mRaySceneQuery.Execute();


			foreach (var item in result) {
				if ((item.distance < farfarAway) && (item.distance > 0)) { // Meet something 
					var hitTest = Game.HitTest;
					if (hitTest.IsObjectControlable(item.movable.Name)) {
						hittedObject = hitTest.GetGameObject(item.movable.Name);
						return true;
					}

				}
			}
			return false;
		}

		private bool HiddenCollision(float delay) {

			// All objects in SolarSystem (without IBullets)
			var dict = solarSystem.GetIMGOs();
			var dict2 = solarSystem.GetISGOs();

			// Create interval for hit

			var secondBound = position + (direction * speed * delay * farfarAway);

			float minX;
			float maxX;
			if (secondBound.x > position.x) {
				minX = position.x;
				maxX = secondBound.x;
			} else {
				maxX = position.x;
				minX = secondBound.x;
			}
			float minZ;
			float maxZ;
			if (secondBound.z > position.z) {
				minZ = position.z;
				maxZ = secondBound.z;
			} else {
				maxZ = position.z;
				minZ = secondBound.z;
			}

			Vector2 minBound = new Vector2(minX, minZ);
			Vector2 maxBound = new Vector2(maxX, maxZ);
			foreach (var item in dict) {
				var positionVector2 = new Vector2(item.Value.Position.x, item.Value.Position.z);
				if (minBound < positionVector2 && positionVector2 < maxBound) {
					if (DistanceFromDiagonal(positionVector2) < farfarAway) {
						hittedObject = item.Value;
						return true;
					}
				}
			}
			foreach (var item in dict2) {
				var positionVector2 = new Vector2(item.Value.Position.x, item.Value.Position.z);
				if (minBound < positionVector2 && positionVector2 < maxBound) {
					if (DistanceFromDiagonal(positionVector2) < (farfarAway * farfarAway)) {
						hittedObject = item.Value;
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Function counts distance from a strai
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		private double DistanceFromDiagonal(Vector2 position) {
			return System.Math.Abs(a * position.x + b * position.y + c) / destinationDevider;

		}


		/// <summary>
		/// The getDirection() function transform Quaternion to Vector3 and return 
		/// Vector3 with direction from Quaternion
		/// </summary>
		/// <param Name="q">Quaternion to transform</param>
		/// <returns>Vector3 with direction</returns>
		private Vector3 GetDirection(Quaternion q) {
			Vector3 v; // Facing in -z
			v = q * Vector3.NEGATIVE_UNIT_Z;  // Transform the vector by the objects rotation.
			return v;
		}


		private void Destroy() {
			solarSystem.RemoveIBullet(this);
			if (sceneNode != null) {
				Game.SceneManager.DestroySceneNode(sceneNode);
				sceneNode = null;
			}
			Game.SceneManager.DestroyEntity(entity);
		}
	}
}
