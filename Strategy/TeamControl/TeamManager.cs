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
    public class TeamManager {

        protected Dictionary<string, Team> teamDict; // Implementation in mybook think about delete. MUST delete from team and group

		protected Dictionary<Team, List<Team>> friendlyTeamDict;

        public Team playerTeam;

        #region singleton and constructor
        public static TeamManager instance;

        public static TeamManager GetInstance() {
            if (instance==null) {
                instance = new TeamManager();
            }
            return instance;
        }

        private TeamManager() {
            friendlyTeamDict= new Dictionary<Team,List<Team>>();
        }
        #endregion

        // Public
        
        public void Inicialization(Dictionary<string,Team> settingTeam, Dictionary<Team, List<Team>> friendlyDict) {
			friendlyTeamDict = friendlyDict;
            teamDict = settingTeam;
            playerTeam = teamDict[Game.playerName];
            Console.WriteLine("Team print at Start of the Game:");
            foreach(KeyValuePair<string,Team> team in teamDict){
                Console.WriteLine(team.Value.Print());
            }
        }

        public void Update() {
			//foreach (KeyValuePair<string, IMaterial> k in playerTeam.GetMaterials()) {
			//	guiControler.SetMaterialState(k.Key, k.Value.State);
			//}
        }

		public bool AreFriendly(Team t1, Team t2) {
			if (friendlyTeamDict[t1].Contains(t2)) {
				return true;
			} else {
				return false;
			}
		}

		public void ChangeTeam(IStaticGameObject isgo, Team newTeam) {
			RemoveFromOwnTeam(isgo);
			newTeam.AddISGO(isgo);
		}

		public void ChangeTeam(IMovableGameObject imgo, Team newTeam) {
			RemoveFromOwnTeam(imgo);
			newTeam.AddIMGO(imgo);
		}

		public void RemoveFromOwnTeam(IStaticGameObject isgo) {
			isgo.Team.RemoveISGO(isgo);
		}

		public void RemoveFromOwnTeam(IMovableGameObject imgo) {
			imgo.Team.RemoveIMGO(imgo);
		}

		public Dictionary<string, IMaterial> GetPlayersMaterials() {
			return playerTeam.GetMaterials();
		}
        // Private
    }
}
