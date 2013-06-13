using System.Collections.Generic;
using Mogre;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	/// <summary>
	/// Represents all movable game objects in the game. All object which implements this interface 
	/// should have last optional argument the Hp.
	/// </summary>
	public interface IMovableGameObject : IGameObject {
		/// <summary>
		/// Moves with the object in visible mode (Mogre.SceneNode is setted).
		/// </summary>
		/// <param Name="f">The delay between last two frames</param>
		void Move(float f);

		/// <summary>
		/// Stops the movement of the object.
		/// </summary>
		void Stop();

		/// <summary>
		/// Moves with the object in invisible mode (Mogre.SceneNode is unsetted).
		/// </summary>
		/// <param Name="f">The delay between last two frames</param>
		void NonActiveMove(float f);

		/// <summary>
		/// Adds a new position where the object must go (firts). Older positions will be visited later.
		/// </summary>
		/// <param Name="placeToGo">The Vectror3 with a position to go.</param>
		void AddNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Adds a list of the positions where the object must go (firts). Older positions will be visited later.
		/// </summary>
		/// <param Name="positionList">The list with positions.</param>
		void AddNextLocation(LinkedList<Vector3> positionList);

		/// <summary>
		/// Sets given position where the object must go. Older positions are cancelled.
		/// </summary>
		/// <param Name="placeToGo">The Vectror3 with position to go.</param>
		void SetNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Sets given list with positions where the object must go. Older positions are cancelled.
		/// </summary>
		/// <param Name="positionList">The list with positions.</param>
		void SetNextLocation(LinkedList<Vector3> positionList);

		/// <summary>
		/// Changes actual object position.
		/// </summary>
		/// <param Name="placeToGo">The new position.</param>
		void JumpToLocation(Vector3 placeToGo);

		/// <summary>
		/// Controles a movement of the object. When the object reachs the last position, must report to the MoveManager.
		/// Sets given list with positions where the object must go. Older positions are cancelled.	 
		/// </summary>
		/// <param Name="positionList">The list with positions.</param>
		void GoToTarget(LinkedList<Vector3> positionList);

		/// <summary>
		/// Controles a movement of the object. When the object reachs the last position, must report to the MoveManager.
		/// Sets given position where the object must go. Older positions are cancelled.
		/// </summary>
		/// <param Name="placeToGo">The Vectror3 with position to go.</param>
		void GoToTarget(Vector3 placeToGo);

		/// <summary>
		/// Controles a movement of the object. When the object reachs the last position, must report to the MoveManager.
		/// The object follows given target (go to objectToGo position).
		/// </summary>
		/// <param Name="objectToGo">IGameObject with position to go</param>
		void GoToTarget(IGameObject objectToGo);

		/// <summary>
		/// Sets bunusDict to the object. The object should use them to update his propertis.
		/// </summary>
		/// <param Name="bonusDict">The dictionary with Properties as object (runtime generic)</param>
		void SetGroupBonuses(Dictionary<string, object> bonusDict);

		/// <summary>
		/// Determines whether it is the object in visible mode or in invisible mode (Mogre.SceneNode is setted or unsetted).
		/// </summary>
		bool Visible { get; }

		/// <summary>
		/// Handles communication between game control units and object. Object gets a information about what happenes and it answes what it wants to do.
		/// </summary>
		/// <param name="point">The mouse position.</param>
		/// <param name="hitObject">The result of a HitTest.</param>
		/// <param name="isFriendly">The information if the hitted object is friendly.</param>
		/// <param name="isMovableGameObject">The information if the hitted object is movable.</param>
		/// <returns>Returns an action it wants to do.</returns>
		ActionAnswer OnMouseAction(Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject);

		/// <summary>
		/// Returns bonuses for other members of the group.
		/// </summary>
		/// <returns>Returns the dictionary with Property as object (runtime generic)</returns>
		Dictionary<string, object> OnGroupAdd();


	}
}
