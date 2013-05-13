using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;

namespace Strategy.GameObjectControl.Game_Objects.Bullet {
	class Missile : IBullet {

		protected SceneNode sceneNode;
		protected Entity entity;
		protected Vector3 position;


		private SceneManager sceneMgr;
		private const string mesh = "missile.mesh";
		private string name;

		public Missile(SceneManager sceneMgr, Vector3 position, string name) {
			this.position = position;
			this.sceneMgr = sceneMgr;
			this.name = name;

			ChangeVisible(true);
		}

		public int Attack {
			get { return 2; }
		}

		public string Name {
			get {
				return name;
			}
		}

		/// <summary>
		/// Called when visibility is changed. Visible is true -> Function creates SceneNode and checks Entity (if is null -> initializes.
		/// Visible is false -> Destroy SceneNode and save actual position.
		/// </summary>
		public virtual void ChangeVisible(bool visible) {
			if (visible && sceneNode == null) {
				// Control if the entity is inicialized
				if (entity == null) {
					entity = sceneMgr.CreateEntity(name, mesh);
				}

				sceneNode = sceneMgr.RootSceneNode.CreateChildSceneNode(name + "Node", position);
				sceneNode.AttachObject(entity);
			} else {
				if (sceneNode != null) {
					position = sceneNode.Position;
					sceneMgr.DestroySceneNode(sceneNode);
					sceneNode = null;
				}

			}
		}


		public void Update(float delay) {
			throw new NotImplementedException();
		}


		public void HiddenUpdate(float delay) {
			throw new NotImplementedException();
		}
	}
}
