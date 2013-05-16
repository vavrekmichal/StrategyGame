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
	class Fight : IBulletStopReciever {
		private GroupMovables groupAttackers;
		private GroupMovables imgoDeffenders;
		private GroupStatics isgoDeffenders;

		private static Dictionary<IGameObject, bool> canAttack = new Dictionary<IGameObject, bool>();
		private Dictionary<string, IBullet> bulletDict;

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

		DamageCounter damageCounter;

		public Fight(GroupMovables attackers, IGameObject deffender) {
			groupAttackers = attackers;
			damageCounter = new DamageCounter();
			bulletDict = new Dictionary<string,IBullet>();
			var objectsInShoutDistance = new List<IGameObject>();
			deffender.Shout(objectsInShoutDistance);
			imgoDeffenders = Game.GroupManager.CreateSelectedGroupMovable(objectsInShoutDistance);
			isgoDeffenders = Game.GroupManager.CreateSelectedGroupStatic(objectsInShoutDistance);
			var solS = Game.GroupManager.GetSolarSystem(Game.GroupManager.GetSolarSystemsNumber(attackers[0]));
			bulletDict.Add("bla",new Missile(Game.SceneManager, new Mogre.Vector3(100, 0, 100), "bla", solS, deffender.Position, this));
		}



		public IBullet TryAttack(IBullet bullet) { return null; }

		protected bool CanAttack() { return false; }

		protected IBullet Attack(IBullet bullet) { return null; }



		public void BulletHit(IBullet bullet, IGameObject hittedObject) {
			Console.WriteLine(bullet.Name+" kulka trefila "+ hittedObject.Name);
			hittedObject.TakeDamage(damageCounter.CountDamage(hittedObject,bullet)); 
		}

		public void BulletMiss(IBullet bullet) {
			if (bulletDict.ContainsKey(bullet.Name)) {
				bulletDict.Remove(bullet.Name);
			}
		}
	}

}
