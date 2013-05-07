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

		protected static Dictionary<string, IGameAction> gameActions;
		protected static Dictionary<string, List<IStaticGameObject>> gameActionsPermitions;


		//Look here create file Load file
		static StaticGameObject() {
			gameActionsPermitions = new Dictionary<string, List<IStaticGameObject>>();
			gameActions = new Dictionary<string, IGameAction>();
			IGameAction o = (IGameAction)Assembly.GetExecutingAssembly().CreateInstance("Strategy.GameObjectControl.Game_Objects.GameActions.Produce"); //TODO delete

			gameActions.Add(o.Name, o);
			gameActionsPermitions.Add(o.Name, new List<IStaticGameObject>());
		}

		public void registerExecuter(string nameOfAction, Dictionary<string, IMaterial> materials, string material) {
			if (gameActionsPermitions.ContainsKey(nameOfAction)) {
				gameActionsPermitions[nameOfAction].Add(this);
			}
			registerProducer(materials[material], 0.01);
		}

		private void registerProducer(IMaterial specificType, double value) {
			((Produce)gameActions["Produce"]).RegisterExecuter(this, specificType, value);
		}


		/// <summary>
		/// Rotating function 
		/// </summary>
		/// <param Name="f">Deley of frames</param>
		public virtual void Rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(50 * f));
		}

		/// <summary>
		/// StaticGameObject doesn't move in non-active mode but child can override.
		/// </summary>
		/// <param Name="f">Deley of frames</param>
		public virtual void NonActiveRotate(float f) {
		}


		public bool TryExecute(string executingAction) {
			if (gameActionsPermitions.ContainsKey(executingAction) && gameActionsPermitions[executingAction].Contains(this)) {
				gameActions[executingAction].Execute(this, Team);
				return true;
			}
			return false;
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

	}
}
