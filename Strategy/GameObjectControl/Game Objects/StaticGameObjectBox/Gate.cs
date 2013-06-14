using System.Collections.Generic;
using Mogre;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;
using Strategy.TeamControl;

namespace Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox {
	/// <summary>
	/// Teleports game objects between SolarSystems. Inherited from StaticGameObject 
	/// to facilitate implementation (overrides just few methods).
	/// </summary>
	public class Gate : StaticGameObject {

		private bool isPorting;

		// Portal animation time
		private const float portTime = 3.4483f;

		private float portTimeDuration;

		private const string meshConst = "gate.mesh";
		// Animation names
		private const string animationPort = "funcionando3_eani_Clip";
		private const string animationStay = "abrirse_eani_Clip";
		private const string travelSound = "ltsaberon01.wav";
		private readonly Vector3 gatePosition = new Vector3(1000, 0, 1000);

		private AnimationState animationState; // The AnimationState the moving object

		protected static List<Traveler> travelerList = new List<Traveler>();

		protected static Team gateTeam;

		/// <summary>
		/// Initializes Mogre properties and sets base animation (animationStay);
		/// </summary>
		/// <param name="name">The name of the gate.</param>
		/// <param name="team">The gate team, it should be None team.</param>
		public Gate(string name, Team team) {
			this.name = name;
			this.team = team;
			position = new Property<Vector3>(gatePosition);
			entity = Game.SceneManager.CreateEntity(name, meshConst);

			SetProperty(PropertyEnum.Position, position);

			animationState = entity.GetAnimationState(animationPort);
			animationState.Loop = true;
			animationState.Enabled = true;
		}

		/// <summary>
		/// Runs the appropriate animation (if porting - animationPort, else -animationStay)
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		public override void Rotate(float delay) {
			if (isPorting) {
				animationState = entity.GetAnimationState(animationStay);
				delay *= 5;
				portTimeDuration -= delay / 10;
				if (portTimeDuration < 0) {
					isPorting = false;
				}
			} else {
				animationState = entity.GetAnimationState(animationPort);
			}
			animationState.Loop = true;
			animationState.Enabled = true;
			animationState.AddTime(delay / 10);
		}

		/// <summary>
		/// Does nothing in invisible mode.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		public override void NonActiveRotate(float delay) { }

		/// <summary>
		/// Shows panel with possible travel destiantions (ShowTravelDestinations).
		/// </summary>
		/// <param name="target">The target which invoke this calling.</param>
		public override void TargetInSight(IMovableGameObject target) {
			ShowTravelDestinations(target);
		}

		/// <summary>
		/// Does nothing on display.
		/// </summary>
		protected override void OnDisplayed() { }

		/// <summary>
		/// Shows GUI panel with possible travel destiantions and playes portal animation.
		/// </summary>
		/// <param name="imgo"></param>
		public void ShowTravelDestinations(IMovableGameObject imgo) {
			Game.InterstellarTravel(imgo);

			isPorting = true;
			portTimeDuration = portTime;
		}

		/// <summary>
		/// Updates all current travelers. If traveler is isDone in the destination so it is removed from update list.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
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

		/// <summary>
		/// Returns list with all current travelers.
		/// </summary>
		/// <returns>Returns list with all current travelers.</returns>
		public static List<Traveler> GetTravelers() {
			return travelerList;
		}

		/// <summary>
		/// Creates traveler from its current SolarSystem to new given SolarSystem.
		/// Object is removed and insert to new one by Traveler class.
		/// Also playes travel sound.
		/// </summary>
		/// <param name="from">The current object SolarSystem.</param>
		/// <param name="to">The future object SolarSystem.</param>
		/// <param name="gameObject">The traveling game object.</param>
		public static void CreateTraveler(SolarSystem from, SolarSystem to, object gameObject) {
			if (gameObject is IMovableGameObject) {
				if (from != to) {
					IMovableGameObject imgo = (IMovableGameObject)gameObject;
					Game.IEffectPlayer.PlayEffect(travelSound);
					travelerList.Add(new Traveler(from, to, (IMovableGameObject)imgo));
				}
			}
		}
	}
}
