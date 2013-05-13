using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.RuntimeProperty;


namespace Strategy.GameObjectControl.GroupMgr {
	public class Traveler {
		private SolarSystem from;
		private SolarSystem to;
		private IMovableGameObject traveler;
		private Property<TimeSpan> timeToGo;

		private bool arrived;

		public Traveler(SolarSystem from, SolarSystem to, IMovableGameObject traveler) {
			this.from = from;
			this.to = to;
			this.traveler = traveler;
			long travelTime = (long)GetSquareOfDistance(from, to);
			from.RemoveIMGO(traveler);
			timeToGo = new Property<TimeSpan>(new TimeSpan(travelTime*60)); //multiply by min
			traveler.ChangeVisible(false);
		}

		private double GetSquareOfDistance(SolarSystem s1, SolarSystem s2) {
			double xd = s2.Position.x - s1.Position.x;
			double yd = s2.Position.y - s1.Position.y;
			double zd = s2.Position.z - s1.Position.z;
			double squareOfDistance = (xd * xd + yd * yd + zd * zd);
			return squareOfDistance;
		}

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

		public bool IsDone {
			get { return arrived; }
		}

		public Property<TimeSpan> TimeProperty {
			get { return timeToGo; }
		}

		public override string ToString() {
			return traveler.Name + " will arrives to " + to.Name+" in ";
		}
	}
}
