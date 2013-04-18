using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.MoveMgr;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	public interface IMovableGameObject {

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

		/// <summary>
		/// Function inicializes or destroy SceneNode. It means hide or show object.
		/// </summary>
		/// <param name="visible">Object shoud be visible (true) or hide (false)</param>
		void changeVisible(bool visible);


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
		/// <param name="moveMgr">This object controls move and informs other object when is position reached</param>
		void goToTarget(LinkedList<Vector3> positionList, IMoveManager moveMgr);

		/// <summary>
		/// Controled movement of object. When object reached position, must report to moveCntr.
		/// </summary>
		/// <param name="placeToGo">Vectror3 with position to go</param>
		/// <param name="moveMgr">This object controls move and informs other object when is position reached</param>
		void goToTarget(Vector3 placeToGo, IMoveManager moveMgr);

		/// <summary>
		/// Returns Property with given name
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		Property<T> getProperty<T>(string propertyName);

		/// <summary>
		/// BunusDict contains bonuses from group and members of group. Object can use them to count his properties.
		/// </summary>
		/// <param name="bonusDict">Dictionary with Property as object (runtime generic)</param>
		void setGroupBonuses(Dictionary<string, object> bonusDict);

		string Name { get; }
		Team Team { get; set; }
		bool Visible { get; }
		Mogre.Vector3 Direction { get; }
		Mogre.Vector3 Position { get; }

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
		ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject);

		/// <summary>
		/// Returns bonuses for other members of group.
		/// </summary>
		/// <returns>Dictionary with Property as object (runtime generic)</returns>
		Dictionary<string, object> onGroupAdd();

		/// <summary>
		/// Return Dictionary with all object Property(ies).
		/// </summary>
		/// <returns>Dictionary with Property as object (runtime generic)</returns>
		Dictionary<string, object> getPropertyToDisplay();
	}
}
