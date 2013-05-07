using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.MoveMgr {
	public interface IMoveManager : IFinishMovementReciever {
		void GoToLocation(IMovableGameObject imgo, Vector3 to);
		void GoToLocation(GroupMovables group, Vector3 to);

		void GoToTarget(GroupMovables group, object gameObject, IFinishMovementReciever reciever);
		void GoToTarget(GroupMovables group, object gameObject);
		void RunAwayFrom(GroupMovables group, Vector3 from);

		void UnlogFromFinishMoveReciever(IMovableGameObject imgo);
		void Update();
	}
}
