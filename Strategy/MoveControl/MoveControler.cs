using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.MoveControl {
	class MoveControler:IMoveControler {

		private static MoveControler instance;

		public static MoveControler getInstance() {
			if (instance == null) {
				instance = new MoveControler();
			}
			return instance;
		}

		private MoveControler(){

		}

		public LinkedList<Mogre.Vector3> getTravel(Mogre.Vector3 from, Mogre.Vector3 to) {
			var a = new LinkedList<Mogre.Vector3>();
			a.AddLast(to);
			return a;
		}

	}
}
