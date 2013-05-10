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
			this.team = myTeam;
			this.manager = manager;
			this.position = position;
			base.SetProperty(PropertyEnum.Speed, propMgr.GetProperty<float>("speed"));
			base.SetProperty(PropertyEnum.Attack, propMgr.GetProperty<int>("basicAttack"));
			base.SetProperty(PropertyEnum.Deffence, propMgr.GetProperty<int>("basicDeff"));
			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
		}

		public override ActionAnswer OnMouseAction(Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {

			if (hitTarget != null) {
				if (isFriendly) {
					if (isMovableGameObject) {
						return ActionAnswer.MoveTo;
					} else {
						return ActionAnswer.MoveTo;
					}

				} else {
					if (isMovableGameObject) {
						return ActionAnswer.Attack;
					} else {
						return ActionAnswer.Occupy;
					}
				}
			}

			return ActionAnswer.Move;
		}

		protected override void OnDisplayed() {

		}

		public override int AttackPower {
			get { return GetPropertyValue<int>(PropertyEnum.Attack); }
		}

		public override int DeffPower {
			get { return GetPropertyValue<int>(PropertyEnum.Deffence); }
		}
	}
}
