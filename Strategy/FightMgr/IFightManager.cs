using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.MoveMgr;

namespace Strategy.FightMgr {
	interface IFightManager : IFinishMovementReciever{
		void Attack(GroupMovables group, IGameObject gameObject);
		void Occupy(GroupMovables group, IGameObject gameObject);

		void Update(float delay);
	}
}
