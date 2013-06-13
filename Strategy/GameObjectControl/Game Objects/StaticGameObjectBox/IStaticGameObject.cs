
namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	/// <summary>
	/// Represents static game objects in the game. All object which implements this interface 
	/// should have last optional argument the Hp.
	/// </summary>
	public interface IStaticGameObject : IGameObject {

		/// <summary>
		/// Moves with the object in visible mode (the object is in active SolarSystem).
		/// </summary>
		/// <param name="f">The delay between last two frames (seconds).</param>
		void Rotate(float f);

		/// <summary>
		/// Moves with the object in invisible mode (the object is in non-active SolarSystem). 
		/// </summary>
		/// <param name="f">The delay between last two frames (seconds).</param>
        void NonActiveRotate(float f);
	}
}
