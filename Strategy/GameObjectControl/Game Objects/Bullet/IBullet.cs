using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.Game_Objects.Bullet {
	public interface IBullet {
		string Name { get; }
		int Attack { get; }

		void Update(float delay);
		void HiddenUpdate(float delay);

		void ChangeVisible(bool visible);

		void Destroy();
	}
}
