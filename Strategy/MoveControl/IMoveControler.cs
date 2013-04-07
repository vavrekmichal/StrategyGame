using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GroupControl;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.MoveControl {
	public interface IMoveControler {
		void goToLocation(IMovableGameObject imgo, Vector3 to);
		void goToLocation(GroupMovables group, Vector3 to);
		void goToTarget(GroupMovables group, IStaticGameObject target);
		void runAwayFrom(GroupMovables group, Vector3 from);

		void interuptMove(IMovableGameObject imgo);

		void update(float delay);
	}
}
