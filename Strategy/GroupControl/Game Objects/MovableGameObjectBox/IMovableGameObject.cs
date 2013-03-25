using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	public interface IMovableGameObject {
		void move(float f);
		void nonActiveMove(float f);
		void changeVisible(bool visible);
		void shout();
		void addNextLocation(Mogre.Vector3 placeToGo);
		void addNextLocation(LinkedList<Mogre.Vector3> positionList);
		void setNextLocation(LinkedList<Mogre.Vector3> positionList);

        string Name{get;set;}
		Team Team { get; set; }
		Mogre.Vector3 Direction { get; }
		Mogre.Vector3 Position {get;}
	}
}
