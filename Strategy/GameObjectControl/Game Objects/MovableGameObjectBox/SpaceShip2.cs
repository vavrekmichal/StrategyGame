using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	public class SpaceShip2 : MovableGameObject {

		private const string meshConst = "SpaceShip1.mesh";

		public SpaceShip2(string name, Team myTeam, object[] args) {
			this.name = name;
			this.mesh = meshConst;
			this.team = myTeam;

			this.position = new Property<Vector3>(ParseStringToVector3((string)args[0]));

			SetProperty(PropertyEnum.Position, this.position);
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

		protected override Type GetIBulletType() {
			return typeof(Missile2);
		}

		protected override int GetIBulletAttackDistance() {
			return Missile2.AttackDistance;
		}

		protected override TimeSpan GetIBulletAttackCooldown() {
			return Missile2.Cooldown;
		}

		protected override IBullet CreateIBullet() {
			var solS = Game.GroupManager.GetSolarSystem(this);
			return new Missile2(position.Value, solS, target.Value.Position, fight);
		}
	}
}
