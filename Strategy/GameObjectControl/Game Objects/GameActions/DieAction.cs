using System.Collections.Generic;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	/// <summary>
	/// Kills all objects in list. This is a group game action (no user defined).
	/// </summary>
	/// <typeparam name="T">The type could be IMovableGameObject or IStaticGameObject</typeparam>
	class DieAction<T> : IGameAction where T: IGameObject {

		List<T> gameObjectList;

		/// <summary>
		/// Initialize list with targets to kill.
		/// </summary>
		/// <param name="objectList">The list with targets.</param>
		public DieAction(List<T> objectList) {
			gameObjectList = objectList;
		}

		/// <summary>
		/// Does nothing on Update.
		/// </summary>
		/// <param name="delay">The delay between last to frames.</param>
		public void Update(float delay) { }

		/// <summary>
		/// damage greater damage than current lives for all targets in the list.
		/// </summary>
		/// <returns>Returns information that objects are death.</returns>
		public string OnMouseClick() {
			foreach (IGameObject gameObject in new List<T>(gameObjectList)) {
				gameObject.TakeDamage(gameObject.Hp + 1);
			}
			return "Objects are destroyed";
		}

		/// <summary>
		/// Returns path to a icon picture.
		/// </summary>
		/// <returns>Returns path to a icon picture.</returns>
		public string IconPath() {
			return "../../media/icons/skull.png";
		}
	}
}
