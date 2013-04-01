using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	public interface IStaticGameObject {
		void rotate(float f);
        void nonActiveRotate(float f);
        void changeVisible(bool visible);
		string Name { get; }
		string Mesh { get; }
        bool tryExecute(string executingAction);

        Team Team { get; set; }

		ActionAnswer onMouseAction(ActionReason reason, Vector3 point, object hitTestResult);

	}
}
