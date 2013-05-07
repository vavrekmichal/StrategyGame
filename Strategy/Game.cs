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
using Strategy.GameObjectControl.Game_Objects;
using Strategy.Sound;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy {
	class Game {

		protected MouseControl mouseControl;


		public static string playerName = "Player";

		private static bool gamePaused;

		protected static SoundPlayer soundPlayer; // Background music
		protected static GameObjectManager gameObjectMgr;
		protected static GUIControler guiControler;
		#region Singleton and constructor
		private static Game instance;

		public static Game getInstance(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			if (instance == null) {
				instance = new Game(sceneManager, c, mWindow, mouse, keyboard);
			}
			return instance;
		}

		private Game(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {

			gameObjectMgr = GameObjectManager.GetInstance(sceneManager, mouse, keyboard, mWindow);
			guiControler = new GUIControler(mWindow, mouse, keyboard);
			mouseControl = MouseControl.GetInstance(c, sceneManager, guiControler);
			gamePaused = false;
			soundPlayer = new SoundPlayer(mWindow); //music player
		}

		public static GUIControler GUIManager {
			get {
				if (guiControler == null) {
					throw new NullReferenceException("GUIManager is not initialized.");
				}
				return guiControler;
			}
		}

		public static GroupManager GroupManager {
			get {
				return gameObjectMgr.GroupManager;
			}
		}

		public static IFightManager IFightManager {
			get {
				return gameObjectMgr.IFightManager;
			}
		}

		public static IMoveManager IMoveManager {
			get {
				return gameObjectMgr.IMoveManager;
			}
		}

		public static HitTest HitTest {
			get {
				return gameObjectMgr.HitTest;
			}
		}

		public static IEffectPlayer IEffectPlayer {
			get {
				return soundPlayer;
			}
		}

		public static PropertyManager PropertyManager {
			get {
				return gameObjectMgr.PropertyManager;
			}
		}

		public static IGameSoundMakerPlayer IGameSoundMakerPlayer {
			get {
				return soundPlayer;
			}
		}

		#endregion


		public static void RemoveObject(IGameObject gameObject) {
			gameObjectMgr.RemoveObject(gameObject);
		}

		public static void ChangeObjectsTeam(IGameObject gameObject, Team newTeam) {
			gameObjectMgr.ChangeObjectsTeam(gameObject, newTeam);
		}

		public void Update(float delay) {
			guiControler.Update();
			soundPlayer.HideBox(delay);
			if (!gamePaused) {
				gameObjectMgr.Update(delay);
			}
		}


		public void Inicialization() {
			gameObjectMgr.Inicialization("StartMission", guiControler);
			guiControler.SetSolarSystemName(Game.GroupManager.GetSolarSystemName(0));
		}

		// Get
		public MouseControl GetMouseControl() {
			return mouseControl;
		}

		// Quit
		public void Quit() {
			guiControler.Dispose();
		}

		// Pause
		public static void Pause(bool paused) {
			gamePaused = paused;
		}

		public static bool GameStatus {
			get { return gamePaused; }
		}
	}
}
