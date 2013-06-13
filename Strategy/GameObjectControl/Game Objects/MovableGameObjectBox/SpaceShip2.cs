using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;
using Strategy.TeamControl;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.GameObjectControl.Game_Objects.Bullet;

namespace Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox {
	/// <summary>
	/// Movable object with more powerful bullet than SpaceShip (Missile2).
	/// Gives speed bonus for a each group member but it can not decide alone
	/// (alwayes returns None answer on mouse action -> needs leader ship).
	/// </summary>
	public class SpaceShip2 : MovableGameObject {

		private const string meshConst = "SpaceShip1.mesh";

		/// <summary>
		/// Initializes the object from given arguments (1-2 members second argument is the Hp).
		/// Loads runtime properties from PropertyManager (Speed, Deffence and UserDefinedProperty). 
		/// </summary>
		/// <param name="name">The name of the creating object.</param>
		/// <param name="myTeam">The object team.</param>
		/// <param name="args">The argouments should contains one or two members (second is Hp).</param>
		public SpaceShip2(string name, Team myTeam, object[] args) {
			this.name = name;
			this.mesh = meshConst;
			this.team = myTeam;

			this.position = new Property<Vector3>(ParseStringToVector3((string)args[0]));

			if (args.Count() == 2) {
				setHp(Convert.ToInt32(args[1]));
			} 

			SetProperty(PropertyEnum.Position, this.position);
			SetProperty(PropertyEnum.Speed, Game.PropertyManager.GetProperty<float>("speed2"));
			SetProperty(PropertyEnum.Deffence, Game.PropertyManager.GetProperty<int>("basicDeff"));
			SetProperty("UserDefinedProperty", new Property<string>("This is UserDefinedProperty"));

			//Mogre inicialization of object
			entity = Game.SceneManager.CreateEntity(name, mesh);
		}

		/// <summary>
		/// Doesn't react on a mouse action (reacts if is the in a group with a more powerful member - can order to move/attack..).
		/// </summary>
		/// <param name="point">The mouse position.</param>
		/// <param name="hitObject">The result of a HitTest.</param>
		/// <param name="isFriendly">The information if the hitted object is friendly.</param>
		/// <param name="isMovableGameObject">The information if the hitted object is movable.</param>
		/// <returns>Returns None.</returns>
		public override ActionAnswer OnMouseAction(Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {
			return ActionAnswer.None;
		}

		/// <summary>
		/// Does nothing on display.
		/// </summary>
		protected override void OnDisplayed() {}

		/// <summary>
		/// Gets speed bonus for each member of object group.
		/// </summary>
		/// <returns>Returns the dictionary with speed bonus (200).</returns>
		public override Dictionary<string, object> OnGroupAdd() {
			var dict = new Dictionary<string, object>();
			dict.Add(PropertyEnum.Speed.ToString(), new Property<float>(200));
			return dict;
		}

		/// <summary>
		/// Returns a type of a current acquired bullet (Missile2).
		/// </summary>
		/// <returns>Return a type of a current acquired bullet.</returns>
		protected override Type GetIBulletType() {
			return typeof(Missile2);
		}

		/// <summary>
		/// Return a maximum attack distance of a current acquired bullet.
		/// </summary>
		/// <returns>Return a maximum attack distance of a current acquired bullet.</returns>
		protected override int GetIBulletAttackDistance() {
			return Missile2.AttackDistance;
		}

		/// <summary>
		/// Return a cooldown of a current acquired bullet.
		/// </summary>
		/// <returns>Return a cooldown of a current acquired bullet.</returns>
		protected override TimeSpan GetIBulletAttackCooldown() {
			return Missile2.Cooldown;
		}

		/// <summary>
		/// Creates and registers a new bullet (a current acquired bullet).
		/// </summary>
		/// <returns>Returns Missile2 instance.</returns>
		protected override IBullet CreateIBullet() {
			var solS = Game.GroupManager.GetSolarSystem(this);
			return new Missile2(position.Value, solS, target.Value.Position, fight);
		}
	}
}
