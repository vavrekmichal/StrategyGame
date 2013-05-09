using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.MoveMgr;

namespace Strategy.FightMgr {
	interface IFightManager : IFinishMovementReciever{
		/// <summary>
		/// Attack registers attacking group. IFightManager shall carry out combat.
		/// (Move to combat distance, complete defender's group, attacking...)
		/// </summary>
		/// <param name="group">Attackers</param>
		/// <param name="gameObject">Target</param>
		void Attack(GroupMovables group, IGameObject gameObject);

		/// <summary>
		/// Occupy registers occupating group. IFightManager shall carry out occupation.
		/// (Move to occupy-distance, occupation...)
		/// </summary>
		/// <param name="group">Attackers</param>
		/// <param name="gameObject">Target</param>
		void Occupy(GroupMovables group, IGameObject gameObject);


		void Update(float delay);
	}
}
