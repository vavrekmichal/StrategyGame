using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl;
using Strategy.MoveControl;
using Strategy.FightControl;
using Mogre;
using Strategy.TeamControl;
using Strategy.MogreControl;
using Mogre.TutorialFramework;
using MOIS;

namespace Strategy {
    class Game {
        protected GroupManager groupManager;
        protected IMoveControler moveControler;
        protected IFightManager fightManager;
        protected TeamManager teamManager;
        protected MouseControl mouseControl;

        protected GUIControler guiControler;
        #region Singleton and constructor
        private static Game instance;

        public static Game getInstance(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
            if (instance==null) {
                instance = new Game(sceneManager, c, mWindow, mouse, keyboard);
            }
            return instance;
        }

        private Game(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
            //guiControler = new GUIControler(
           
            groupManager = GroupManager.getInstance(sceneManager);
            moveControler = MoveControler.getInstance();
            fightManager = FightManager.getInstance();
            teamManager = TeamManager.getInstance();
            guiControler = GUIControler.getInstance(mWindow, mouse, keyboard, teamManager.getMaterials());
            mouseControl = MouseControl.getInstance(c, sceneManager, groupManager, guiControler);
            
        }
        #endregion

        public void update(float delay) {
            guiControler.update();
            groupManager.update(delay);
            //production of Materials
        }

        public GroupManager getGroupManager() {
            return groupManager;
        }

        public void inicialization() {
            groupManager.inicializeWorld();
            guiControler.setSolarSystemName(groupManager.getSolarSystemName(0));
            teamManager.inicialization(groupManager.getTeams());
            
        }

        //GET
        public MouseControl getMouseControl() {
            return mouseControl;
        }

        //QUIT
        public void quit() {
            guiControler.dispose();
        }
    }
}
