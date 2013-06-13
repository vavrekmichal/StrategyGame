using System.Collections.Generic;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.Bullet;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;

namespace Strategy.FightMgr {

	/// <summary>
	/// Represents fight between two groups (attacking group and deffending groups (movable and static)).
	/// Class implements two interfaces therefore that it could get information about the object reached destination (move)
	/// or information about the bullet hit. Also contols fight - allows attack, gets target.
	/// </summary>
	public class Fight : IBulletStopReciever, IFinishMovementReciever {

		// Fighting groups
		private GroupMovables groupAttackers;
		private GroupMovables imgoDeffenders;
		private GroupStatics isgoDeffenders;

		private DamageCounter damageCounter;
		private Dictionary<IMovableGameObject, IGameObject> movingDict;

		private static Dictionary<IGameObject, bool> canAttack = new Dictionary<IGameObject, bool>();

		// Actual targets
		private Property<IGameObject> attackerTarget;
		private Property<IGameObject> deffenderTarget;

		// Represent squared maximum distance between fighting objects. When is distance bigger so object stop fighting.
		private const int squaredfarFarAway = 1000000;


		/// <summary>
		/// Freezes given object (the object cannot attack).
		/// </summary>
		/// <param name="gameObject">Disabled IGameObject</param>
		public static void DisableAttackGameObject(IGameObject gameObject) {
			if (canAttack.ContainsKey(gameObject)) {
				canAttack[gameObject] = false;
			} else {
				canAttack.Add(gameObject, false);
			}
		}

		/// <summary>
		/// Unfreezes given object (the object can attack).
		/// </summary>
		/// <param name="gameObject">Enabled IGameObject</param>
		public static void EnableAttackGameObject(IGameObject gameObject) {
			if (canAttack.ContainsKey(gameObject)) {
				canAttack[gameObject] = true;
			} else {
				canAttack.Add(gameObject, true);
			}
		}

		/// <summary>
		/// Creates instance of the Fight and collect all defenders on shout distance. Also selectes both (attacker
		/// and deffender) group and start fight.
		/// </summary>
		/// <param name="attackers">The attacking group (complete)</param>
		/// <param name="deffender">The deffending group (uncomplete - Shout)</param>
		public Fight(GroupMovables attackers, IGameObject deffender) {
			groupAttackers = attackers;
			movingDict = new Dictionary<IMovableGameObject, IGameObject>();
			damageCounter = new DamageCounter();

			var objectsInShoutDistance = new List<IGameObject>();
			objectsInShoutDistance.Add(deffender);
			deffender.Shout(objectsInShoutDistance);
			imgoDeffenders = Game.GroupManager.CreateSelectedGroupMovable(objectsInShoutDistance);
			isgoDeffenders = Game.GroupManager.CreateSelectedGroupStatic(objectsInShoutDistance);

			attackerTarget = new Property<IGameObject>(deffender);
			deffenderTarget = new Property<IGameObject>(attackers[0]);

			StartFight();
		}

		#region public functions

		/// <summary>
		/// Returns the object to be attacked. Object is representing as Property for better reactions
		/// to death object. If one of the sides has no member so the fight is ends.
		/// </summary>
		/// <param name="team">Used for find another oponent.</param>
		/// <returns>Returns next target of the asking team.</returns>
		public Property<IGameObject> GetTarget(TeamControl.Team team) {
			if (groupAttackers.Count == 0 || (imgoDeffenders.Count == 0 && isgoDeffenders.Count == 0)) {
				// Fight ends
				EndFight();
				return null;
			}
			if (team == groupAttackers.OwnerTeam) {
				// It is attacker
				return attackerTarget;
			} else {
				//It is deffender
				return deffenderTarget;
			}
		}

