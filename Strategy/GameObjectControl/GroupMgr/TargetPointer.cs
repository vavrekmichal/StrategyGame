using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.GroupMgr {
	/// <summary>
	/// Indicates current targeted objects. Creates a object above the targeted object
	/// which indicates that the object is targeted.
	/// </summary>
	class TargetPointer {

		private SceneNode node;
		private Entity entity;
		private Property<Vector3> position;
		private IGameObject gameObject;

		const string mesh = "pointer.mesh";
		const string typeName = "Pointer";
		readonly Vector3 liftingConst = new Vector3(0, 100, 0);

		/// <summary>
		/// Initializes TargetPointer (gets unused name and creates SceneNode and Entity).
		/// Alse stores reference to targeted object.
		/// </summary>
		/// <param name="gameObject">The targeted object.</param>
		public TargetPointer(IGameObject gameObject) {

			this.gameObject = gameObject;
			this.position = gameObject.GetProperty<Vector3>(PropertyEnum.Position);
			string name = Game.IGameObjectCreator.GetUnusedName(typeName);
			entity = Game.SceneManager.CreateEntity(name, mesh);

			node = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", position.Value + liftingConst);
			node.Pitch(new Degree(180));
			node.AttachObject(entity);
		}

		/// <summary>
		/// Updates pointer and checks if the targeted object is still alive.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			if (gameObject.Hp > 0) {
				node.Position = position.Value + liftingConst;
				node.Yaw(new Mogre.Degree(150 * delay));
			} else {
				Destroy();
			}
		}

		/// <summary>
		/// Destroys pointer ( destroy SceneNode and Entity).
		/// </summary>
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
