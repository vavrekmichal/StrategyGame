using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.MoveMgr {
	public interface IFinishMovementReciever {
		void MovementFinished(IMovableGameObject imgo);
		void MovementInterupted(IMovableGameObject imgo);
	}
}
