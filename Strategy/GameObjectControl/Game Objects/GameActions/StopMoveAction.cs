using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	/// <summary>
	/// Stops movement. This is a group game action (no user defined).
	/// </summary>
	class StopMoveAction : IGameAction {

		List<IMovableGameObject> imgoList;

		/// <summary>
		/// Stores given list with IMovableGameObjects
		/// </summary>
		/// <param name="imgoList">The list with IMovableGameObjects</param>
		public StopMoveAction(List<IMovableGameObject> imgoList) {
			this.imgoList = imgoList;
		}

		/// <summary>
		/// Does nothing on Update.
		/// </summary>
		/// <param name="delay">The delay between last to frames.</param>
		public void Update(float delay) {}

		/// <summary>
		/// Calls Stop function on each object in the list.
		/// </summary>
		/// <returns>Return information that movement is stopped.</returns>
		public string OnMouseClick() {
			foreach (IMovableGameObject imgo in imgoList) {
				imgo.Stop();
			}
			return "Movement is stopped";
		}

		/// <summary>
		/// Returns path to a icon picture.
		/// </summary>
		/// <returns>Returns path to a icon picture.</returns>
		public string IconPath() {
			return "../../media/icons/stop.png";
		}
	}
}
