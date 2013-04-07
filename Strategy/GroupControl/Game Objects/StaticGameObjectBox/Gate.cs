using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.GroupControl.Game_Objects.GameActions;
using Strategy.GroupControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	public class Gate : StaticGameObject {


		private Vector3 position;
		private AnimationState animationState; //The AnimationState the moving object

		protected static List<Traveler> travelerList = new List<Traveler>();

		protected static Team gateTeam;

		public Gate(string name, string mesh, SceneManager manager, Vector3 position, Team team) {
			this.name = name;
			this.mesh = mesh;
			this.manager = manager;
			this.position = position;
			this.Team = team;
			entity = manager.CreateEntity(name, mesh);

			animationState = entity.GetAnimationState("funcionando3_eani_Clip");
			animationState.Loop = true;
			animationState.Enabled = true;
		}

		public override void rotate(float f) {
			//animationState = entity.GetAnimationState("abrirse_eani_Clip");
			animationState = entity.GetAnimationState("funcionando3_eani_Clip");
			animationState.Loop = true;
			animationState.Enabled = true;
			animationState.AddTime(f/10);
		}

		public override void nonActiveRotate(float f) { }

		public override void changeVisible(bool visible) {
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

		public override ActionAnswer onMouseAction(ActionReason reason, Vector3 point, object hitTestResult) {
			return ActionAnswer.None;
		}

		protected override void onDisplayed() {
			
		}

		public static void updateTravelers(float delay){
			foreach (var traveler in travelerList) {
				traveler.update(delay);
			}
		}

		public static List<Traveler> getTravelers() {
			return travelerList;
		}
	}
}
