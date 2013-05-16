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
	public class SpaceShip2 : MovableGameObject {

		public SpaceShip2(string name, string mesh, Team myTeam, Vector3 position) {
			this.name = name;
			this.mesh = mesh;
			this.team = myTeam;

			this.position = position;
			SetProperty(PropertyEnum.Speed, Game.PropertyManager.GetProperty<float>("speed2"));
			SetProperty(PropertyEnum.Attack, Game.PropertyManager.GetProperty<int>("basicAttack"));
			SetProperty(PropertyEnum.Deffence, Game.PropertyManager.GetProperty<int>("basicDeff"));
			SetProperty("Hovno", new Property<string>("Toto je hovno"));

			//Mogre inicialization of object
			entity = Game.SceneManager.CreateEntity(name, mesh);
		}

		public override ActionAnswer OnMouseAction(Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {
			return ActionAnswer.None;
		}

		protected override void OnDisplayed() {

		}

		public override Dictionary<string, object> OnGroupAdd() {
			var dict = new Dictionary<string, object>();
			dict.Add(PropertyEnum.Speed.ToString(), new Property<float>(200));
			return dict;
		}
	}
}
