﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.FightMgr {
	class DamageCounter {

		/// <summary>
		/// Randomize bullet's attack power and count real damage. The result is returned if it is greater than 0. When the result 
		/// is lower then 0 so function returns 0 (attack cannot "heal").
		/// </summary>
		/// <param name="gameObject">The object which defends itself.</param>
		/// <param name="bullet">The bullet that hit the object.</param>
		/// <returns>Calculated damage when it is greater then 0.</returns>
		public int CountDamage(IGameObject gameObject, IBullet bullet) {
			var damage = GetRandomizeAttack(bullet.Attack) - gameObject.DeffPower;
			return damage > 0 ? damage : 0;
		}


		private static Random random = new Random();

		/// <summary>
		/// Returns a random number from interval [-bulletPower/4, bulletPower/4].
		/// </summary>
		/// <param name="bulletPower">Power of IBullet</param>
		/// <returns>Random number from interval [-bulletPower/4, bulletPower/4]</returns>
		private static int GetRandomizeAttack(int bulletPower) {
			int r = random.Next(bulletPower / 2);
			return bulletPower + r - bulletPower / 4;
		}
	}
}
