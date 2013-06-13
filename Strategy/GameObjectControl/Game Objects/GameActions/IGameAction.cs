
namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	/// <summary>
	/// Is update on every new frame. IGameObject is owner of game actions. Targeted IGameObject
	/// shows its game actions and user can Click on them.
	/// </summary>
	public interface IGameAction {
		/// <summary>
		/// Updates game action.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		void Update(float delay);

		/// <summary>
		/// Reacts on mouse click. When is clicked so IGameAction owner is targeted.
		/// </summary>
		/// <returns>Returns answer which will be printed to the game console.</returns>
		string OnMouseClick();

		/// <summary>
		/// Returns path to a icon picture.
		/// </summary>
		/// <returns>Returns icon path.</returns>
		string IconPath();

	}
}
