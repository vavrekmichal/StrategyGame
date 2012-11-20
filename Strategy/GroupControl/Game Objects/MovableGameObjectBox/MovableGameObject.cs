using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.Game_Objects.MovableGameObjectBox {
	abstract class MovableGameObject : IMovableGameObject{
		/*protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction;
		protected Mogre.Vector3 position;
		*/
		public abstract void move(float f);

		public abstract void shout();
	}
}
