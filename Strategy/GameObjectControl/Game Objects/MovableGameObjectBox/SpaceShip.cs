using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	public class SpaceShip : MovableGameObject {


		public SpaceShip(string name, string mesh, Team myTeam, Mogre.SceneManager manager, Vector3 position, PropertyManager propMgr) {
			this.name = name;
			this.mesh = mesh;
			this.movableObjectTeam = myTeam;
			this.manager = manager;
			this.position = position;
			base.setProperty(PropertyEnum.Speed, propMgr.getProperty<float>("speed"));
			base.setProperty(PropertyEnum.Attack, propMgr.getProperty<int>("basicAttack"));
			base.setProperty(PropertyEnum.Deffence, propMgr.getProperty<int>("basicDeff"));
			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
		}

		public override ActionAnswer onMouseAction(ActionReason reason, Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {
			switch (reason) {
				case ActionReason.onRightButtonClick:
					if (hitTarget != null) {
						if (isFriendly) {
							if (isMovableGameObject) {
								return ActionAnswer.MoveTo;
							} else {
								return ActionAnswer.MoveTo;
							}

						} else {
							return ActionAnswer.Attack;
						}
					}
					break;

				default:
					if (isFriendly) {
						return ActionAnswer.Move;
					}
					break;
			}
			return ActionAnswer.Move;
		}

		protected override void onDisplayed() {

		}

		public override int AttackPower {
			get { return getPropertyValue<int>(PropertyEnum.Attack); }
		}

		public override int DeffPower {
			get { return getPropertyValue<int>(PropertyEnum.Deffence); }
		}
	}
}
