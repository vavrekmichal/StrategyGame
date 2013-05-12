using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.FightMgr.Bullet;
using Strategy.GameObjectControl.Game_Objects;

namespace Strategy.FightMgr {
	class AttackExecuter {

		private Dictionary<GameObject, bool> canAttack;

		public AttackExecuter() {
			canAttack = new Dictionary<GameObject, bool>();
		}

		public IBullet TryAttack(IBullet bullet) { return null; }

		protected bool CanAttack() { return false; }

		protected IBullet Attack(IBullet bullet) { return null; }
	}
}
