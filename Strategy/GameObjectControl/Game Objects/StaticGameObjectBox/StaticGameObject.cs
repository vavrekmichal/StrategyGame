using System.Collections.Generic;
using Mogre;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	/// <summary>
	/// Implements all IStaticGameObject functions. Designed to facilitate the implementation of game objects.
	/// </summary>
	public abstract class StaticGameObject : GameObject, IStaticGameObject {

		protected const int startHP = 1000;

		/// <summary>
		/// Sets start Hp and sets object to invisible mode (SolarSystem can switch to visible).
		/// </summary>
		public StaticGameObject() {
			isVisible = false;
			propertyDict = new Dictionary<PropertyEnum, object>();
			userDefinedPropertyDict = new Dictionary<string, object>();
			propertyDict.Add(PropertyEnum.Hp, new Property<int>(startHP));
		}


		/// <summary>
		/// Rotates (Roll) with object SceneNode and calls Update function.
		/// </summary>
		/// <param Name="delay">The deley between last two frames.</param>
		public virtual void Rotate(float delay) {
			Update(delay);
			sceneNode.Roll(new Mogre.Degree(50 * delay));
		}

		/// <summary>
		/// Does not move with object, just calls Update.
		/// </summary>
		/// <param Name="delay">The deley between last two frames.</param>
		public virtual void NonActiveRotate(float delay) {
			Update(delay);
		}

		/// <summary>
		/// (true) Initializes SceneNode and Entity (base.ChangeVisible) and sets correct SceneNode orientation (Pitch)
		/// (false) Destroys SceneNode (base.ChangeVisible).
		/// </summary>
		public override void ChangeVisible(bool visible) {
			base.ChangeVisible(visible);
			if (visible) {
				sceneNode.Pitch(new Degree(-90f));
			}
		}

		/// <summary>
		/// Returns the distance, which is necessary for meet with the object.
		/// </summary>
		public override float PickUpDistance {
			get { return 150; }
		}

		/// <summary>
		/// Returns the distance, which is necessary for a occupation. In this case the object cannot be occupied.
		/// (object should override this function).
		/// </summary>
		public override float OccupyDistance {
			get { return 0; }
		}

		/// <summary>
		/// Returns the time, which is necessary for a successful occupation (20s).
		/// </summary>
		public override int OccupyTime {
			get { return 20; }
		}

		/// <summary>
		/// Returns a current state of health.
		/// </summary>
		public override int Hp {
			get { return ((Property<int>)propertyDict[PropertyEnum.Hp]).Value; }
		}

		/// <summary>
		/// Sets the state of health.
		/// </summary>
		/// <param name="hp">The new state of health.</param>
		protected void setHp(int hp) {
			((Property<int>)propertyDict[PropertyEnum.Hp]).Value = hp;
		}

		/// <summary>
		/// Subtracts receives damage from current Hp. If the health is lower then 0 so RaiseDie is called.
		/// </summary>
		/// <param name="damage">The taken damage.</param>
		public override void TakeDamage(int damage) {
			var hpProp = (Property<int>)propertyDict[PropertyEnum.Hp];
			var actualHp = hpProp.Value - damage;
			hpProp.Value = actualHp;
			if (actualHp < 0) {
				RaiseDie();
			}
		}
	}
}
