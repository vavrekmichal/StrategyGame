using System;
using System.Collections.Generic;
using Mogre;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	/// <summary>
	/// Implements all IMovableGameObject's functions. Designed to facilitate the implementation of game objects.
	/// </summary>
	public abstract class MovableGameObject : GameObject, IMovableGameObject {

		protected bool moving;

		protected Dictionary<string, object> propertyBonusDict;

		protected const int startHP = 100;

		/// <summary>
		/// Initializes objects Hp and sets the object to invisible (visible mode is setted by SolarSystem).
		/// </summary>
		public MovableGameObject() {	
			isVisible = false;
			flyList = new LinkedList<Vector3>();
			propertyDict.Add(PropertyEnum.Hp, new Property<int>(startHP));
			propertyBonusDict = new Dictionary<string, object>();
		}

		#region Moving
		// Flying points in linked list
		protected LinkedList<Vector3> flyList;

		// The distance that the object must travel to next destianation.
		protected float distance = 0.0f;

		// The direction in which the object is moving   
		protected Vector3 direction = Vector3.ZERO;

		// The destination the object is moving towards
		protected Vector3 destination = Vector3.ZERO; 

		// The follow indicator
		protected bool follow;
		protected IGameObject followTarget;

		// Move to a target indicator
		protected bool isMovingToTarget = false;			

		protected Vector3 modelDirection = Vector3.NEGATIVE_UNIT_Z;

		private int collisionCount = 0;
		private bool detourReached = false;
		private int colliisionConst = 100;


		/// <summary>
		/// Moves with the object. Is called in visible mode it means MovableGameObject
		/// is in acitve SolarSystem (SceneNode is setted). Controls distance from destination,
		/// collisions, and solves the collisions.
		/// </summary>
		/// <param Name="delay">The delay between last two frames (seconds).</param>
		public virtual void Move(float delay) {
			Update(delay);
			if (!moving) {
				if (NextLocation()) {

					moving = true;

					// Get the next destination.
					destination = flyList.First.Value; 

					// Update the direction and the distance
					direction = destination - sceneNode.Position;
					distance = direction.Normalise();
					Vector3 src = GetDirection(sceneNode.Orientation);
					if ((1.0f + src.DotProduct(direction)) < 0.0001f) {
						sceneNode.Yaw(180.0f);
					} else {
						direction.y = 0; // Inaccuracy of decimal places fix
						Quaternion quat = src.GetRotationTo(direction);
						sceneNode.Rotate(quat);
					}
				} else { // Nothing to do so stay in position

				}
			} else {
				if (!Collision()) { 
					// Object's not in colision
					float move = GetPropertyValue<float>(PropertyEnum.Speed) * delay;
					distance -= move;
					if (distance <= .0f) {
						// Reached the destination
						sceneNode.Position = destination;
						detourReached = true;
						direction = Vector3.ZERO;
						moving = false;
						// Remove that node from the front of the list
						flyList.RemoveFirst(); 
					} else {
						sceneNode.Translate(direction * move);
						Vector3 src = GetDirection(sceneNode.Orientation);
						if ((1.0f + src.DotProduct(direction)) < 0.0001f) {
							sceneNode.Yaw(180.0f);
						} else {
							direction.y = 0; // Inaccuracy of decimal places fix
							Quaternion quat = src.GetRotationTo(direction);
							sceneNode.Rotate(quat);
						}
						position.Value = sceneNode.Position;
					}
				} else {
					// Collision solver
					if (collisionCount > colliisionConst) {
						Stop();
					}
					moving = false;
					if (collisionCount > 0 && !detourReached) {
						if (flyList.Count == 0) {
							Stop();
						} else {
							flyList.RemoveFirst();
						}
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
		/// Moves with the object. Is called in invisible mode it means MovableGameObject
		/// is in hidden SolarSystem (SceneNode is null). Controls distance from destination,
		/// and collisions.
		/// </summary>
		/// <param Name="delay">The delay between last two frames (seconds).</param>
		public virtual void NonActiveMove(float delay) {
			Update(delay);
			if (!moving) {
				if (NextLocation()) {
					moving = true;
					// Update the destination using the flyList.
					destination = flyList.First.Value;
					// Update the direction and the distance
					direction = destination - position.Value;
					distance = direction.Normalise();
				} else { // Nothing to do so stay in position

				}
			} else {
				float move = GetPropertyValue<float>(PropertyEnum.Speed) * delay;
				distance -= move;
				if (distance <= .0f) { // Reached the destination
					position.Value = destination;
					direction = Vector3.ZERO;
					moving = false;
					flyList.RemoveFirst(); // Remove that node from the front of the list
				} else {
					position.Value = position.Value + (direction * move);

				}
			}
		}


		/// <summary>
		/// Adds new position to the flyList (on first place).
		/// </summary>
		/// <param Name="pointToGo">The new position to go.</param>
		public virtual void AddNextLocation(Vector3 pointToGo) {
			flyList.AddFirst(pointToGo);
		}

		/// <summary>
		/// Adds all positions from given LinkedList to flyList (before older).
		/// </summary>
		/// <param Name="positionList">The LinkedList with positions togo.</param>
		public virtual void AddNextLocation(LinkedList<Vector3> positionList) {
			PositionToMoveChanged();
			var list = new LinkedList<Vector3>(positionList);
			foreach (var pointToGo in flyList) {
				flyList.AddLast(pointToGo);
			}
			flyList = list;
		}

		/// <summary>
		/// Creates new LinkedList and sets given position to go.
		/// Old positions to go are canceled.
		/// </summary>
		/// <param Name="pointToGo">The position to go.</param>
		public virtual void SetNextLocation(Vector3 pointToGo) {
			PositionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(pointToGo);
			moving = false;
		}

		/// <summary>
		/// Sets given LinkedList as flyList. Old positions to go are canceled.
		/// </summary>
		/// <param Name="positionList">The LinkedList with positions to go.</param>
		public virtual void SetNextLocation(LinkedList<Vector3> positionList) {
			PositionToMoveChanged();
			flyList = positionList;
			moving = false;
		}

		/// <summary>
		/// Sets MovableGameObject to given position.
		/// </summary>
		/// <param Name="pointToGo">The new position.</param>
		public virtual void JumpToLocation(Vector3 pointToGo) {
			if (sceneNode == null) {
				// Invisible mode
				position.Value = pointToGo;
			} else {
				// Visible mode
				sceneNode.Position = pointToGo;
			}
		}

		/// <summary>
		/// Sets given LinkedList as flyList. Old positions to go are canceled.
		/// Sets the "go to target" indicator.
		/// </summary>
		/// <param Name="positionList">The LinkedList with positions to go.</param>
		public void GoToTarget(LinkedList<Vector3> positionList) {
			PositionToMoveChanged();
			flyList = positionList;
			moving = false;
			isMovingToTarget = true;
		}

		/// <summary>
		/// Creates new LinkedList and sets given position to go. Old positions to go are canceled.
		/// Sets the "go to target" indicator.
		/// </summary>
		/// <param Name="placeToGo">The position to go.</param>
		public void GoToTarget(Vector3 placeToGo) {
			PositionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(placeToGo);
			moving = false;
			isMovingToTarget = true;
		}

		/// <summary>
		/// Creates new LinkedList and sets given position to go. Old positions to go are canceled.
		/// Sets the "go to target" indicator and the "follow" indicator.
		/// </summary>
		/// <param Name="objectToGo">The following object.</param>
		public void GoToTarget(IGameObject objectToGo) {
			PositionToMoveChanged();
			flyList = new LinkedList<Vector3>();
			flyList.AddLast(objectToGo.Position);
			follow = true;
			followTarget = objectToGo;
			moving = false;
			isMovingToTarget = true;
		}


		/// <summary>
		/// Stops object's moving. Clears flyList. If the object was in "go to target" mode,
		/// that sends information to IMoveManager.
		/// </summary>
		public void Stop() {
			if (isMovingToTarget) {
				Game.IMoveManager.MovementInterupted(this);
				isMovingToTarget = false;
			}
			flyList = new LinkedList<Vector3>();
			moving = false;
		}

		/// <summary>
		/// Controls if object can move forward. Function cast the Ray in object's direction
		/// and controls distance between objects.
		/// </summary>
		/// <returns>Returns if object crashed.</returns>
		public bool Collision() {
			Ray ray = new Ray(sceneNode.Position, GetDirection(sceneNode.Orientation));
			var mRaySceneQuery = Game.SceneManager.CreateRayQuery(ray);
			RaySceneQueryResult result = mRaySceneQuery.Execute();
			const float farfarAway = 30;

			foreach (var item in result) {
				if ((item.distance < farfarAway) && (item.distance > 0)) { // Meet something 
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks if object go to the target when is a position to go changed.
		/// If object is in "go to target" mode that sends information to IMoveManager.
		/// </summary>
		protected void PositionToMoveChanged() {
			follow = false;
			if (attack) {
				attack = false;
				target = null;
			}
			if (isMovingToTarget) {
				Game.IMoveManager.MovementInterupted(this);
				isMovingToTarget = false;
			}
		}

		

		/// <summary>
		/// Check if exist next position to go.
		/// </summary>
		/// <returns>Returns if exists next position to go.</returns>
		protected virtual bool NextLocation() {
			if (flyList.Count == 0) {
				collisionCount = 0;
				return false;
			} else {
				return true;
			}
		}

		/// <summary>
		/// Updates object's direction and distance when it is in "follow" mode
		/// and calls TryAttack function (decrease the cooldown time and can attack a target).
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		protected override void Update(float delay) {
			base.Update(delay);
			TryAttack(delay);
			if (follow) {
				direction = followTarget.Position - position.Value;
				distance = direction.Normalise();
			}
		}

		/// <summary>
		/// Transforms Quaternion to Vector3 and returns a Vector3 contains the direction from the Quaternion.
		/// </summary>
		/// <param Name="q">The Quaternion to transform.</param>
		/// <returns>Returns the Vector3 contains the direction.</returns>
		private Vector3 GetDirection(Quaternion q) {
			Vector3 v; // Facing in +z
			v = q * modelDirection;  // Transform the vector by the objects rotation.
			return v;
		}
		#endregion

		#region Attack

		protected Property<IGameObject> target;
		protected Dictionary<Type, TimeSpan> coolDownList = new Dictionary<Type, TimeSpan>();

		/// <summary>
		/// Checks if the object has setted target (if not so gets next from Fight or stop attacking).
		/// Contols if the object can fire (doesn't have cooldown) and if it can shoot so it turns to the target and shoots.
		/// </summary>
		protected virtual void Attack() {
			if (target == null || target.Value == null || target.Value.Hp <= 0) {
				target = fight.GetTarget(team);
			}
			if (target == null) {
				StopAttack();
				return;
			}
			if (!coolDownList.ContainsKey(GetIBulletType())) {
				if (fight.TryAttack(this, target.Value, GetIBulletAttackDistance())) {
					var attackDirection = target.Value.Position - position.Value;

					if (sceneNode != null) {
						Vector3 src = GetDirection(sceneNode.Orientation);
						if ((1.0f + src.DotProduct(attackDirection)) < 0.0001f) {
							sceneNode.Yaw(180.0f);
						} else {
							direction.y = 0; // Rotation fix
							Quaternion quat = src.GetRotationTo(attackDirection);
							sceneNode.Rotate(quat);
						}
					}
					coolDownList.Add(GetIBulletType(), GetIBulletAttackCooldown());
					CreateIBullet();
				}
			}
		}

		/// <summary>
		/// Decreases cooldowns by delay and if the object is in "attack" mode, it calls Attack()
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		protected virtual void TryAttack(float delay) {
			if (coolDownList.Count > 0) {
				var delayTimeSpan = TimeSpan.FromSeconds(delay);
				var copy = new Dictionary<Type, TimeSpan>(coolDownList);
				foreach (var coolDown in copy) {
					var newCooldown = coolDown.Value - delayTimeSpan;
					if (newCooldown < TimeSpan.Zero) {
						coolDownList.Remove(coolDown.Key);
					} else {
						coolDownList[coolDown.Key] = newCooldown;
					}
				}
			}
			if (attack) {
				Attack();
			}
		}

		/// <summary>
		/// Return a type of a current acquired bullet.
		/// </summary>
		/// <returns>Returns a type of a current acquired bullet.</returns>
		protected virtual Type GetIBulletType() {
			return typeof(Strategy.GameObjectControl.Game_Objects.Bullet.Missile);
		}

		/// <summary>
		/// Return a maximum attack distance of a current acquired bullet.
		/// </summary>
		/// <returns>Returns a maximum attack distance of a current acquired bullet.</returns>
		protected virtual int GetIBulletAttackDistance() {
			return Missile.AttackDistance;
		}

		/// <summary>
		/// Return a cooldown of a current acquired bullet.
		/// </summary>
		/// <returns>Returns a cooldown of a current acquired bullet.</returns>
		protected virtual TimeSpan GetIBulletAttackCooldown() {
			return Missile.Cooldown;
		}

		/// <summary>
		/// Creates and registers a new bullet.
		/// </summary>
		/// <returns>Returns Missile instance.</returns>
		protected virtual IBullet CreateIBullet() {
			var solS = Game.GroupManager.GetSolarSystem(this);
			return new Missile(position.Value, solS, target.Value.Position, fight);
		}
		#endregion

		/// <summary>
		/// This prototype of movable object always returns Move (class which will inherits should override this method).
		/// </summary>
		/// <param name="point">The mouse position.</param>
		/// <param name="hitObject">The result of a HitTest.</param>
		/// <param name="isFriendly">The information if the hitted object is friendly.</param>
		/// <param name="isMovableGameObject">The information if the hitted object is movable.</param>
		/// <returns>Returns Move action.</returns>
		public virtual ActionAnswer OnMouseAction(Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject) {
			return ActionAnswer.Move;
		}

		/// <summary>
		/// Stores given group bonuses.
		/// </summary>
		/// <param name="bonusDict">The dictionary contains the group bonuses.</param>
		public virtual void SetGroupBonuses(Dictionary<string, object> bonusDict) {
			propertyBonusDict = bonusDict;
		}

		/// <summary>
		/// This prototype of movable object return empty dictionary (no bonuses for group members).
		/// </summary>
		/// <returns>Returns empty dictionary.</returns>
		public virtual Dictionary<string, object> OnGroupAdd() {
			return new Dictionary<string, object>();
		}
		
		/// <summary>
		/// Returns Property's value from propertyBonusDict summed with group bonus. If object does't contains queried property,
		/// returs new property with default value.
		/// </summary>
		/// <typeparam name="T">The type of queried property.</typeparam>
		/// <param name="name">The name of the property. (PropertyEnum)</param>
		/// <returns>Returns founded property or new property with default value.</returns>
		protected T GetPropertyValue<T>(PropertyEnum name) {
			Property<T> bonus;
			// Group bonus
			if (!propertyBonusDict.ContainsKey(name.ToString())) {
				bonus = new Property<T>(default(T));
			} else {
				bonus = ((Property<T>)propertyBonusDict[name.ToString()]);
			}

			// Object's property
			Property<T> property = GetProperty<T>(name);
			var op = Property<T>.Operator.Plus;
			return bonus.SimpleMath(op, property).Value;
		}

		/// <summary>
		/// Returns Property's value from propertyBonusDict summed with group bonus. If object does't contains queried property,
		/// returs new property with default value. (User defined property - string name).
		/// </summary>
		/// <typeparam name="T">The type of queried property.</typeparam>
		/// <param name="name">The name of the property. (string)</param>
		/// <returns>Returns founded property or new property with default value.</returns>
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
		/// Returns a current state of health.
		/// </summary>
		public override int Hp {
			get { return ((Property<int>)propertyDict[PropertyEnum.Hp]).Value; }
		}

		/// <summary>
		/// Sets the state of health.
		/// </summary>
		/// <param name="hp">The new state of health.</param>
		protected void setHp(int hp) {
			((Property<int>)propertyDict[PropertyEnum.Hp]).Value = hp;
		}

		/// <summary>
		/// Subtracts receives damage from current Hp. If the health is lower then 0 so RaiseDie is called.
		/// </summary>
		/// <param name="damage">The taken damage.</param>
		public override void TakeDamage(int damage) {
			var hpProp = (Property<int>)propertyDict[PropertyEnum.Hp];
			var actualHp = hpProp.Value - damage;
			hpProp.Value = actualHp;
			if (actualHp < 0) {
				RaiseDie();
			}
		}

		/// <summary>
		/// Returns the distance, which is necessary for meet with the object.
		/// </summary>
		public override float PickUpDistance {
			get { return 80; }
		}

		/// <summary>
		/// Returns the distance, which is necessary for a occupation.
		/// </summary>
		public override float OccupyDistance {
			get { return 90; }
		}

		/// <summary>
		/// Returns the time, which is necessary for a successful occupation.
		/// </summary>
		public override int OccupyTime {
			get { return 10; }
		}

		/// <summary>
		/// This prototype of movable object always returns None (doesn't react on ActionReason).
		/// </summary>
		/// <param name="reason">The reason of calling this function.</param>
		/// <param name="target">The target which invoke this calling.</param>
		/// <returns>Always returns None.</returns>
		public override ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target) {
			return ActionReaction.None;
		}
	}
}
