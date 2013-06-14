﻿using System;
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

        public TeamManager() {
            friendlyTeamDict= new Dictionary<Team,List<Team>>();
        }


        // Public
        
        public void Inicialization(Dictionary<string,Team> settingTeam, Dictionary<Team, List<Team>> friendlyDict) {
			friendlyTeamDict = friendlyDict;
            teamDict = settingTeam;
            playerTeam = teamDict[Game.PlayerName];
            Console.WriteLine("Team print at Start of the Game:");
            foreach(KeyValuePair<string,Team> team in teamDict){
                Console.WriteLine(team.Value.Print());
            }
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

		public Team GetTeam(string name) {
			foreach (var team in teamDict) {
				if (team.Key==name) {
					return team.Value;
				}
			}
			return null;
		}
        // Private
    }
}
