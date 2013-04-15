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

		void move(float f);
		void stop();

		void nonActiveMove(float f);
		void changeVisible(bool visible);
		void shout();
		void addNextLocation(Vector3 placeToGo);
		void addNextLocation(LinkedList<Vector3> positionList);

		void setNextLocation(Vector3 placeToGo);
		void setNextLocation(LinkedList<Vector3> positionList);

		void jumpNextLocation(Vector3 placeToGo);

		void goToTarget(LinkedList<Vector3> positionList, IMoveManager moveCntr);
		void goToTarget(Vector3 placeToGo, IMoveManager moveCntr);

		Property<T> getProperty<T>(string propertyName);

		void setGroupBonuses(Dictionary<string, object> bonusDict);

		string Name { get; }
		Team Team { get; set; }
		bool Visible { get; }
		Mogre.Vector3 Direction { get; }
		Mogre.Vector3 Position { get; }

		int AttackPower { get; }
		int DeffPower { get; }
		int Hp { get; }

		ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitObject, bool isFriendly, bool isMovableGameObject);
		Dictionary<string, object> onGroupAdd();
		Dictionary<string, object> getPropertyToDisplay();
	}
}
