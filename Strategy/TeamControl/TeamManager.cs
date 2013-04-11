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

namespace Strategy.TeamControl {
    class TeamManager {
        //TODO: ProduceManager
        //protected List<IMaterial> materials; //will be singleton and shared with this and GUI to show state

        protected GUIControler guiControler;

        protected Dictionary<string, Team> teamDict; //implementation in mybook think about delete. MUST delete from team and group

		protected Dictionary<Team, List<Team>> friendlyTeamDict;

        public Team playerTeam;

        #region singleton and constructor
        public static TeamManager instance;

        public static TeamManager getInstance() {
            if (instance==null) {
                instance = new TeamManager();
            }
            return instance;
        }

        private TeamManager() {
            friendlyTeamDict= new Dictionary<Team,List<Team>>();
        }
        #endregion

        //public
        
        public void inicialization(Dictionary<string,Team> settingTeam, Dictionary<Team, List<Team>> friendlyDict) {
			friendlyTeamDict = friendlyDict;
            teamDict = settingTeam;
            playerTeam = teamDict[Game.playerName];
            Console.WriteLine("Team print at Start of the Game:");
            foreach(KeyValuePair<string,Team> team in teamDict){
                Console.WriteLine(team.Value.print());
            }
        }

        //think about me
        public void setGUI(GUIControler gui) {
            guiControler = gui;
        }

        public void update() {
            foreach (KeyValuePair<string, IMaterial> k in playerTeam.getMaterials()) {
                guiControler.setMaterialState(k.Key, k.Value.state);
            }
        }

		public bool areFriendly(Team t1, Team t2) {
			if (friendlyTeamDict[t1].Contains(t2)) {
				return true;
			} else {
				return false;
			}
		}
        //private
    }
}
