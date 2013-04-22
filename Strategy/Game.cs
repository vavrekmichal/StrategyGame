using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.MoveMgr;
using Strategy.FightMgr;
using Mogre;
using Strategy.TeamControl;
using Strategy.MogreControl;
using Mogre.TutorialFramework;
using MOIS;
using Strategy.GameGUI;

namespace Strategy {
	class Game {

		protected MouseControl mouseControl;
		protected GameObjectManager gameObjectMgr;

		public static string playerName = "Player";

		private static bool gamePaused;

		protected GUIControler guiControler;
		#region Singleton and constructor
		private static Game instance;

		public static Game getInstance(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			if (instance == null) {
				instance = new Game(sceneManager, c, mWindow, mouse, keyboard);
			}
			return instance;
		}

		private Game(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {

			gameObjectMgr = GameObjectManager.getInstance(sceneManager);
			//fightMgr = FightManager.getInstance();
			guiControler = GUIControler.getInstance(mWindow, mouse, keyboard);
			mouseControl = MouseControl.getInstance(c, sceneManager, guiControler);
			gamePaused = false;
		}
		#endregion

		public void update(float delay) {
			guiControler.update();
			if (!gamePaused) {
				gameObjectMgr.update(delay);
			}
		}


		public void inicialization() {
			gameObjectMgr.setGUI(guiControler);
			gameObjectMgr.inicialization("StartMission");
			guiControler.setSolarSystemName(GroupManager.getInstance().getSolarSystemName(0));

		}

		// Get
		public MouseControl getMouseControl() {
			return mouseControl;
		}

		// Quit
		public void quit() {
			guiControler.dispose();
		}

		// Pause
		public static void pause(bool paused) {
			gamePaused = paused;
		}

		public static bool GameStatus {
			get { return gamePaused; }
		}
	}
}
