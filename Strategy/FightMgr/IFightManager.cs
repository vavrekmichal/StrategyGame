using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.FightMgr {
	interface IFightManager {
		void update();
		void attack(GroupMovables group, object gameObject);
		void occupy();
	}
}
