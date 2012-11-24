using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	class Sun : IStaticGameObject {
		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected List<IGameAction> listOfAction = new List<IGameAction>();
		protected Mogre.Vector3 position;

		public Sun(string name, string mesh,int planetSystem, Mogre.Vector3 position, Mogre.SceneManager manager) {
			this.name = name;
			this.position = position;
			this.mesh = mesh;
			entity = manager.CreateEntity(name, mesh);
			sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
			sceneNode.AttachObject(entity);
			sceneNode.Pitch(new Mogre.Degree(-90f));
		}

		public void rotate(float f) {
			sceneNode.Roll(new Mogre.Degree(50 *f));
		}

	}
}
