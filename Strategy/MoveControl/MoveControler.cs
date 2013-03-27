using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.MoveControl {
	class MoveControler:IMoveControler {

		private const int randConst = 40;


		private static MoveControler instance;

		public static MoveControler getInstance() {
			if (instance == null) {
				instance = new MoveControler();
			}
			return instance;
		}

		private MoveControler(){

		}

		private Mogre.Vector3 randomizeVector(Mogre.Vector3 v) {
			Random r = new Random();
			int i = r.Next(4);
			switch (i) {
				case 0: v.x += randConst;
					break;
				case 1: v.x -= randConst;
					break;
				case 2: v.z += randConst;
					break;
				case 3: v.z -= randConst;
					break;
			}
			return v;
		}

		public void goToLocation(GroupMovables group, Mogre.Vector3 to) {
			Dictionary<Mogre.Vector3, IMovableGameObject> collision = new Dictionary<Mogre.Vector3, IMovableGameObject>();
			Mogre.Vector3 positionToGo = to;
			foreach (IMovableGameObject imgo in group) {
				if (collision.ContainsKey(positionToGo)) {
					bool isTaken = true;
					positionToGo = to;

					while (isTaken) {
						if (!collision.ContainsKey(positionToGo)) {
							collision.Add(positionToGo, imgo);
							isTaken = false;
						} else {
							positionToGo = randomizeVector(positionToGo);
						}
					}
					imgo.setNextLocation(positionToGo);
				} else {
					collision.Add(to, imgo);
					imgo.setNextLocation(to);
				}
			}
		}

		public void goToLocation(IMovableGameObject imgo, Mogre.Vector3 to) {
			var a = new LinkedList<Mogre.Vector3>();
			a.AddLast(to);
			imgo.setNextLocation(a);
		}



		public void runAwayFrom(GroupMovables group, Mogre.Vector3 from) {
			throw new NotImplementedException();
		}
	}
}
