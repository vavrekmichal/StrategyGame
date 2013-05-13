using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.FightMgr {
	class AttackExecuter {

		private static Dictionary<GameObject, bool> canAttack = new Dictionary<GameObject,bool>();

		public AttackExecuter() {
			
		}

		public IBullet TryAttack(IBullet bullet) { return null; }

		protected bool CanAttack() { return false; }

		protected IBullet Attack(IBullet bullet) { return null; }
	}
}
