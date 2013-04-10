using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.MoveMgr {
	public interface IMoveManager {
		void goToLocation(IMovableGameObject imgo, Vector3 to);
		void goToLocation(GroupMovables group, Vector3 to);
		void goToTarget(GroupMovables group, IStaticGameObject target);
		void runAwayFrom(GroupMovables group, Vector3 from);

		void interuptMove(IMovableGameObject imgo);

		void update(float delay);
	}
}
