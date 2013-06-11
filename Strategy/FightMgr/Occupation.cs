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
	/// <summary>
	/// Represents occupying of the target by the attackers. Class controls distance between target and attackers
	/// and occupying time.
	/// </summary>
	class Occupation {
		private string name;
		private GroupMovables attackers;
		private IGameObject target;
		private Property<TimeSpan> remainingTime;

		// Occupy-distance is increased by this constant.
		const float distanceConst = 1.2f;
		// Sound of start the occupation
		const string occStart = "OccBegan1.wav";
		// Sound of end the occupation
		const string occEnd = "OccSucc1.wav";  

		/// <summary>
		/// Creates Property with remaining time to end of the occupying.
		/// This Property is added to occupied object (to objects properties).
		/// Also runs the sound of the occupation. 
		/// </summary>
		/// <param name="occupier">The attacking group.</param>
		/// <param name="occupied">The target of the occupation.</param>
		public Occupation(GroupMovables occupier, IGameObject occupied) {
			var time = TimeSpan.FromSeconds(occupied.OccupyTime);

			remainingTime = new Property<TimeSpan>(time);

			occupied.AddProperty<TimeSpan>(PropertyEnum.Occupation, remainingTime);
			this.name = "Occupation of " + occupied.Name;


			this.attackers = occupier;
			this.target = (IGameObject)occupied;
			// Play occupation sound
			Game.IEffectPlayer.PlayEffect(occStart);

		}

		#region public 

		/// <summary>
		/// Checks if occupation contains given object.
		/// </summary>
		/// <param name="igo">The testing object.</param>
		/// <returns>Returns if occupation contains given object.</returns>
		public bool Contains(IGameObject igo) {
			if (igo == target) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// Inserts given group to attacker's group and sends it
		/// to target's destination and recalculates group. 
		/// </summary>
		/// <param name="group">The inserting group.</param>
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
		/// Checks if occupation continues, is done or if is canceled.
		/// </summary>
		/// <param Name="delay">The delay between last two frames</param>
		/// <returns>Returns true when occupation is done (object is occupated or occupation is canceled)</returns>
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

		#endregion
		
		/// <summary>
		/// Removes property with occupation time from target's properties, play finish occupation sound 
		/// and finally change target's team to attacker's team.
		/// </summary>
		private void FinishOccupation() {

			target.RemoveProperty(PropertyEnum.Occupation);

			Game.IEffectPlayer.PlayEffect(occEnd);

			Game.ChangeObjectsTeam(target, attackers.OwnerTeam);
		}

		/// <summary>
		/// Check distance between the attacker's position and the target's position. Distance is compare with occupyDistance.
		/// </summary>
		/// <param Name="attackerPostion">The attacker's position</param>
		/// <returns>Returns if attacker is in occupyDistance of target.</returns>
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
