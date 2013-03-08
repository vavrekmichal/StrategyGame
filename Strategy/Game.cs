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
using Strategy.GameGUI;

namespace Strategy {
    class Game {
        protected GroupManager groupManager;
        protected IMoveControler moveControler;
        protected IFightManager fightManager;
        protected TeamManager teamManager;
        protected MouseControl mouseControl;
		
		private static bool gamePaused;

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
           
            groupManager = GroupManager.getInstance(sceneManager);
            moveControler = MoveControler.getInstance();
            fightManager = FightManager.getInstance();
            teamManager = TeamManager.getInstance();
            guiControler = GUIControler.getInstance(mWindow, mouse, keyboard);  
            mouseControl = MouseControl.getInstance(c, sceneManager, groupManager, guiControler);
			gamePaused = false;
        }
        #endregion

        public void update(float delay) {
            guiControler.update();
			if(!gamePaused){
				groupManager.update(delay);
				teamManager.update();
			}
            //production of Materials
        }

        public GroupManager getGroupManager() {
            return groupManager;
        }

        public void inicialization() {
			groupManager.setGUI(guiControler);
            groupManager.inicializeWorld();
			teamManager.setGUI(guiControler);
            teamManager.inicialization(groupManager.getTeams());
            guiControler.inicialization(teamManager.playerTeam.getMaterials());
            guiControler.setSolarSystemName(groupManager.getSolarSystemName(0));
            
        }

        //GET
        public MouseControl getMouseControl() {
            return mouseControl;
        }

        //QUIT
        public void quit() {
            guiControler.dispose();
        }

		//PAUSE
		public static void pause(bool paused) {
			gamePaused = paused;
		}

		public static bool gameStatus() {
			return gamePaused;
		}
    }
}
