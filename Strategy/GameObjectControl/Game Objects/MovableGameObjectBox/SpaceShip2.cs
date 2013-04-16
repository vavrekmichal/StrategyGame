using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	public class SpaceShip2 : MovableGameObject {

		public SpaceShip2(string name, string mesh, Team myTeam, Mogre.SceneManager manager, Vector3 position, PropertyManager propMgr) {
			this.name = name;
			this.mesh = mesh;
			this.movableObjectTeam = myTeam;
			this.manager = manager;
			this.position = position;
			setProperty("Speed", propMgr.getProperty<float>("speed2"));
			setProperty("Attack", propMgr.getProperty<int>("basicAttack"));
			setProperty("Deffence", propMgr.getProperty<int>("basicDeff"));

			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
		}

		public override ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {
			return ActionAnswer.None;
		}

		protected override void onDisplayed() {

		}
	}
}
