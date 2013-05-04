using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.MoveMgr;

namespace Strategy.FightMgr {
	interface IFightManager : IFinishMovementReciever{
		void attack(GroupMovables group, object gameObject);
		void occupy(GroupMovables group, object gameObject);

		void update(float delay);
	}
}
