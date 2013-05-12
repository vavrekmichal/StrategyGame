using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.FightMgr.Bullet;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.FightMgr {
	abstract class Fight<T> {
		GroupMovables groupAttackers;
		T groupDeffenders;

		AttackExecuter attackExec;
		DamageCounter damageCounter;

		public Fight(GroupMovables attackers, T deffenders) {
			groupAttackers = attackers;
			groupDeffenders = deffenders;
			attackExec = new AttackExecuter();
			damageCounter = new DamageCounter();
		}

	}

	class FightWithStatic : Fight<GroupStatics> {
		public FightWithStatic(GroupMovables attackers, GroupStatics deffenders)
			: base(attackers, deffenders) {

		}

	}

	class FightWithMovables : Fight<GroupMovables> {
		public FightWithMovables(GroupMovables attackers, GroupMovables deffenders)
			: base(attackers, deffenders) {

		}

	}
}
