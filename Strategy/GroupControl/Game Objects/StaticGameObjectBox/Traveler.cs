using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Strategy.GroupControl.Game_Objects.MovableGameObjectBox;


namespace Strategy.GroupControl.Game_Objects.StaticGameObjectBox {
	public class Traveler {
		private SolarSystem from;
		private SolarSystem to;
		private IMovableGameObject traveler;
		private TimeSpan timeToGo;

		private bool arrived;

		public Traveler(SolarSystem from, SolarSystem to, IMovableGameObject traveler) {
			this.from = from;
			this.to = to;
			this.traveler = traveler;
			long travelTime = (long)getSquareOfDistance(from, to);
			timeToGo = new TimeSpan(travelTime);
		}

		private double getSquareOfDistance(SolarSystem s1, SolarSystem s2) {
			double xd = s2.Position.x - s1.Position.x;
			double yd = s2.Position.y - s1.Position.y;
			double zd = s2.Position.z - s1.Position.z;
			double squareOfDistance = (xd * xd + yd * yd + zd * zd);
			return squareOfDistance;
		}

		public void update(float delay) {
			if (!arrived) {
				var zeroSpan = new TimeSpan(0, 0, 0);
				var diff = TimeSpan.FromSeconds(delay);
				var timeToGo2 = timeToGo - diff;

				if (timeToGo2 > zeroSpan) {
					timeToGo = timeToGo2;
				} else {
					timeToGo = zeroSpan;
				}
			} else {
				arrived = true;
			}
		}

		public bool Done {
			get { return arrived; }
		}
	}
}
