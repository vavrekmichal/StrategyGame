using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GroupControl {
	public enum ActionAnswer {
		None, Attack, Move, RunAway, Reply
	}

	[Flags]
	public enum ActionFlag {
		Friendly, Enemy, onRightButtonClick
	}
}
