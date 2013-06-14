using System;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;


namespace Strategy.GameObjectControl {
	/// <summary>
	/// Represents a traveler between game solar systems.
	/// </summary>
	public class Traveler {
		private SolarSystem from;
		private SolarSystem to;
		private IMovableGameObject traveler;
		private Property<TimeSpan> timeToGo;

		private bool arrived;

		/// <summary>
		/// Initializes Traveler and calculates the time of the travel (from the distance between
		/// SolarSystems). Also creates the object invisible and removes it from current SolarSystem.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="traveler"></param>
		public Traveler(SolarSystem from, SolarSystem to, IMovableGameObject traveler) {
			this.from = from;
			this.to = to;
			this.traveler = traveler;
			long travelTime = (long)GetSquareOfDistance(from, to);

			timeToGo = new Property<TimeSpan>(new TimeSpan(travelTime*60)); // Multiply by 60

			traveler.Stop();

			// Removes from current SolarSystem
			from.RemoveIMGO(traveler);
			traveler.ChangeVisible(false);
		}

		/// <summary>
		/// Updates the traveler. If it reached the destination, so adds it to new SolarSystem
		/// and sets the indicator.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			if (!arrived) {
				var zeroSpan = new TimeSpan(0, 0, 0);
				var diff = TimeSpan.FromSeconds(delay);
				var timeToGo2 = timeToGo.Value - diff;

				if (timeToGo2 > zeroSpan) {
					timeToGo.Value = timeToGo2;
				} else {
					timeToGo.Value = zeroSpan;
					to.AddIMGO(traveler);
					arrived = true;
				}
			}
		}

		/// <summary>
		/// Indicates if the Target reached the destination.
		/// </summary>
		public bool IsDone {
			get { return arrived; }
		}

		/// <summary>
		/// Returns the Property with current time to reach the destiantion.
		/// </summary>
		public Property<TimeSpan> TimeProperty {
			get { return timeToGo; }
		}

		/// <summary>
		/// Creates the text about Traveler.
		/// </summary>
		/// <returns>Returns the text with information from and where it's traveling.</returns>
		public override string ToString() {
			return traveler.Name + " will arrives to " + to.Name+" in ";
		}

		/// <summary>
		/// Calculates squared distance between given SolarSystems.
		/// </summary>
		/// <param name="s1">The first SolarSystem.</param>
		/// <param name="s2">The second SolarSystem.</param>
		/// <returns></returns>
		private static double GetSquareOfDistance(SolarSystem s1, SolarSystem s2) {
			double xd = s2.Position.x - s1.Position.x;
			double yd = s2.Position.y - s1.Position.y;
			double zd = s2.Position.z - s1.Position.z;
			double squareOfDistance = (xd * xd + yd * yd + zd * zd);
			return squareOfDistance;
		}
	}
}
