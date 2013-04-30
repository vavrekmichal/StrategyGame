using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.FightMgr {
	class FightManager : IFightManager{

		private Dictionary<GroupMovables, GroupMovables> fightsDict;
		private Dictionary<GroupMovables, IStaticGameObject> occupatorsDict;
		
		public FightManager() {
			fightsDict = new Dictionary<GroupMovables, GroupMovables>();
			occupatorsDict = new Dictionary<GroupMovables, IStaticGameObject>();
		}

		public void update(){

		}

		public void attack(GroupMovables group, object gameObject) {
			if (gameObject is IStaticGameObject) {
				Console.WriteLine("Utocis na static");
			}
			if (gameObject is IMovableGameObject) {
				Console.WriteLine("Utocis na movable");
			}
		}

		public void occupy() {
			throw new NotImplementedException();
		}
	}
}
