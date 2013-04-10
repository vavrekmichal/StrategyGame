using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.FightMgr {
	class FightManager : IFightManager{

		private static FightManager instance;

		public static FightManager getInstance() {
			if (instance == null) {
				instance = new FightManager();
			}
			return instance;
		}

		private FightManager() {

		}

	}
}
