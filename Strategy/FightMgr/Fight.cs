using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.Bullet;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;

namespace Strategy.FightMgr {
	public class Fight : IBulletStopReciever, IFinishMovementReciever {
		private GroupMovables groupAttackers;
		private GroupMovables imgoDeffenders;
		private GroupStatics isgoDeffenders;
		private DamageCounter damageCounter;

		private Property<IGameObject> attackerTarget;
		private Property<IGameObject> deffenderTarget;

		private Dictionary<IMovableGameObject, IGameObject> movingDict;

		private const int farFarAway = 1000;

		private static Dictionary<IGameObject, bool> canAttack = new Dictionary<IGameObject, bool>();

		/// <summary>
		/// Given IGameObject will be freeze. It means the object cannot attack.
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
		/// Given IGameObject will be unfreeze. It means the object can attack.
		/// </summary>
		/// <param name="gameObject">Enabled IGameObject</param>
		public static void EnableAttackGameObject(IGameObject gameObject) {
			if (canAttack.ContainsKey(gameObject)) {
				canAttack[gameObject] = true;
			} else {
				canAttack.Add(gameObject, true);
			}
		}

		public Fight(GroupMovables attackers, IGameObject deffender) {
			groupAttackers = attackers;
			movingDict = new Dictionary<IMovableGameObject, IGameObject>();
			damageCounter = new DamageCounter();

			var objectsInShoutDistance = new List<IGameObject>();
			deffender.Shout(objectsInShoutDistance);
			imgoDeffenders = Game.GroupManager.CreateSelectedGroupMovable(objectsInShoutDistance);
			isgoDeffenders = Game.GroupManager.CreateSelectedGroupStatic(objectsInShoutDistance);

			attackerTarget = new Property<IGameObject>(deffender);
			deffenderTarget = new Property<IGameObject>(attackers[0]);

			StartFight();
		}

		/// <summary>
		/// Opposing objects gets another target.
		/// </summary>
		/// <param name="team">IGameObject's Team is used for find another oponent</param>
		/// <returns>Next target</returns>
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


		public bool TryAttack(IGameObject gameObject, IGameObject target, float attackDistance) {
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
						Game.IMoveManager.GoToTarget(imgo, target, this);
					}
					//Console.WriteLine(gameObject.Name + " je moc daleko");
					return false;
				}
				return true;
			}

			return false;
		}

		protected bool CanAttack(IGameObject gameObject) {
			if (canAttack.ContainsKey(gameObject)) {
				return canAttack[gameObject];
			}
			return true;
		}

		public void BulletHit(IBullet bullet, IGameObject hittedObject) {
			Console.WriteLine(bullet.Name + " kulka trefila " + hittedObject.Name);
			hittedObject.TakeDamage(damageCounter.CountDamage(hittedObject, bullet));
		}

		public void BulletMiss(IBullet bullet) {

		}

		/// <summary>
		/// Each object in the Fight will start attacking.
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

		private void OnAttackerDieEvent(IGameObject igo, MyDieArgs m) {
			Console.WriteLine(igo.Name + " chcipnul v souboji");
			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				RemoveFromDeffenderTarget(imgo);
			}

		}

		private void OnDeffenderDieEvent(IGameObject igo, MyDieArgs m) {
			Console.WriteLine(igo.Name + " chcipnul v souboji");

			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				RemoveFromAttackerTarget(imgo);
			} else {
				var isgo = igo as IStaticGameObject;
				if (isgo != null) {
					RemoveFromAttackerTarget(isgo);
				}
			}
		}

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

		private void RemoveFromAttackerTarget(IMovableGameObject igo) {
			imgoDeffenders.RemoveMember(igo);
			if (attackerTarget.Value == igo) {
				SelectAttackerTarget();
			}
		}

		private void RemoveFromAttackerTarget(IStaticGameObject igo) {
			isgoDeffenders.RemoveMember(igo);
			if (attackerTarget.Value == igo) {
				SelectAttackerTarget();
			}
		}

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

		public void MovementFinished(IMovableGameObject imgo) {
			movingDict.Remove(imgo);
			imgo.StartAttack(this);
		}

		public void MovementInterupted(IMovableGameObject imgo) {
			Console.WriteLine("pohyb prerusen");
		}

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
		/// Checks if the group already striking at the target
		/// </summary>
		/// <param name="group">Checking group</param>
		/// <param name="target">Attacking target</param>
		/// <returns>Returns if group attacking the target or not.</returns>
		public bool ContainsAttackingGroup(GroupMovables group, IGameObject target) {
			if (group == groupAttackers) {
				var imgo = target as IMovableGameObject;
				if (imgo != null && imgoDeffenders.HasMember(imgo)) {
					return false;
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

		public bool Check(float delay) {

			float xd = 0;
			float yd = 0;
			float squareDistance = 0;

			foreach (var item in movingDict) {
				xd = item.Value.Position.x - item.Key.Position.x;
				yd = item.Value.Position.z - item.Key.Position.z;
				squareDistance = xd * xd + yd * yd;

				if (squareDistance > farFarAway * farFarAway) {
					item.Key.Stop();
					return true;
				}
			}

			return false;
		}

		public void AddGroup(GroupMovables group) {
			GroupManager groupMgr = Game.GroupManager;
			if (group.OwnerTeam == groupAttackers.OwnerTeam) {
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
	}

}
