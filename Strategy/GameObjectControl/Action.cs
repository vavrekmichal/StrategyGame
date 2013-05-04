using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl {
	public enum ActionAnswer {
		None = 1,
		Move = 2, 
		RunAway = 3,
		MoveTo = 4,
		Attack = 5, 
		Occupy = 6
	}

	public enum ActionReaction {
		GUI,
		None
	}

	public enum ActionReason {
		targetInDistance
	}
}
