using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.FightMgr {
	class Occupation {
		private string name;
		private GroupMovables attackers;
		private object target;
		private Property<TimeSpan> remainingTime;

		private bool occIsMov;

		const string prepName = "Occupation of ";

		public Occupation(GroupMovables occupator, object occupied, TimeSpan time) {

			var castedOccupied = occupied as IMovableGameObject;
			if (castedOccupied != null) {
				occIsMov = true;
				this.name = prepName + castedOccupied.Name;

			} else {
				occIsMov = false;
				this.name = prepName + ((IStaticGameObject)occupied).Name;
			}

			this.attackers = occupator;
			this.target = occupied;
			remainingTime = new Property<TimeSpan>(time);
		}

		/// <summary>
		/// Function is checking if occupation is done or if is canceled
		/// </summary>
		/// <param name="delay">Delay between last two frames</param>
		/// <returns>Returns true when occupation is done (object is occupated or occupation
		/// is canceled) and false when </returns>
		public bool check(float delay) {

			bool noOneInDist = true;
			foreach (IMovableGameObject unit in attackers) {
				if (checkDistance(unit.Position)) {
					noOneInDist = false;
				}
			}

			if (noOneInDist) {
				// Objects are too far and occupation must be stopped
				return true;
			}

			var delayTimeSpan = TimeSpan.FromSeconds(delay);
			var remaining = remainingTime.Value - delayTimeSpan;
			// Object is occupied
			if (remaining < TimeSpan.Zero) {
				finishOccupation();
				return true;
			} else {
				Console.WriteLine(remaining);
				remainingTime.Value = remaining;
				return false;
			}
		}

		private void finishOccupation() {
			Game.changeObjectsTeam(target,attackers.OwnerTeam);
		}

		/// <summary>
		/// Function check distance between attacker and target. Distance is compare with occupyDistance
		/// (IMGO/ISGO)
		/// </summary>
		/// <param name="attackerPostion">Attacker's position</param>
		/// <returns>Returns false when distance is too long else returns true</returns>
		private bool checkDistance(Mogre.Vector3 attackerPostion) {
			float maxDist;
			Mogre.Vector3 targetPosition;
			if (occIsMov) {
				var imgo = (IMovableGameObject)target;
				maxDist = imgo.OccupyDistance;
				targetPosition = imgo.Position; 
			} else {
				var isgo = (IStaticGameObject)target;
				maxDist = isgo.OccupyDistance;
				targetPosition = isgo.Position;
			}

			var xd = targetPosition.x - attackerPostion.x;
			var zd = targetPosition.z - attackerPostion.z;
			var squaredDistance = xd * xd + zd * zd;

			var squaredPickUpDist = maxDist * maxDist;

			if (squaredDistance > (squaredPickUpDist * 1.5)) {
				return false;
			} else {
				return true;
			}

		}
	}
}
