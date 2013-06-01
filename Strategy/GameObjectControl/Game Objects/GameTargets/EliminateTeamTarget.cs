using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	class EliminateTeamTarget : ITarget {

		string teamName;
		Property<string> targetInfo;
		TeamControl.Team team;

		const string text1 = "You must eliminate team ";
		const string text2 = "Target is completed. You eliminated team ";

		public EliminateTeamTarget(object[] args) {
			teamName = (string)args[0];
			targetInfo = new Property<string>(text1 + teamName);
		}

		public bool Check(float delay) {
			if (team.Count == 0) {
				targetInfo.Value = text2 + teamName;
				return true;
			} else {
				return false;
			}
		}

		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

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
