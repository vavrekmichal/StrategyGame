using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.Game_Objects;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.FightMgr {
	class Occupation {
		private string name;
		private GroupMovables attackers;
		private IGameObject target;
		private Property<TimeSpan> remainingTime;

		const float distanceConst = 1.2f;	// Occupy-distance is increased by this constant.
		const string occStart = "OccBegan1.wav"; // Sound of start the occupation
		const string occEnd = "OccSucc1.wav";  // Sound of end the occupation

		/// <summary>
		/// Constructor creates Property with remaining time to end of the occupation.
		/// This Property is added to occupied object (time is displayed in GUI - object's properties).
		/// It also runs the sound of the occupation. 
		/// </summary>
		/// <param name="occupier">Attacking group</param>
		/// <param name="occupied">Occupied group</param>
		public Occupation(GroupMovables occupier, IGameObject occupied) {
			var time = TimeSpan.FromSeconds(occupied.OccupyTime);

			remainingTime = new Property<TimeSpan>(time);

			occupied.AddProperty<TimeSpan>(PropertyEnum.Occupation, remainingTime);
			this.name = "Occupation of " + occupied.Name;


			this.attackers = occupier;
			this.target = (IGameObject)occupied;
			Game.IEffectPlayer.PlayEffect(occStart);

		}

		public bool Contains(IGameObject igo) {
			if (igo == target) {
				return true;
			}

			return false;
		}

		public void AddGroup(GroupMovables group) {
			GroupManager groupMgr = Game.GroupManager;
			Game.IMoveManager.GoToTarget(group, target);
			int groupCount = group.Count;
			for (int i = 0; i < groupCount; i++) {
				IMovableGameObject temp = group[0];
				groupMgr.RemoveFromGroup(temp);
				groupMgr.AddToGroup(attackers, temp);

			}
			attackers.Reselect();
			groupMgr.SelectGroup(attackers);

		}

		/// <summary>
		/// Function is checking if occupation is done or if is canceled
		/// </summary>
		/// <param Name="delay">Delay between last two frames</param>
		/// <returns>Returns true when occupation is done (object is occupated or occupation
		/// is canceled) and false when </returns>
		public bool Check(float delay) {

			bool noOneInDist = true;
			foreach (IMovableGameObject unit in attackers) {
				if (CheckDistance(unit.Position)) {
					noOneInDist = false;
				}
			}

			if (noOneInDist) {
				// Objects are too far and occupation must be stopped
				target.RemoveProperty(PropertyEnum.Occupation);
				return true;
			}

			var delayTimeSpan = TimeSpan.FromSeconds(delay);
			var remaining = remainingTime.Value - delayTimeSpan;

			if (remaining < TimeSpan.Zero) {

				// Object is occupied
				FinishOccupation();
				return true;
			} else {
				remainingTime.Value = remaining;
				return false;
			}
		}

		private void FinishOccupation() {

			target.RemoveProperty(PropertyEnum.Occupation);

			Game.IEffectPlayer.PlayEffect(occEnd);

			Game.ChangeObjectsTeam(target, attackers.OwnerTeam);
		}

		/// <summary>
		/// Function check distance between attacker and target. Distance is compare with occupyDistance
		/// (IMGO/ISGO)
		/// </summary>
		/// <param Name="attackerPostion">Attacker's position</param>
		/// <returns>Returns false when distance is too long else returns true</returns>
		private bool CheckDistance(Mogre.Vector3 attackerPostion) {
			float maxDist;
			Mogre.Vector3 targetPosition;

			maxDist = target.OccupyDistance;
			targetPosition = target.Position;


			var xd = targetPosition.x - attackerPostion.x;
			var zd = targetPosition.z - attackerPostion.z;
			var squaredDistance = xd * xd + zd * zd;

			var squaredPickUpDist = maxDist * maxDist;

			if (squaredDistance > (squaredPickUpDist )) {
				return false;
			} else {
				return true;
			}

		}
	}
}
