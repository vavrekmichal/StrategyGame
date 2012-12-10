using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.MoveControl;
using Strategy.FightControl;
using Strategy.GameMaterial;
using Strategy.GroupControl;
using Mogre;

namespace Strategy.TeamControl {
    class TeamManager {
        //TODO: ProduceManager
        protected List<IMaterial> materials; //will be singleton and shared with this and GUI to show state

        protected GUIControler guiControler;

        protected Dictionary<string, Team> teams; //implementation in mybook think about delete. MUST delete from team and group

        #region singleton and constructor
        public static TeamManager instance;

        public static TeamManager getInstance() {
            if (instance==null) {
                instance = new TeamManager();
            }
            return instance;
        }

        private TeamManager() {
            materials = new List<IMaterial>() { new Wolenium() ,new Wolenarium(), new Class1()};
        }
        #endregion

        //public
        

        
        public void inicialization(Dictionary<string,Team> settingTeam) {
            teams = settingTeam;
            Console.WriteLine("Team print at Start of the Game:");
            foreach(KeyValuePair<string,Team> team in teams){
                Console.WriteLine(team.Value.ToString());
            }
        }



        /// <summary>
        /// get all IMaterials 
        /// </summary>
        /// <returns>return list with IMaterial</returns>
        public List<IMaterial> getMaterials() {
            return materials;
        }



        //think about me
        public void setGUI(GUIControler gui) {
            guiControler = gui;
        }
        //private
    }
}
