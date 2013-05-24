using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	class StopMoveAction : IGameAction {

		List<IMovableGameObject> imgoList;

		public StopMoveAction(List<IMovableGameObject> imgoList) {
			this.imgoList = imgoList;
		}

		public void Update(float delay) {}

		public string OnMouseClick() {
			foreach (IMovableGameObject imgo in imgoList) {
				imgo.Stop();
			}
			return "Movement is stopped";
		}

		public string IconPath() {
			return "../../media/icons/stop.gif";
		}
	}
}
