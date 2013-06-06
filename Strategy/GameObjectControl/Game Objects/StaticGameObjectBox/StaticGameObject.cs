using System;
using System.Collections.Generic;
using Strategy.TeamControl;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameMaterial;
using Mogre;
using System.Reflection;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.Exceptions;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	public abstract class StaticGameObject : GameObject, IStaticGameObject {

		protected const int startHP = 1000;

		public StaticGameObject() {
			isVisible = false;
			propertyDict = new Dictionary<PropertyEnum, object>();
			userDefinedPropertyDict = new Dictionary<string, object>();
			propertyDict.Add(PropertyEnum.Hp, new Property<int>(startHP));
		}


		/// <summary>
		/// Rotates SceneNode
		/// </summary>
		/// <param Name="delay">Deley between last two frames</param>
		public virtual void Rotate(float delay) {
			Update(delay);
			sceneNode.Roll(new Mogre.Degree(50 * delay));
		}

		/// <summary>
		/// Does not move with object, just updates gameActions.
		/// </summary>
		/// <param Name="delay">Deley of frames</param>
		public virtual void NonActiveRotate(float delay) {
			Update(delay);
		}


		/// <summary>
		/// Called when object will be invisible
		/// </summary>
		public override void ChangeVisible(bool visible) {
			base.ChangeVisible(visible);
			if (visible) {
				sceneNode.Pitch(new Degree(-90f));
			}
		}

		public override float PickUpDistance {
			get { return 150; }
		}

		public override float OccupyDistance {
			get { return 0; }
		}

		public override int OccupyTime {
			get { return 20; }
		}


		public override int Hp {
			get { return ((Property<int>)propertyDict[PropertyEnum.Hp]).Value; }
		}

		protected void setHp(int hp) {
			((Property<int>)propertyDict[PropertyEnum.Hp]).Value = hp;
		}

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
