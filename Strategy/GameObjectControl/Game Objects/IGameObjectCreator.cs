using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects {
	/// <summary>
	/// Creates game objects and controls used object names.
	/// </summary>
	public interface IGameObjectCreator {
		/// <summary>
		/// Creates IStaticGameObject by given typeName and argument. Inserts the object to given SolarSystem.
		/// </summary>
		/// <param name="typeName">The type of the creating object.</param>
		/// <param name="args">The arguments of the creating object.</param>
		/// <param name="solSyst">The creating object SolarSystem.</param>
		/// <returns>Returns created IStaticGameObject.</returns>
		IStaticGameObject CreateIsgo(string typeName, object[] args, SolarSystem solSyst);

		/// <summary>
		/// Creates IMovableGameObject by given typeName and argument. Inserts the object to given SolarSystem.
		/// </summary>
		/// <param name="typeName">The type of the creating object.</param>
		/// <param name="args">The arguments of the creating object.</param>
		/// <param name="solSyst">The creating object SolarSystem.</param>
		/// <returns>Returns created IMovableGameObject.</returns>
		IMovableGameObject CreateImgo(string typeName, object[] args, SolarSystem solSyst);

		/// <summary>
		/// Controls used names and returns unused variant of the name.
		/// </summary>
		/// <param name="name">The base of a object name.</param>
		/// <returns>Returns unused name with base from given name.</returns>
		string GetUnusedName(string name);
	}

}
