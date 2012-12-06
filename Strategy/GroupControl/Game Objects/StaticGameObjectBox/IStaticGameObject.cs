using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	interface IStaticGameObject {
		void rotate(float f);
        void nonActiveRotate(float f);
        void changeVisible(bool visible);
        string getName();

        //int team { get; set; }
        string team { get; set; }
        
        //int getSolarSystem { get; }
	}
}
