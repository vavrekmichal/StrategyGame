using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.MoveControl;
using Strategy.FightControl;
using Strategy.GameMaterial;
using Strategy.GroupControl;
using Mogre;
using Strategy.GameGUI;

namespace Strategy.TeamControl {
    class TeamManager {
        //TODO: ProduceManager
        //protected List<IMaterial> materials; //will be singleton and shared with this and GUI to show state

        protected GUIControler guiControler;

        protected Dictionary<string, Team> teams; //implementation in mybook think about delete. MUST delete from team and group
        
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
            
        }
        #endregion

        //public
        
        public void inicialization(Dictionary<string,Team> settingTeam) {
            teams = settingTeam;
            playerTeam = teams[MyMogre.playerName];
            Console.WriteLine("Team print at Start of the Game:");
            foreach(KeyValuePair<string,Team> team in teams){
                //team.Value.setMaterials(materials);
                Console.WriteLine(team.Value.ToString());
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

        //private
    }
}
