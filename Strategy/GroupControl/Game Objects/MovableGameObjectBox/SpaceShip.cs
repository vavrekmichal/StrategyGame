using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GroupControl;
using Strategy.GroupControl.RuntimeProperty;

namespace Strategy.GroupControl.Game_Objects.MovableGameObjectBox {
	public class SpaceShip : MovableGameObject {

		private Property<int> attack;
		private Property<int> deff;

		public SpaceShip(string name, string mesh, Team myTeam, Mogre.SceneManager manager, Vector3 position, PropertyManager propMgr) {
			this.name = name;
			this.mesh = mesh;
			this.movableObjectTeam = myTeam;
			this.manager = manager;
			this.position = position;
			this.flySpeed = propMgr.getProperty<float>("speed");
			this.attack = propMgr.getProperty<int>("basicAttack");
			this.deff = propMgr.getProperty<int>("basicDeff");

			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
		}

		public override ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {
			switch (reason) {
				case ActionReason.onRightButtonClick:
					if (hitTarget != null && isFriendly) {
						if (isMovableGameObject) {
							return ActionAnswer.Move;
						} else {
							return ActionAnswer.MoveTo;
						}

					} else {
						return ActionAnswer.Move;
					}
					break;

				default:
					if (isFriendly) {
						return ActionAnswer.Move;
					} 
					break;
			}
			return ActionAnswer.None;
		}

		protected override void onDisplayed() {

		}

		public override int AttackPower {
			get { return attack.Value; }
		}

		public override int DeffPower {
			get { return deff.Value; }
		}
	}
}
