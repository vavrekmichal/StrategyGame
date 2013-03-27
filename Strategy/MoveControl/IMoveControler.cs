using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GroupControl;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.MoveControl {
	interface IMoveControler {
		void goToLocation(IMovableGameObject imgo, Vector3 to);
		void goToLocation(GroupMovables group, Vector3 to);
		void runAwayFrom(GroupMovables group, Vector3 from);
	}
}
