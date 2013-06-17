
namespace Strategy.Sound {
	/// <summary>
	/// Allows the game classes play a effects from the file with the game effects.
	/// </summary>
	public interface IEffectPlayer {

		/// <summary>
		/// Playes the effect by the given name.
		/// </summary>
		/// <param name="name"></param>
		void PlayEffect(string name);

	}
}
