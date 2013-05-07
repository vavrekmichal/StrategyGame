using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	public interface IGameAction {
        void Execute(object executer, Team team);

		string Name {
			get;
		}
	}
}
