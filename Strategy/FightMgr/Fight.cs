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
	public class Fight : IBulletStopReciever {
		private GroupMovables groupAttackers;
		private GroupMovables imgoDeffenders;
		private GroupStatics isgoDeffenders;
		private DamageCounter damageCounter;

		private Property<IGameObject> attackerTarget;
		private Property<IGameObject> deffenderTarget;

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
				return null; //TODO destroy Fight
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
					//neco jako stopattack a moveto s hlidanim
					Console.WriteLine(gameObject.Name + " je moc daleko");
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
				imgoAtt.DieHandler += OnDieEvent;
			}
			foreach (IMovableGameObject imgoDeff in imgoDeffenders) {
				imgoDeff.StartAttack(this);
				imgoDeff.DieHandler += OnDieEvent;
			}
			foreach (IStaticGameObject isgoDeff in isgoDeffenders) {
				isgoDeff.StartAttack(this);
				isgoDeff.DieHandler += OnDieEvent;
			}
		}

		private void OnDieEvent(IGameObject igo, MyDieArgs m) {
			Console.WriteLine(igo.Name + " chcipnul v souboji");

			var imgo = igo as IMovableGameObject;
			if (imgo != null) {
				if (imgoDeffenders.OwnerTeam == imgo.Team) {
					RemoveFromAttackerTarget(imgo);
				} else {
					RemoveFromDeffenderTarget(imgo);
				}
			} else {
				// IStaticGameObject is from isgoDeffenders
				var isgo = igo as IStaticGameObject;
				if (isgo != null) {
					RemoveFromAttackerTarget(isgo);
				}
			}
		}

		private void SelectAttackerTarget() {
			if (imgoDeffenders.Count == 0) {
				attackerTarget.Value = isgoDeffenders[0];
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
				deffenderTarget.Value = groupAttackers[0];
			}
		}
	}

}
