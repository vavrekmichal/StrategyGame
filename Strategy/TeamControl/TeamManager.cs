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

        protected GroupManager groupManager;
        protected IMoveControler moveControler;
        protected IFightManager fightManager;
        protected List<IMaterial> materials; //will be singleton and shared with this and GUI to show state

        protected GUIControler guiControler;

        protected Dictionary<string, Team> teams; //implementation in mybook think about delete. MUST delete from team and group

        #region singleton and constructor
        public static TeamManager instance;

        public static TeamManager getInstance(SceneManager sceneManager) {
            if (instance==null) {
                instance = new TeamManager(sceneManager);
            }
            return instance;
        }

        private TeamManager(SceneManager sceneManager) {
            groupManager = GroupManager.getInstance(sceneManager);
            moveControler = MoveControler.getInstance();
            fightManager = FightManager.getInstance();
            materials = new List<IMaterial>() { new Wolenium() };
        }
        #endregion

        //public
        public void update(float delay) {
            groupManager.update(delay);
            //production of Materials
        }

        
        public void inicialization() {
            groupManager.inicializeWorld();
            teams = groupManager.getTeams();
            Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            foreach(KeyValuePair<string,Team> team in teams){
                Console.WriteLine(team.Value.ToString());
            }
        }

        public GroupManager getGroupManager() {
            return groupManager;
        }


        /// <summary>
        /// get all IMaterials 
        /// </summary>
        /// <returns>return list with IMaterial</returns>
        public List<IMaterial> getMaterials() {
            return materials;
        }

        public void changeSolarSystem(int numberOfSolarSystem) {
            groupManager.changeSolarSystem(numberOfSolarSystem);
            guiControler.setSolarSystemName(groupManager.getSolarSystemName(numberOfSolarSystem));
        }

        //think about me
        public void setGUI(GUIControler gui) {
            guiControler = gui;
        }
        //private
    }
}
