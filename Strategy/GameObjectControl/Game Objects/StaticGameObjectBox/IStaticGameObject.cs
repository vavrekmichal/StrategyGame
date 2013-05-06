using System.Collections.Generic;
using Mogre;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	public interface IStaticGameObject : IGameObject {
		void rotate(float f);
        void nonActiveRotate(float f);

		
        bool tryExecute(string executingAction);

	}
}
