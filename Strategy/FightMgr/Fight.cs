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
using Strategy.MoveMgr;

namespace Strategy.FightMgr {
	public class Fight : IBulletStopReciever {
		private GroupMovables groupAttackers;
		private GroupMovables imgoDeffenders;
		private GroupStatics isgoDeffenders;
		private DamageCounter damageCounter;

		private static Dictionary<IGameObject, bool> canAttack = new Dictionary<IGameObject, bool>();


		public static void DisableAttackGameObject(IGameObject gameObject) {
			if (canAttack.ContainsKey(gameObject)) {
				canAttack[gameObject] = false;
			} else {
				canAttack.Add(gameObject, false);
			}
		}

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
			var solS = Game.GroupManager.GetSolarSystem(Game.GroupManager.GetSolarSystemsNumber(attackers[0]));
			new Missile(Game.SceneManager, new Mogre.Vector3(100, 0, 100), "bla", solS, deffender.Position, this);
		}



		public bool TryAttack(IGameObject gameObject, IGameObject target, float attackDistance) {
			if (CanAttack(gameObject)) {
				var xd = target.Position.x - gameObject.Position.x;
				var yd = target.Position.z - target.Position.z;
				var squareDistance = xd * xd + yd * yd;

				if (squareDistance > attackDistance) {
					// Cannot attack because it is too far.
					//neco jako stopattack a moveto s hl9d8n9m
					Console.WriteLine("Je moc daleko");
					return false;
				}
				Console.WriteLine(gameObject.Name + " bude utocit.");
				return true;
			}

			return false;
		}

		protected bool CanAttack(IGameObject gameObject) {
			if (canAttack.ContainsKey(gameObject)) {
				return canAttack[gameObject];
			}
			return false;
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
			}
			foreach (IMovableGameObject imgoDeff in imgoDeffenders) {
				imgoDeff.StartAttack(this);
			}
			foreach (IStaticGameObject isgoDeff in isgoDeffenders) {
				isgoDeff.StartAttack(this);
			}
		}
	}

}
