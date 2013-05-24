using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	class DieGameAction<T> : IGameAction where T: IGameObject {

		List<T> gameObjectList;

		public DieGameAction(List<T> objectList) {
			gameObjectList = objectList;
		}

		public void Update(float delay) { }

		public string OnMouseClick() {
			foreach (IGameObject gameObject in new List<T>(gameObjectList)) {
				gameObject.TakeDamage(gameObject.Hp + 1);
			}
			return "Objects are destroyed";
		}

		public string IconPath() {
			return "../../media/icons/skull.gif";
		}
	}
}
