using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.MoveMgr {
	/// <summary>
	/// Transfers message when the object stops the movement and the reason of a stop (finish/interupt).
	/// </summary>
	public interface IFinishMovementReciever {
		/// <summary>
		/// Reports the movable object when the destiantion is reached.
		/// </summary>
		/// <param name="imgo">The reporting object which reached the destiantion.</param>
		void MovementFinished(IMovableGameObject imgo);

		/// <summary>
		/// Reports the movable object when the movement is interupted.
		/// </summary>
		/// <param name="imgo">The reporting object when the movement is interupted.</param>
		void MovementInterupted(IMovableGameObject imgo);
	}
}
