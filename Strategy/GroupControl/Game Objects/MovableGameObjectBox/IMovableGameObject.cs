using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	interface IMovableGameObject {
		void move(float f);
		void shout();
        string getName();
	}
}
