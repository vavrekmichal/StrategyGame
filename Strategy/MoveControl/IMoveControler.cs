using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.MoveControl {
	interface IMoveControler {
		void goToLocation(IMovableGameObject imgo, Mogre.Vector3 to);
		void goToLocation(GroupMovables group, Mogre.Vector3 to);
	}
}
