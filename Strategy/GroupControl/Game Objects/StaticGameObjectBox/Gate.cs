using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.GroupControl.Game_Objects.GameActions;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	class Gate : IStaticGameObject {

		protected string name;
		protected Mogre.Entity entity;
		protected Mogre.SceneNode sceneNode;
		protected string mesh;
		protected Mogre.SceneManager manager;

		private Vector3 position;
		private AnimationState animationState; //The AnimationState the moving object

		protected static Team gateTeam = new Team("Gates");

		public Gate(string name, string mesh, SceneManager manager, Vector3 position) {
			this.name = name;
			this.mesh = mesh;
			this.manager = manager;
			this.position = position;
			entity = manager.CreateEntity(name, mesh);
			changeVisible(true);//TODO remove

			animationState = entity.GetAnimationState("funcionando3_eani_Clip");
			animationState.Loop = true;
			animationState.Enabled = true;
		}

		public void rotate(float f) {
			//sceneNode.Yaw(new Degree(5 * f));

			//animationState = entity.GetAnimationState("abrirse_eani_Clip");
			animationState = entity.GetAnimationState("funcionando3_eani_Clip");
			animationState.Loop = true;
			animationState.Enabled = true;
			animationState.AddTime(f/10);
		}

		public void nonActiveRotate(float f) {}

		public void changeVisible(bool visible) {
			if (visible) {
				if (entity == null) {
					entity = manager.CreateEntity(name, mesh);
				}
				sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);

				sceneNode.Pitch(new Degree(-90f));
				sceneNode.AttachObject(entity);
			} else {
				manager.DestroySceneNode(sceneNode);
			}
		}

		public string Name {
			get { return name; }
		}

		public string Mesh {
			get { return mesh; }
		}

		public bool tryExecute(string executingAction) {
			throw new NotImplementedException();
		}

		public TeamControl.Team Team {
			get {
				return gateTeam;
			}
			set {
				gateTeam= value;
			}
		}
	}
}
