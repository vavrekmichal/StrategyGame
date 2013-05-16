using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	public interface IMovableGameObject : IGameObject {

		/// <summary>
		/// Active move - SceneNode is setted
		/// </summary>
		/// <param Name="f">Delay between last two frames</param>
		void Move(float f);

		/// <summary>
		/// Function stops objects moving
		/// </summary>
		void Stop();

		/// <summary>
		/// Non-Active move is called when SceneNode is unsetted (object is in hidden SolarSystem or travels between them)
		/// </summary>
		/// <param Name="f">Delay between last two frames</param>
		void NonActiveMove(float f);

		/// <summary>
		/// Add position in flyList on a firts place
		/// </summary>
		/// <param Name="placeToGo">Vectror3 with position to go</param>
		void AddNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Add list with positions in flyList (after actual)
		/// </summary>
		/// <param Name="positionList">List with positions (Vector3)</param>
		void AddNextLocation(LinkedList<Vector3> positionList);

		/// <summary>
		/// Set given position in flyList (creates new)
		/// </summary>
		/// <param Name="placeToGo">Vectror3 with position to go</param>
		void SetNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Set list with positions as flyList
		/// </summary>
		/// <param Name="positionList">List with positions (Vector3)</param>
		void SetNextLocation(LinkedList<Vector3> positionList);

		/// <summary>
		/// Change actual position
		/// </summary>
		/// <param Name="placeToGo">new position</param>
		void JumpNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Controled movement of object. When object reached last position, must report to moveCntr.
		/// </summary>
		/// <param Name="positionList">List with positions (Vector3)</param>
		void GoToTarget(LinkedList<Vector3> positionList);

		/// <summary>
		/// Controled movement of object. When object reached position, must report to moveCntr.
		/// </summary>
		/// <param Name="placeToGo">Vectror3 with position to go</param>
		void GoToTarget(Vector3 placeToGo);

		/// <summary>
		/// Returns Property with given Name (base properties)
		/// </summary>
		/// <typeparam Name="T">Type</typeparam>
		/// <param Name="propertyName">Name of property from Enum (base properties)</param>
		/// <returns>Instance of Property</returns>
		Property<T> GetProperty<T>(PropertyEnum propertyName);

		/// <summary>
		/// Returns Property with given Name (user defined properties)
		/// </summary>
		/// <typeparam Name="T">Type</typeparam>
		/// <param Name="propertyName">Name of property (user defined properties)</param>
		/// <returns>Instance of Property</returns>
		Property<T> GetProperty<T>(string propertyName);

		/// <summary>
		/// BunusDict contains bonuses from group and members of group. Object can use them to count his properties.
		/// </summary>
		/// <param Name="bonusDict">Dictionary with Property as object (runtime generic)</param>
		void SetGroupBonuses(Dictionary<string, object> bonusDict);

		bool Visible { get; }

		Mogre.Vector3 Direction { get; }

		/// <summary>
		/// Function handles communication between game control units and object. Object gets what happened and answed what it want to do.
		/// </summary>
		/// <param Name="reason">Reason of calling function</param>
		/// <param Name="point">Mouse position</param>
		/// <param Name="hitObject">Hitted object</param>
		/// <param Name="isFriendly">If object is in friendly team</param>
		/// <param Name="isMovableGameObject">If object is movable or static</param>
		/// <returns>Answer on action</returns>
		ActionAnswer OnMouseAction(Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject);

		/// <summary>
		/// Returns bonuses for other members of group.
		/// </summary>
		/// <returns>Dictionary with Property as object (runtime generic)</returns>
		Dictionary<string, object> OnGroupAdd();


	}
}
