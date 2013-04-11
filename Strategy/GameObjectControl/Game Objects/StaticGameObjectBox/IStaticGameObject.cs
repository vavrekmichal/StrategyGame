using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	public interface IStaticGameObject {
		void rotate(float f);
        void nonActiveRotate(float f);
        void changeVisible(bool visible);
		string Name { get; }
		string Mesh { get; }
        bool tryExecute(string executingAction);

        Team Team { get; set; }
		float PickUpDistance { get; }
		Vector3 Position { get; }

		ActionReaction reactToInitiative(ActionReason reason, IMovableGameObject target);
		Dictionary<string, object> getPropertyToDisplay();
	}
}
