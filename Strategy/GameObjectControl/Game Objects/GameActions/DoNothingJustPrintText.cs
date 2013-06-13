
namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	/// <summary>
	/// Prints text. Prototyp game action.
	/// </summary>
	class DoNothingJustPrintText : IGameAction {

		private IGameObject gameObject;

		/// <summary>
		/// Just saves rafarence to owner.
		/// </summary>
		/// <param name="gameObject">The IGameAction owner.</param>
		/// <param name="args">The arguments should be empty.</param>
		public DoNothingJustPrintText(IGameObject gameObject, object[] args) {
			this.gameObject = gameObject;
		}

		/// <summary>
		/// Does nothing on Update.
		/// </summary>
		/// <param name="delay">The delay between last to frames.</param>
		public void Update(float delay) {
		}

		/// <summary>
		/// Just returns text to print.
		/// </summary>
		/// <returns>Returns text to print.</returns>
		public string OnMouseClick() {
			return "This is just prototype of function.";
		}


		/// <summary>
		/// Returns path to a icon picture.
		/// </summary>
		/// <returns>Returns path to a icon picture.</returns>
		public string IconPath() {
			return "../../media/icons/write.png";
		}
	}
}
