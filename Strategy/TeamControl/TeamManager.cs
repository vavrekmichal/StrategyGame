using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.MoveMgr;
using Strategy.FightMgr;
using Strategy.GameMaterial;
using Strategy.GameObjectControl;
using Mogre;
using Strategy.GameGUI;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;

namespace Strategy.TeamControl {
	/// <summary>
	/// Controls the relations between the teams and transfers objects between the Teams.
	/// </summary>
    public class TeamManager {

        protected Dictionary<string, Team> teamDict; 
		protected Dictionary<Team, List<Team>> friendlyTeamDict;

        public Team playerTeam;

		/// <summary>
		/// Creates the instance of the TeamControl.
		/// </summary>
        public TeamManager() {
            friendlyTeamDict= new Dictionary<Team,List<Team>>();
        }
        
		/// <summary>
		/// Initializes the TeamManager. sets the friendly relations between teams and sets the teams to the manager.
		/// </summary>
		/// <param name="settingTeam">The dictionary with teams (name of the team,team).</param>
		/// <param name="friendlyDict">The relations between teams.</param>
        public void Inicialization(Dictionary<string,Team> settingTeam, Dictionary<Team, List<Team>> friendlyDict) {
			friendlyTeamDict = friendlyDict;
            teamDict = settingTeam;
            playerTeam = teamDict[Game.PlayerName];
        }

		/// <summary>
		/// Checks if the given Teams are friendly.
		/// </summary>
		/// <param name="t1">The firtst Team.</param>
		/// <param name="t2">The second Team.</param>
		/// <returns>Returns if the Teams are friendly.</returns>
		public bool AreFriendly(Team t1, Team t2) {
			if (friendlyTeamDict[t1].Contains(t2)) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Removes the object from the old Team and inserts it to the given Team.
		/// </summary>
		/// <param name="isgo">The removing static object.</param>
		/// <param name="newTeam">The new Team of the static object.</param>
		public void ChangeTeam(IStaticGameObject isgo, Team newTeam) {
			RemoveFromOwnTeam(isgo);
			newTeam.AddISGO(isgo);
		}

		/// <summary>
		/// Removes the object from the old Team and inserts it to the given Team.
		/// </summary>
		/// <param name="imgo">The removing movable object.</param>
		/// <param name="newTeam">The new Team of the movable object.</param>
		public void ChangeTeam(IMovableGameObject imgo, Team newTeam) {
			RemoveFromOwnTeam(imgo);
			newTeam.AddIMGO(imgo);
		}

		/// <summary>
		/// Removes static object from its Team.
		/// </summary>
		/// <param name="isgo">The removing static object.</param>
		public void RemoveFromOwnTeam(IStaticGameObject isgo) {
			isgo.Team.RemoveISGO(isgo);
		}

		/// <summary>
		/// Removes moveble object from its Team. 
		/// </summary>
		/// <param name="imgo">The removing moveble object.</param>
		public void RemoveFromOwnTeam(IMovableGameObject imgo) {
			imgo.Team.RemoveIMGO(imgo);
		}

		/// <summary>
		/// Returns all materials from player's Team inserted to the dictionary.
		/// </summary>
		/// <returns>Returns all materials from player's Team inserted to the dictionary.</returns>
		public Dictionary<string, IMaterial> GetPlayersMaterials() {
			return playerTeam.GetMaterials();
		}

		/// <summary>
		/// Returns Team by the given unique name. If the TeamManager does not know the name,
		/// so returns null.
		/// </summary>
		/// <param name="name">The unique name of the Team.</param>
		/// <returns>Return Team bz the given name.</returns>
		public Team GetTeam(string name) {
			foreach (var team in teamDict) {
				if (team.Key==name) {
					return team.Value;
				}
			}
			return null;
		}
    }
}
