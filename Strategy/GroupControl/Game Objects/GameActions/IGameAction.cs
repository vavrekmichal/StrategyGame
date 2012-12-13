using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.GameActions {
	interface IGameAction {
        void execute(object executer, Team team);

        string getName();
	}
}
