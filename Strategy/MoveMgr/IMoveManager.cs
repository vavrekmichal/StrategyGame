using Mogre;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects;
using System.Collections.Generic;

namespace Strategy.MoveMgr {
	/// <summary>
	/// Controls the movement (controlled/uncontroled). When the target reachs or interupts the contolled movement,
	/// the IMoveManager reports the massage to the object which asks for the controlled movement. Uncontrolled move
	/// just sends the objects to the destination.
	/// </summary>
	public interface IMoveManager : IFinishMovementReciever {

		/// <summary>
		/// Sends the object to the point. (Uncontrolled movement)
		/// </summary>
		/// <param name="imgo">The object which is sended to the destiantion.</param>
		/// <param name="to">The destiantion of the movement.</param>
		void GoToLocation(IMovableGameObject imgo, Vector3 to);

		/// <summary>
		/// Sends the whole group to the point. (Uncontrolled movement)
		/// </summary>
		/// <param name="group">The group which is sended to the destiantion.</param>
		/// <param name="to">The destiantion of the movement.</param>
		void GoToLocation(GroupMovables group, Vector3 to);

		/// <summary>
		/// Sends the whole group to the position of the given game object and sets the IFinishMovementReciever
		/// for this group. (Controlled movement) 
		/// </summary>
		/// <param name="group">The group which is sended to the position of the game object.</param>
		/// <param name="gameObject">The object where the group goes.</param>
		/// <param name="reciever">The IFinishMovementReciever for the group.</param>
		void GoToTarget(GroupMovables group, IGameObject gameObject, IFinishMovementReciever reciever);

		/// <summary>
		/// Sends the movable object to the position of the given game object and sets the IFinishMovementReciever
		/// for the movable object. (Controlled movement)  
		/// </summary>
		/// <param name="imgo">The movable object which is sended to the position of the game object.</param>
		/// <param name="gameObject">The object where the movable object goes.</param>
		/// <param name="reciever">The IFinishMovementReciever for the movable object.</param>
		void GoToTarget(IMovableGameObject imgo, IGameObject gameObject, IFinishMovementReciever reciever);

		/// <summary>
		/// Sends the whole group to the position of the given game object. (Uncontrolled movement) 
		/// </summary>
		/// <param name="group">The group which is sended to the position of the game object.</param>
		/// <param name="gameObject">The object where the group goes.</param>
		void GoToTarget(GroupMovables group, IGameObject gameObject);

		/// <summary>
		/// Removes the target from contoled movements.
		/// </summary>
		/// <param name="imgo">The removing target.</param>
		void UnlogFromFinishMoveReciever(IMovableGameObject imgo);

		/// <summary>
		/// Updates all controled movements.
		/// </summary>
		void Update();

		/// <summary>
		/// Returns all controled movements without the movements with setted IFinishMovementReciever.
		/// </summary>
		/// <returns>Returns all controled movements without the movements with setted IFinishMovementReciever.</returns>
		Dictionary<IMovableGameObject, IGameObject> GetAllMovements();

		/// <summary>
		/// Initilizes loaded movements.
		/// </summary>
		/// <param name="loadedMovements">Contains loaded movements (moving object, target).</param>
		void Initialize(Dictionary<string, string> loadedMovements);
	}
}
