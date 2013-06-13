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
	/// <summary>
	/// Representig basic game movable object. Can attack movable targets and occupy static targets. Inhertis from
	/// the MovableGameObject (just override few methods).
	/// </summary>
	public class SpaceShip : MovableGameObject {

		private const string meshConst = "SpaceShip2.mesh";

		/// <summary>
		/// Initializes the object from given arguments (1-2 members second argument is the Hp).
		/// Loads runtime properties from PropertyManager (Speed, Deffence). 
		/// </summary>
		/// <param name="name">The name of the creating object.</param>
		/// <param name="myTeam">The object's team.</param>
		/// <param name="args">The argouments should contains one or two members (second is Hp).</param>
		public SpaceShip(string name, Team myTeam, object[] args) {
			this.name = name;
			this.mesh = meshConst;
			this.team = myTeam;

			this.position = new Property<Vector3>(ParseStringToVector3((string)args[0]));
			if (args.Count() == 2) {
				setHp(Convert.ToInt32(args[1]));
			} 
			Console.WriteLine(position.Value);
			base.SetProperty(PropertyEnum.Position, this.position);
			base.SetProperty(PropertyEnum.Speed, Game.PropertyManager.GetProperty<float>("speed"));
			base.SetProperty(PropertyEnum.Deffence, Game.PropertyManager.GetProperty<int>("basicDeff"));

			//Mogre inicialization of object
			entity = Game.SceneManager.CreateEntity(name, mesh);
		}

		/// <summary>
		/// Overrides function and reacts on mouse action. If the hitted is friendly so the object moves to target else 
		/// attack movable/ occupy static.
		/// </summary>
		/// <param name="point">The mouse position.</param>
		/// <param name="hitObject">The result of a HitTest.</param>
		/// <param name="isFriendly">The information if the hitted object is friendly.</param>
		/// <param name="isMovableGameObject">The information if the hitted object is movable.</param>
		/// <returns>Returns a required action.</returns>
		public override ActionAnswer OnMouseAction(Vector3 point, MovableObject hitTarget, bool isFriendly, bool isMovableGameObject) {

			if (hitTarget != null) {
				if (isFriendly) {
					return ActionAnswer.MoveTo;
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

		/// <summary>
		/// Does nothing on display.
		/// </summary>
		protected override void OnDisplayed() {}

		/// <summary>
		/// Returns a deffence property's value.
		/// </summary>
		public override int DeffPower {
			get { return GetPropertyValue<int>(PropertyEnum.Deffence); }
		}
	}
}
