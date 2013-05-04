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

		public SpaceShip2(string name, string mesh, Team myTeam, Mogre.SceneManager manager, Vector3 position, PropertyManager propMgr) {
			this.name = name;
			this.mesh = mesh;
			this.movableObjectTeam = myTeam;
			this.manager = manager;
			this.position = position;
			setProperty(PropertyEnum.Speed, propMgr.getProperty<float>("speed2"));
			setProperty(PropertyEnum.Attack, propMgr.getProperty<int>("basicAttack"));
			setProperty(PropertyEnum.Deffence, propMgr.getProperty<int>("basicDeff"));
			setProperty("Hovno", new Property<string>("Toto je hovno"));

			//Mogre inicialization of object
			entity = manager.CreateEntity(name, mesh);
		}

		public override ActionAnswer onMouseAction(Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {
			return ActionAnswer.None;
		}

		protected override void onDisplayed() {

		}

		public override Dictionary<string, object> onGroupAdd() {
			var dict = new Dictionary<string, object>();
			dict.Add(PropertyEnum.Speed.ToString(), new Property<float>(200));
			return dict;
		}
	}
}
