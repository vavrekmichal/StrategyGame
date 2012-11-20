using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	class Planet : StaticGameObject {
		public Planet(string name, string mesh,int team, int planetSystem, Mogre.Vector3 position, Mogre.SceneManager manager) {
			this.name = name;
			this.position = position;
			this.mesh = mesh;
			this.team = team;
			this.planetSystem = planetSystem;
			entity = manager.CreateEntity(name, mesh);
			sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);
			sceneNode.AttachObject(entity);
		}

		public override void rotate() {
			throw new NotImplementedException();
		}

		public override void produce(float f) {
			throw new NotImplementedException();
		}

	}
}
