using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl {
	/// <summary>
	/// Represents Action answers with assigned priority.
	/// </summary>
	public enum ActionAnswer {
		None = 1,
		Move = 2, 
		RunAway = 3,
		MoveTo = 4,
		Attack = 5, 
		Occupy = 6
	}
}
