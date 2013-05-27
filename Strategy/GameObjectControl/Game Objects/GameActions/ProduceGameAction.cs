using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	class ProduceGameAction : IGameAction {
		private IGameObject gameObject;

		private Property<double> production;
		private string produceMaterial;

		public ProduceGameAction(IGameObject gameObject, object[] args) {
			this.gameObject = gameObject;
			produceMaterial = (string)args[0];
			production = Game.PropertyManager.GetProperty<double>("baseProduction");
		}

		public void Update(float delay) {
			gameObject.Team.Produce(produceMaterial, production.Value * delay);
		}

		public string OnMouseClick() {
			return "Actual production is " + production.Value + " of "+produceMaterial+ " per second.";
		}

		public string IconPath() {
			return "../../media/icons/stone.png";
		}
	}
}
