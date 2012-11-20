using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GroupControl.Game_Objects {
	interface IGameAction {
		bool canExecute();
		void execute();
	}
}