		/// <summary>
		/// Checks if gameObject can attack the target.  
		/// </summary>
		/// <param name="gameObject">The attacking object the target.</param>
		/// <param name="target">The target of the attack.</param>
		/// <param name="attackDistance">The maximum distance between the objects for attack.</param>
		/// <returns>Returns if attacker can attack the target.</returns>
		public bool TryAttack(IGameObject gameObject, IGameObject target, float attackDistance) {

			// Checks if object is freezed.
			if (CanAttack(gameObject)) {
				var xd = target.Position.x - gameObject.Position.x;
				var yd = target.Position.z - gameObject.Position.z;
				var squareDistance = xd * xd + yd * yd;

				if (squareDistance > attackDistance * attackDistance) {

					// Cannot attack because it is too far.
					var imgo = gameObject as IMovableGameObject;
					if (imgo != null) {
						gameObject.StopAttack();
						if (!movingDict.ContainsKey(imgo)) {
							movingDict.Add(imgo, target);
						}
						// Move attacker closer to target.
						Game.IMoveManager.GoToTarget(imgo, target, this);
					}
					return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Gets damage to hitted target from bullet.
		/// </summary>
		/// <param name="bullet">The bullet that hit the target.</param>
		/// <param name="hittedObject">The object that is hit.</param>
		public void BulletHit(IBullet bullet, IGameObject hittedObject) {
			hittedObject.TakeDamage(damageCounter.CountDamage(hittedObject, bullet));
		}

		/// <summary>
		/// Receives information that the object reached the destiantion.
		/// </summary>
		/// <param name="imgo">The object in destination.</param>
		public void MovementFinished(IMovableGameObject imgo) {
			movingDict.Remove(imgo);
			imgo.StartAttack(this);
		}

		/// <summary>
		/// Receives information that the object interupted movement.
		/// Does nothing with this object.
		/// </summary>
		/// <param name="imgo">The object interupted movement.</param>
		public void MovementInterupted(IMovableGameObject imgo) {}

		/// <summary>
		/// Checks if this fight contains given object.
		/// </summary>
		/// <param name="igo">The object which is controlled.</param>
		/// <returns>Returns if the fight contains given object.</returns>
		public bool Contains(IGameObject igo) {
			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				if (imgoDeffenders.HasMember(imgo)) {
					return true;
				}
				if (groupAttackers.HasMember(imgo)) {
					return true;
				}
			} else {
				if (isgoDeffenders.HasMember((IStaticGameObject)igo)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks if the group already striking at the given target.
		/// </summary>
		/// <param name="group">The checking group.</param>
		/// <param name="target">The attacking target</param>
		/// <returns>Returns if group attacking the target or not.</returns>
		public bool ContainsAttackingGroup(GroupMovables group, IGameObject target) {
			if (group == groupAttackers) {
				var imgo = target as IMovableGameObject;
				if (imgo != null && imgoDeffenders.HasMember(imgo)) {
					return true;
				} else {
					if (isgoDeffenders.HasMember(target as IStaticGameObject)) {
						return true;
					}
				}
			} else {
				if (group == imgoDeffenders) {
					var imgo = target as IMovableGameObject;
					if (imgo != null) {
						if (groupAttackers.HasMember(imgo)) {
							return true;
						}
					}
				}

			}
			return false;
		}

		/// <summary>
		/// Checks distance between object and target. Compares the distance with
		/// maximum distance constant and return if the target is in range.
		/// </summary>
		/// <returns>Return if the target is in range distance.</returns>
		public bool CheckDistance() {
			float xd = 0;
			float yd = 0;
			float squareDistance = 0;

			foreach (var item in movingDict) {
				xd = item.Value.Position.x - item.Key.Position.x;
				yd = item.Value.Position.z - item.Key.Position.z;
				squareDistance = xd * xd + yd * yd;

				// Uses squared value.
				if (squareDistance > squaredfarFarAway) {
					item.Key.Stop();
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Checks if the adding group is atacker or deffender and inserts it to 
		/// the appropriate group and recalculates it. 
		/// </summary>
		/// <param name="group">The inserting group.</param>
		public void AddGroup(GroupMovables group) {
			GroupManager groupMgr = Game.GroupManager;
			if (group.OwnerTeam == groupAttackers.OwnerTeam) {

				// Group is attacker
				int groupCount = group.Count;
				for (int i = 0; i < groupCount; i++) {
					IMovableGameObject temp = group[0];
					groupMgr.RemoveFromGroup(temp);
					groupMgr.AddToGroup(groupAttackers, temp);
					temp.StartAttack(this);
				}
				groupAttackers.Reselect();
				groupMgr.SelectGroup(groupAttackers);
			} else {

				// Group is deffender
				int groupCount = group.Count;
				for (int i = 0; i < groupCount; i++) {
					IMovableGameObject temp = group[0];
					groupMgr.RemoveFromGroup(temp);
					groupMgr.AddToGroup(imgoDeffenders, temp);
					temp.StartAttack(this);
				}
				groupAttackers.Reselect();
				groupMgr.SelectGroup(imgoDeffenders);
			}
		}

		#endregion

		/// <summary>
		/// Checks if object is freezed.
		/// </summary>
		/// <param name="gameObject">The checking object</param>
		/// <returns>Returns if freeze dictionary constains (is freezed) or not given object.</returns>
		protected bool CanAttack(IGameObject gameObject) {
			if (canAttack.ContainsKey(gameObject)) {
				return canAttack[gameObject];
			}
			return true;
		}

		/// <summary>
		/// Starts attack with both sides. Also sets DieHandler to reached a message about
		/// objects death.
		/// </summary>
		private void StartFight() {
			foreach (IMovableGameObject imgoAtt in groupAttackers) {
				imgoAtt.StartAttack(this);
				imgoAtt.DieHandler += OnAttackerDieEvent;
			}
			foreach (IMovableGameObject imgoDeff in imgoDeffenders) {
				imgoDeff.StartAttack(this);
				imgoDeff.DieHandler += OnDeffenderDieEvent;
			}
			foreach (IStaticGameObject isgoDeff in isgoDeffenders) {
				isgoDeff.StartAttack(this);
				isgoDeff.DieHandler += OnDeffenderDieEvent;
			}
		}

		/// <summary>
		/// Receives information about objects death and remove it from attacker group.
		/// </summary>
		/// <param name="igo">The death object.</param>
		/// <param name="m">The death argument.</param>
		private void OnAttackerDieEvent(IGameObject igo, MyDieArgs m) {
			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				RemoveFromDeffenderTarget(imgo);
			}
		}

		/// <summary>
		/// Receives information about objects death and remove it from deffender group.
		/// </summary>
		/// <param name="igo">The death object.</param>
		/// <param name="m">The death argument.</param>
		private void OnDeffenderDieEvent(IGameObject igo, MyDieArgs m) {
			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				// IMovableGameObject
				RemoveFromAttackerTarget(imgo);
			} else {
				var isgo = igo as IStaticGameObject;
				if (isgo != null) {
					// IStaticGameObject
					RemoveFromAttackerTarget(isgo);
				}
			}
		}

		/// <summary>
		/// Selects target to attackers and checks if any deffender is alive (ends fight).
		/// </summary>
		private void SelectAttackerTarget() {
			if (imgoDeffenders.Count == 0) {
				if (isgoDeffenders.Count != 0) {
					attackerTarget.Value = isgoDeffenders[0];
				} else {
					EndFight();
				}
			} else {
				attackerTarget.Value = imgoDeffenders[0];
			}
		}

		/// <summary>
		/// Removes given object from attackes targets. If the removing object is the actual
		/// target then selects new one.
		/// </summary>
		/// <param name="igo">The removing target.</param>
		private void RemoveFromAttackerTarget(IMovableGameObject igo) {
			imgoDeffenders.RemoveMember(igo);
			if (attackerTarget.Value == igo) {
				SelectAttackerTarget();
			}
		}

		/// <summary>
		/// Removes given object from attackes targets. If the removing object is the actual
		/// target then selects new one.
		/// </summary>
		/// <param name="igo">The removing target.</param>
		private void RemoveFromAttackerTarget(IStaticGameObject igo) {
			isgoDeffenders.RemoveMember(igo);
			if (attackerTarget.Value == igo) {
				SelectAttackerTarget();
			}
		}

		/// <summary>
		/// Removes given object from deffenders targets. If the removing object is the actual
		/// target then selects new one. Also controls if exist any next attacker (if not fight ends).
		/// </summary>
		/// <param name="igo">The removing target.</param>
		private void RemoveFromDeffenderTarget(IMovableGameObject igo) {
			groupAttackers.RemoveMember(igo);
			if (deffenderTarget.Value == igo) {
				if (groupAttackers.Count != 0) {
					deffenderTarget.Value = groupAttackers[0];
				} else {
					EndFight();
				}
			}
		}

		/// <summary>
		/// Ends the fight for each participating object.
		/// </summary>
		private void EndFight() {
			foreach (IMovableGameObject imgoAtt in groupAttackers) {
				imgoAtt.StopAttack();
				imgoAtt.DieHandler -= OnAttackerDieEvent;
			}
			foreach (IMovableGameObject imgoDeff in imgoDeffenders) {
				imgoDeff.StopAttack();
				imgoDeff.DieHandler -= OnDeffenderDieEvent;
			}
			foreach (IStaticGameObject isgoDeff in isgoDeffenders) {
				isgoDeff.StopAttack();
				isgoDeff.DieHandler -= OnDeffenderDieEvent;
			}
		}
	}

}
