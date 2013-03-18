using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	public interface IMovableGameObject {
		void move(float f);
		void nonActiveMove(float f);
		void shout();

        string Name{get;set;}
		Team Team { get; set; }
	}
}
