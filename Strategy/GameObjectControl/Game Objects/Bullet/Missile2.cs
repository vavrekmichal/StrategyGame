using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.FightMgr;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.Bullet {
	/// <summary>
	/// Implements IBullet interface. Represents a second type of IBullet which has power and attact distance 
	/// from runtim Property and a slower attack speed (a longer cooldown).
	/// </summary>
	public class Missile2 : IBullet {

		protected SceneNode sceneNode;
		protected Entity entity;
		protected Vector3 position;
		private string name;
		
		//The distance the object has left to travel
		private float distance = 0.0f;          
    	// The direction the object is moving
		private Vector3 direction = Vector3.ZERO;   
		private SolarSystem solarSystem;

		private const string mesh = "Bullet.mesh";
		private const float farfarAway = 5;
		private const float speed = 100;

		private IGameObject hittedObject;

		// General form of equation of a line
		private float a;
		private float b;
		private float c;
		private float destinationDevider;

		private const string missilePowerString = "missilePower2";
		private const string missileDistanceString = "missileDistance2";

		private static Property<int> missilePower = Game.PropertyManager.GetProperty<int>(missilePowerString);
		private static Property<int> missileDistance = Game.PropertyManager.GetProperty<int>(missileDistanceString);

		IBulletStopReciever reciever;

		/// <summary>
		/// Creates instance at given position and traveling to given target (position).
		/// </summary>
		/// <param name="position">The bullet's start position.</param>
		/// <param name="solSystem">The bullet's SolarSystem.</param>
		/// <param name="targetPosition">The taget's position.</param>
		/// <param name="rec">The bullet't hit reciever.</param>
		public Missile2(Vector3 position, SolarSystem solSystem, Vector3 targetPosition, IBulletStopReciever rec) {
			this.position = position;
			this.name = Game.IGameObjectCreator.GetUnusedName(typeof(Missile2).ToString());
			this.solarSystem = solSystem;
			this.reciever = rec;

			// Calucalates a distance and a direction.
			direction = targetPosition - position;
			distance = direction.Normalise();

			// Registers IBullet and SolarSystem set visibility.
			solarSystem.AddIBullet(this);

			// Caluculate constants for hidden collision.
			a = -(targetPosition.z - position.z);
			b = (targetPosition.x - position.x);
			c = -a * targetPosition.x - b * targetPosition.z;
			destinationDevider = (float)System.Math.Sqrt(a * a + b * b);
		}

		/// <summary>
		/// Returns bullet's power.
		/// </summary>
		public int Attack {
			get { return missilePower.Value; }
		}

		/// <summary>
		/// Returns bullet's name.
		/// </summary>
		public string Name {
			get { return name; }
		}

		/// <summary>
		/// Returns bullet's maximum attack distance.
		/// </summary>
		public static int AttackDistance {
			get { return missileDistance.Value; }
		}

		/// <summary>
		/// Returns bullet's cooldown.
		/// </summary>
		public static TimeSpan Cooldown {
			get { return TimeSpan.FromSeconds(5); }
		}

		/// <summary>
		/// Changes bullet's visibility. Visible is true -> Creates SceneNode and checks Entity (if is null -> initializes).
		/// Visible is false -> Destroys SceneNode and save actual position.
		/// </summary>
		public virtual void ChangeVisible(bool visible) {
			if (visible && sceneNode == null) {
				// Controls if the entity is inicialized
				if (entity == null) {
					entity = Game.SceneManager.CreateEntity(name, mesh);
				}

				sceneNode = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);

				Vector3 src = sceneNode.Orientation * Vector3.NEGATIVE_UNIT_Z;

				// SceneNode rotation
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

		/// <summary>
		/// Updates bullet's position and check collision in a visible mode.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			if (distance <= -100f) {
				// Missed target.
				Destroy();
			} else {
				if (!Collision()) {
					// Move
					var move = delay * speed;
					distance -= move;
					sceneNode.Translate(direction * move);
				} else {
					// Bullet hits
					reciever.BulletHit(this, hittedObject);
					Destroy();
				}
			}
		}

		/// <summary>
		/// Updates bullet's position and check collision in an invisible mode.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void HiddenUpdate(float delay) {
			if (distance <= -100f) {
				// Missed target.
				Destroy();
			} else {
				if (!HiddenCollision(delay)) {
					// Move
					var move = delay * speed;
					distance -= move;
					position = position + (direction * move);
				} else {
					// Bullet hits
					reciever.BulletHit(this, hittedObject);
					Destroy();
				}
			}
		

		/// <summary>
		/// Calculates collistion from Rays and check distance between bullet and object gets from the Ray result.
		/// </summary>
		/// <returns>Returns if bullet hits any target.</returns>
		private bool Collision() {
			Ray ray = new Ray(sceneNode.Position, GetDirection(sceneNode.Orientation));
			var mRaySceneQuery = Game.SceneManager.CreateRayQuery(ray);
			RaySceneQueryResult result = mRaySceneQuery.Execute();

			// Check all objects from the Ray result.
			foreach (var item in result) {
				if ((item.distance < farfarAway) && (item.distance > 0)) {
					// Meet something 
					var hitTest = Game.HitTest;
					if (hitTest.IsObjectControlable(item.movable.Name)) {
						// Can hit just controlable game objects (no bullets, no pointers...)
						hittedObject = hitTest.GetGameObject(item.movable.Name);
						return true;
					}

				}
			}
			return false;
		}

		/// <summary>
		/// Calculates collistion from the distance between a point and a line. Calculates the general form of equation of a line
		/// and calculates distance between the line and point (objects) in the SolarSystem. Uses delay to get a line form.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		/// <returns>Returns if bullet hits any target.</returns>
		private bool HiddenCollision(float delay) {

			// Gets all objects in SolarSystem (without IBullets)
			var dictMovable = solarSystem.GetIMGOs();
			var dictStatic = solarSystem.GetISGOs();

			// Create interval for hit

			var secondBound = position + (direction * speed * delay * farfarAway);

			// Gets min and max points in a space.
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

			// Checks all IMovableGameObjects in the SolarSystem.
			foreach (var item in dictMovable) {
				var positionVector2 = new Vector2(item.Value.Position.x, item.Value.Position.z);
				if (minBound < positionVector2 && positionVector2 < maxBound) {
					if (DistanceFromDiagonal(positionVector2) < farfarAway) {
						hittedObject = item.Value;
						return true;
					}
				}
			}

			// Checks all IStaticGameObject in the SolarSystem.
			foreach (var item in dictStatic) {
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
		/// Counts distance between point and diagonal line (line representing bullet and its direction).
		/// The distance is lenght of the ortogonal line from the point to the line.
		/// </summary>
		/// <param name="position">The checking point</param>
		/// <returns>Returns distance between point and bullet.</returns>
		private double DistanceFromDiagonal(Vector2 position) {
			return System.Math.Abs(a * position.x + b * position.y + c) / destinationDevider;

		}


		/// <summary>
		/// Transforms Quaternion to Vector3 and return the direction vector. 
		/// </summary>
		/// <param Name="q">The quaternion to transform.</param>
		/// <returns>Returns the vector3 with direction.</returns>
		private Vector3 GetDirection(Quaternion q) {
			Vector3 v;
			// Facing in -z
			v = q * Vector3.NEGATIVE_UNIT_Z;
			return v;
		}

		/// <summary>
		/// Removes the bullet from SolarSystem and destroys SceneNode and Entity.
		/// </summary>
		private void Destroy() {
			solarSystem.RemoveIBullet(this);
			if (sceneNode != null) {
				Game.SceneManager.DestroySceneNode(sceneNode);
				sceneNode = null;
			}
			Game.SceneManager.DestroyEntity(entity);
		}

		/// <summary>
		/// Destroy bullet's SceneNode and Entity.
		/// </summary>
		void IBullet.Destroy() {
			if (entity != null) {
				Game.SceneManager.DestroySceneNode(sceneNode);
				Game.SceneManager.DestroyEntity(entity);
				sceneNode = null;
				entity = null;
			}
		}
	}
}
