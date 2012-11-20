using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	abstract class StaticGameObject : IStaticGameObject {
		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction = new List<IGameAction> ();
		protected Mogre.Vector3 position;
		protected int team;
		protected int planetSystem;

		public abstract void rotate();

		public abstract void produce(float f);


	}
}
