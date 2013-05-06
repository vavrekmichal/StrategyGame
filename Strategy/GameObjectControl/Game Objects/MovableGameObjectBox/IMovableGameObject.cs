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
		/// <param name="f">Delay between last two frames</param>
		void move(float f);

		/// <summary>
		/// Function stops objects moving
		/// </summary>
		void stop();

		/// <summary>
		/// Non-Active move is called when SceneNode is unsetted (object is in hidden SolarSystem or travels between them)
		/// </summary>
		/// <param name="f">Delay between last two frames</param>
		void nonActiveMove(float f);


		void shout();

		/// <summary>
		/// Add position in flyList on a firts place
		/// </summary>
		/// <param name="placeToGo">Vectror3 with position to go</param>
		void addNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Add list with positions in flyList (after actual)
		/// </summary>
		/// <param name="positionList">List with positions (Vector3)</param>
		void addNextLocation(LinkedList<Vector3> positionList);

		/// <summary>
		/// Set given position in flyList (creates new)
		/// </summary>
		/// <param name="placeToGo">Vectror3 with position to go</param>
		void setNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Set list with positions as flyList
		/// </summary>
		/// <param name="positionList">List with positions (Vector3)</param>
		void setNextLocation(LinkedList<Vector3> positionList);

		/// <summary>
		/// Change actual position
		/// </summary>
		/// <param name="placeToGo">new position</param>
		void jumpNextLocation(Vector3 placeToGo);

		/// <summary>
		/// Controled movement of object. When object reached last position, must report to moveCntr.
		/// </summary>
		/// <param name="positionList">List with positions (Vector3)</param>
		void goToTarget(LinkedList<Vector3> positionList);

		/// <summary>
		/// Controled movement of object. When object reached position, must report to moveCntr.
		/// </summary>
		/// <param name="placeToGo">Vectror3 with position to go</param>
		void goToTarget(Vector3 placeToGo);

		/// <summary>
		/// Returns Property with given name (base properties)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="propertyName">Name of property from Enum (base properties)</param>
		/// <returns>Instance of Property</returns>
		Property<T> getProperty<T>(PropertyEnum propertyName);

		/// <summary>
		/// Returns Property with given name (user defined properties)
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="propertyName">Name of property (user defined properties)</param>
		/// <returns>Instance of Property</returns>
		Property<T> getProperty<T>(string propertyName);

		/// <summary>
		/// BunusDict contains bonuses from group and members of group. Object can use them to count his properties.
		/// </summary>
		/// <param name="bonusDict">Dictionary with Property as object (runtime generic)</param>
		void setGroupBonuses(Dictionary<string, object> bonusDict);

		bool Visible { get; }

		Mogre.Vector3 Direction { get; }

		int AttackPower { get; }
		int DeffPower { get; }
		int Hp { get; }

		/// <summary>
		/// Function handles communication between game control units and object. Object gets what happened and answed what it want to do.
		/// </summary>
		/// <param name="reason">Reason of calling function</param>
		/// <param name="point">Mouse position</param>
		/// <param name="hitObject">Hitted object</param>
		/// <param name="isFriendly">If object is in friendly team</param>
		/// <param name="isMovableGameObject">If object is movable or static</param>
		/// <returns>Answer on action</returns>
		ActionAnswer onMouseAction(Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject);

		/// <summary>
		/// Returns bonuses for other members of group.
		/// </summary>
		/// <returns>Dictionary with Property as object (runtime generic)</returns>
		Dictionary<string, object> onGroupAdd();


	}
}
