using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.FightMgr {
	class Fight {
		GroupMovables groupAttackers;
		GroupMovables imgoDeffenders;
		GroupStatics isgoDeffenders;


		AttackExecuter attackExec;
		DamageCounter damageCounter;

		public Fight(GroupMovables attackers, IGameObject deffenders) {
			groupAttackers = attackers;
			attackExec = new AttackExecuter();
			damageCounter = new DamageCounter();
			var objectsInShoutDistance = new List<IGameObject>();
			deffenders.Shout(objectsInShoutDistance);
			imgoDeffenders = Game.GroupManager.CreateSelectedGroupMovable(objectsInShoutDistance);
			isgoDeffenders = Game.GroupManager.CreateSelectedGroupStatic(objectsInShoutDistance);
		}



	}

}
