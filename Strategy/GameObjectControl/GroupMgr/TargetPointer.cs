using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.GroupMgr {
	class TargetPointer {

		private SceneNode node;
		private Entity entity;
		private Property<Vector3> position;
		private IGameObject gameObject;

		const string mesh = "pointer.mesh";
		const string typeName = "Pointer";
		readonly Vector3 liftingConst = new Vector3(0, 100, 0);

		public TargetPointer(IGameObject gameObject) {
			//Property<Vector3> position
			this.gameObject = gameObject;
			this.position = gameObject.GetProperty<Vector3>(PropertyEnum.Position);
			string name = Game.IGameObjectCreator.GetUnusedName(typeName);
			entity = Game.SceneManager.CreateEntity(name, mesh);

			node = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", position.Value + liftingConst);
			node.Pitch(new Degree(180));
			node.AttachObject(entity);
		}

		public void Update(float delay) {
			if (gameObject.Hp > 0) {
				node.Position = position.Value + liftingConst;
				node.Yaw(new Mogre.Degree(150 * delay));
			} else {
				Destroy();
			}
		}

		public void Destroy() {
			if (node != null) {
				Game.SceneManager.DestroySceneNode(node);
				node = null;
			}
			if (entity != null) {
				Game.SceneManager.DestroyEntity(entity);
				entity = null;
			}
		}


	}
}
