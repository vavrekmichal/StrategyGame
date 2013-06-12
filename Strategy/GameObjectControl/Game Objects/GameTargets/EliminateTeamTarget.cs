using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	/// <summary>
	/// Target is to destroy the entire team to the last unit.
	/// </summary>
	class EliminateTeamTarget : ITarget {

		string teamName;
		Property<string> targetInfo;
		TeamControl.Team team;

		const string text1 = "You must eliminate team ";
		const string text2 = "Target is completed. You eliminated team ";

		/// <summary>
		/// Sets data to initialization: a team-taget and info Property.
		/// </summary>
		/// <param name="args">The arguments should contains just a team name.</param>
		public EliminateTeamTarget(object[] args) {
			teamName = (string)args[0];
			targetInfo = new Property<string>(text1 + teamName);
		}
	
		/// <summary>
		/// Controls if the team-target has any member.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		/// <returns>Returns if the team is without members.</returns>
		public bool Check(float delay) {
			if (team.Count == 0) {
				targetInfo.Value = text2 + teamName;
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Return Property with a mission's target info.
		/// </summary>
		/// <returns>Return reference to Property with a mission's target info.</returns>
		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

		/// <summary>
		/// Initializes mission's target: gets the team-target.
		/// </summary>
		/// <returns>Returns if initialization was successful.</returns>
		public bool Initialize() {
			team = Game.TeamManager.GetTeam(teamName);
			// Team does't exist = eliminated
			if (team == null) {
				return false;
			} else {
				return true;
			}
		}
	}
}
