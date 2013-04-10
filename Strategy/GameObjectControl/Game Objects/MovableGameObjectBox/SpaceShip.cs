using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	public class SpaceShip : MovableGameObject {


		public SpaceShip(string name, string mesh, Team myTeam, Mogre.SceneManager manager, Vector3 position, PropertyManager propMgr) {
			this.name = name;
			this.mesh = mesh;
			this.movableObjectTeam = myTeam;
			this.manager = manager;
			this.position = position;
			base.setProperty("speed", propMgr.getProperty<float>("speed"));
			base.setProperty("attack", propMgr.getProperty<int>("basicAttack"));
			base.setProperty("deffence", propMgr.getProperty<int>("basicDeff"));
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
			get { return getProperty<int>("attack").Value * getPropertyValueFromBonusDict<int>("attack"); }
		}

		public override int DeffPower {
			get { return getProperty<int>("deffence").Value; }
		}
	}
}
