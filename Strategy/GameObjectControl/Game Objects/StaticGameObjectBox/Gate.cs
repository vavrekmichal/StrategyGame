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

		private Vector3 position;
		private AnimationState animationState; // The AnimationState the moving object

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

		public override void nonActiveRotate(float f) { }

		public override void changeVisible(bool visible) {
			if (visible) {
				if (sceneNode == null) {
					if (entity == null) {
						entity = manager.CreateEntity(name, mesh);
					}
					sceneNode = manager.RootSceneNode.CreateChildSceneNode(name + "Node", position);

					sceneNode.Pitch(new Degree(-90f));
					sceneNode.AttachObject(entity);
				}
			} else {
				if (sceneNode != null) {
					manager.DestroySceneNode(sceneNode);
					sceneNode = null;
				}
			}
		}

		public override ActionReaction reactToInitiative(ActionReason reason, IMovableGameObject target) {
			showTravelDestinations(target);
			return ActionReaction.None;
		}

		protected override void onDisplayed() {

		}

		public void showTravelDestinations(IMovableGameObject imgo) {
			var gui = GameGUI.GUIControler.getInstance();
			var groupMgr = GroupManager.getInstance();
			gui.showSolarSystSelectionPanel(groupMgr.getAllSolarSystemNames(), "Choose where you'll travel", imgo);

			isPorting = true;
			portTimeDuration = portTime;
		}

		public override float PickUpDistance {
			get {
				return 200;
			}
		}

		public static void updateTravelers(float delay) {
			List<Traveler> copy = new List<Traveler>(travelerList);
			foreach (var traveler in copy) {
				if (traveler.isDone) {
					travelerList.Remove(traveler);

				} else {
					traveler.update(delay);
				}
			}

		}

		public static List<Traveler> getTravelers() {
			return travelerList;
		}

		public static void createTraveler(SolarSystem from, SolarSystem to, object gameObject) {
			if (gameObject is IMovableGameObject) {
				if (from != to) {
					IMovableGameObject imgo = (IMovableGameObject)gameObject;
					travelerList.Add(new Traveler(from, to, (IMovableGameObject)imgo));
				}
			}
		}
	}
}
