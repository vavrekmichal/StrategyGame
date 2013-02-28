﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	interface IStaticGameObject {
		void rotate(float f);
        void nonActiveRotate(float f);
        void changeVisible(bool visible);
        string getName();
		string getMesh();
        bool tryExecute(string executingAction);

        Team Team { get; set; }

	}
}
