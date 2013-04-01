using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GroupControl {
	public enum ActionAnswer {
		None = 1,
		Attack = 5, 
		Move = 2, 
		RunAway = 3
	}

	public enum ActionReason {
		Friendly, Enemy, onRightButtonClick
	}
}
