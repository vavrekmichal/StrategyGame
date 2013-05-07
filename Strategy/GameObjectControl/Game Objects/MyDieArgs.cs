using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.Game_Objects {
	public class MyDieArgs : EventArgs {
		int hp;
		public MyDieArgs(int hp) {
			this.hp = hp;
		}

		public int Hp {
			get { return hp; }
		}

	}
}
