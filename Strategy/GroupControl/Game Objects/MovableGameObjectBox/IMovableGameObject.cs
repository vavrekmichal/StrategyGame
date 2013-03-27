using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	public interface IMovableGameObject {
		void move(float f);
		void nonActiveMove(float f);
		void changeVisible(bool visible);
		void shout();
		void addNextLocation(Vector3 placeToGo);
		void addNextLocation(LinkedList<Vector3> positionList);
		void setNextLocation(Vector3 placeToGo);
		void setNextLocation(LinkedList<Vector3> positionList);
		void jumpNextLocation(Vector3 placeToGo);

        string Name{get;set;}
		Team Team { get; set; }
		Mogre.Vector3 Direction { get; }
		Mogre.Vector3 Position {get;}

		ActionAnswer onMouseAction(ActionFlag reason, Vector3 point, object hitTestResult);
	}
}
