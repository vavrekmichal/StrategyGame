using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	public interface IMovableGameObject {
		void move(float f);
		void shout();
        string getName();
	}
}
