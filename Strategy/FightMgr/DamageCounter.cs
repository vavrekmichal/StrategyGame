using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.FightMgr {
	class DamageCounter {

		public DamageCounter() {

		}

		/// <summary>
		/// Function randomize bullet's attack power and count real damage. The result is returned if it is greater than 0.
		/// </summary>
		/// <param name="gameObject">Deffending object</param>
		/// <param name="bullet">Attacking bullet</param>
		/// <returns>Calculated damage when it is greater then 0</returns>
		public int CountDamage(IGameObject gameObject, IBullet bullet) {
			var damage = bullet.Attack + GetRandomNumberFromInterval(bullet.Attack) - gameObject.DeffPower;
			return damage > 0 ? damage : 0;
		}


		private static Random random = new Random();

		/// <summary>
		/// Function returns a random number from interval [-bulletPower/4, bulletPower/4].
		/// </summary>
		/// <param name="bulletPower">Power of IBullet</param>
		/// <returns>Random number from interval [-bulletPower/4, bulletPower/4]</returns>
		private static int GetRandomNumberFromInterval(int bulletPower) {
			int r = random.Next(bulletPower / 2);
			return r - bulletPower / 4;
		}
	}
}
