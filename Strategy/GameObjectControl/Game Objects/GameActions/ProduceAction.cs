using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	/// <summary>
	/// Calls team action Produce everey frame update with setted production (production.Value * delay)
	/// </summary>
	class ProduceAction : IGameAction {
		private IGameObject gameObject;

		private Property<double> production;
		private string produceMaterial;

		/// <summary>
		/// Initializes an owner, a producing material and a producing quantity.
		/// </summary>
		/// <param name="gameObject">The game action's owner.</param>
		/// <param name="args">The game action's arguments. Should be 2 (material name and quantity)</param>
		public ProduceAction(IGameObject gameObject, object[] args) {
			this.gameObject = gameObject;
			produceMaterial = (string)args[0];
			production = Game.PropertyManager.GetProperty<double>("baseProduction");
		}

		/// <summary>
		/// Produces setted quantity of the material per seceond. On Update produces production per second
		/// (production.Value) multiply by time (delay).
		/// </summary>
		/// <param name="delay">The deley between last two frames.</param>
		public void Update(float delay) {
			gameObject.Team.Produce(produceMaterial, production.Value * delay);
		}

		/// <summary>
		/// Just returns infromatrion about a production.
		/// </summary>
		/// <returns>Returns information about production of the material per second</returns>
		public string OnMouseClick() {
			return "Actual production is " + production.Value + " of "+produceMaterial+ " per second.";
		}

		/// <summary>
		/// Returns path to a icon picture.
		/// </summary>
		/// <returns>Returns path to a icon picture.</returns>
		public string IconPath() {
			return "../../media/icons/stone.png";
		}
	}
}
