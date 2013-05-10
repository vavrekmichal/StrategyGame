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

		public Fight(GroupMovables attackers, T deffenders) {
			groupAttackers = attackers;
			groupDeffenders = deffenders;
		}

		public abstract void TryAttack(IBullet bullet);

		protected abstract bool CanAttack();

		protected abstract void Attack(IBullet bullet);
	}

	class FightWithStatic : Fight<GroupStatics> {
		public FightWithStatic(GroupMovables attackers, GroupStatics deffenders)
			: base(attackers, deffenders) {

		}

		public override void TryAttack(IBullet bullet) {
			throw new NotImplementedException();
		}

		protected override bool CanAttack() {
			throw new NotImplementedException();
		}

		protected override void Attack(IBullet bullet) {
			throw new NotImplementedException();
		}
	}

	class FightWithMovables : Fight<GroupMovables> {
		public FightWithMovables(GroupMovables attackers, GroupMovables deffenders)
			: base(attackers, deffenders) {

		}

		public override void TryAttack(IBullet bullet) {
			throw new NotImplementedException();
		}

		protected override bool CanAttack() {
			throw new NotImplementedException();
		}

		protected override void Attack(IBullet bullet) {
			throw new NotImplementedException();
		}
	}
}
