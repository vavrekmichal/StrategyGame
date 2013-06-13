using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.MoveMgr;

namespace Strategy.FightMgr {
	/// <summary>
	/// Controls all fights and occupations.
	/// </summary>
	public interface IFightManager : IFinishMovementReciever{
		/// <summary>
		/// Registers the attacking group. Sends it to the specified position and start attack if it is possible.
		/// </summary>
		/// <param name="group">The attacking group.</param>
		/// <param name="gameObject">The target of the attack.</param>
		void Attack(GroupMovables group, IGameObject gameObject);

		/// <summary>
		/// Registers occupating group. Sends it to the specified position and start occupation if it is possible.
		/// </summary>
		/// <param name="group">The occupying group</param>
		/// <param name="gameObject">The target of the occupation.</param>
		void Occupy(GroupMovables group, IGameObject gameObject);

		/// <summary>
		/// Updates all fights and occupations.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		void Update(float delay);
	}
}
