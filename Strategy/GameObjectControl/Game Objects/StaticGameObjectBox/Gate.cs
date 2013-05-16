using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mogre;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects.GameActions;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	public class Gate : StaticGameObject {

		private bool isPorting;
		private const float portTime = 3.4483f;
		private float portTimeDuration;

		private AnimationState animationState; // The AnimationState the moving object

		protected static List<Traveler> travelerList = new List<Traveler>();

		protected static Team gateTeam;

		public Gate(string name, string mesh, Vector3 position, Team team) {
			this.name = name;
			this.mesh = mesh;
			this.position = position;
			this.team = team;
			entity = Game.SceneManager.CreateEntity(name, mesh);

			animationState = entity.GetAnimationState("funcionando3_eani_Clip");
			animationState.Loop = true;
			animationState.Enabled = true;
		}

		public override void Rotate(float f) {
			if (isPorting) {
				animationState = entity.GetAnimationState("abrirse_eani_Clip");
				f *= 5;
				portTimeDuration -= f / 10;
				if (portTimeDuration < 0) {
					isPorting = false;
				}
			} else {
				animationState = entity.GetAnimationState("funcionando3_eani_Clip");
			}
			animationState.Loop = true;
			animationState.Enabled = true;
			animationState.AddTime(f / 10);
		}

		public override void NonActiveRotate(float f) { }

		public override void ChangeVisible(bool visible) {
			if (visible) {
				if (sceneNode == null) {
					if (entity == null) {
						entity = Game.SceneManager.CreateEntity(name, mesh);
					}
					sceneNode = Game.SceneManager.RootSceneNode.CreateChildSceneNode(name + "Node", position);

					sceneNode.Pitch(new Degree(-90f));
					sceneNode.AttachObject(entity);
				}
			} else {
				if (sceneNode != null) {
					Game.SceneManager.DestroySceneNode(sceneNode);
					sceneNode = null;
				}
			}
		}

		public override ActionReaction ReactToInitiative(ActionReason reason, IMovableGameObject target) {
			ShowTravelDestinations(target);
			return ActionReaction.None;
		}

		protected override void OnDisplayed() {

		}

		public void ShowTravelDestinations(IMovableGameObject imgo) {
			var gui = Game.GUIManager;
			var groupMgr = Game.GroupManager;
			gui.ShowSolarSystSelectionPanel(groupMgr.GetAllSolarSystemNames(), "Choose where you'll travel", imgo);

			isPorting = true;
			portTimeDuration = portTime;
		}


		public static void UpdateTravelers(float delay) {
			List<Traveler> copy = new List<Traveler>(travelerList);
			foreach (var traveler in copy) {
				if (traveler.IsDone) {
					travelerList.Remove(traveler);

				} else {
					traveler.Update(delay);
				}
			}

		}

		public static List<Traveler> GetTravelers() {
			return travelerList;
		}

		public static void CreateTraveler(SolarSystem from, SolarSystem to, object gameObject) {
			if (gameObject is IMovableGameObject) {
				if (from != to) {
					IMovableGameObject imgo = (IMovableGameObject)gameObject;
					travelerList.Add(new Traveler(from, to, (IMovableGameObject)imgo));
				}
			}
		}
	}
}
