using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.FightMgr {
	public interface IBulletStopReciever {
		/// <summary>
		/// Calls when bullet hit the object.
		/// </summary>
		/// <param name="bullet">The bullet whitch hitted the object.</param>
		/// <param name="hittedObject">The hitted object.</param>
		void BulletHit(IBullet bullet, IGameObject hittedObject);
	}
}
