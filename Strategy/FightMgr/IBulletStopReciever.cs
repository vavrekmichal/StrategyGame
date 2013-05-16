using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.FightMgr {
	public interface IBulletStopReciever {
		void BulletHit(IBullet bullet, IGameObject hittedObject);
		void BulletMiss(IBullet bullet);
	}
}
