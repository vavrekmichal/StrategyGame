using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.MoveControl {
	interface IMoveControler {
		LinkedList<Mogre.Vector3> getTravel(Mogre.Vector3 from, Mogre.Vector3 to);
	}
}
