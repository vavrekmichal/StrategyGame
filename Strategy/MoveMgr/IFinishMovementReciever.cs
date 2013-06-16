using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;

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
