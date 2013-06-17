using System;
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
using Strategy.MissionControl;
using System.Xml.Linq;

namespace Strategy {
	public class Game {

		protected MouseControl mouseControl;


		public static string PlayerName = "Player";
		public static string SavesGamePath = "../../Media/Mission/Saves";
		public static string NewGamePath = "../../Media/Mission";

		private static bool gamePaused;
		private static bool keyboardIsCaptured;

		protected static SoundPlayer soundPlayer; // Background music
		protected static GameObjectManager gameObjectMgr;
		protected static IGameGUI gameGUI;
		protected static SceneManager sceneMgr;
		protected static Mission mission;


		#region Singleton and constructor
		private static Game instance;

		public static Game GetInstance(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			if (instance == null) {
				instance = new Game(sceneManager, c, mWindow, mouse, keyboard);
			}
			return instance;
		}

		private Game(SceneManager sceneManager, CameraMan camera, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			sceneMgr = sceneManager;
			gameObjectMgr = GameObjectManager.GetInstance();
			gameGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, mouse, keyboard);
			mouseControl = MouseControl.GetInstance(camera, (int)mWindow.Width, (int)mWindow.Height);
			gamePaused = true;
			soundPlayer = new SoundPlayer(mWindow);
			mission = new Mission();
		}

		public static IGameGUI IGameGUI {
			get {
				if (gameGUI == null) {
					throw new NullReferenceException("IGameGUI is not initialized.");
				}
				return gameGUI;
			}
		}

		public static GroupManager GroupManager {
			get {
				return gameObjectMgr.GroupManager;
			}
		}

		public static IGameObjectCreator IGameObjectCreator {
			get {
				return gameObjectMgr.IGameObjectCreator;
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

		public static SolarSystemManager SolarSystemManager {
			get {
				return gameObjectMgr.SolarSystemManager;
			}
		}

		public static TeamManager TeamManager {
			get {
				return gameObjectMgr.TeamManager;
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

		public static IGameMusicPlayer IGameSoundMakerPlayer {
			get {
				return soundPlayer;
			}
		}

		public static Mission Mission {
			get {
				return mission;
			}
		}

		public static SceneManager SceneManager {
			get { return sceneMgr; }
		}

		public static IGameObject GetIGameObject(string name) {
			if (gameObjectMgr.HitTest.IsObjectControllable(name)) {
				return gameObjectMgr.HitTest.GetGameObject(name);
			} else {
				return null;
			}
		}

		public static void Save(string name) {
			Console.WriteLine(name);
			new XDocument(
				new XElement("root",
					new XElement("someNode2", "someValue")
				)
			)
			.Save("../../Media/Mission/Saves/foo.xml");
		}

		public static void Load(string name) {
			Console.WriteLine(name);
			Inicialization(name);
		}

		public static void PrintToGameConsole(string text) {
			gameGUI.PrintToGameConsole(text);
		}

		public static void InterstellarTravel(IGameObject gameObject) {
			gameGUI.ShowTravelSelectionPanel(gameObjectMgr.SolarSystemManager.GetAllSolarSystemNames(), gameObject);
		}

		public static void EndGame(string printText) {
			gameGUI.MissionEnd(printText);
		}

		#endregion


		public static void RemoveObject(IGameObject gameObject) {
			gameObjectMgr.RemoveObject(gameObject);
		}

		public static void ChangeObjectsTeam(IGameObject gameObject, Team newTeam) {
			gameObjectMgr.ChangeObjectsTeam(gameObject, newTeam);
		}

		public void Update(float delay) {
			gameGUI.Update();

			soundPlayer.Update(delay);
			if (!gamePaused) {
				gameObjectMgr.Update(delay);
				mission.Update(delay);
			}
		}


		public static void DestroyGame() {

			gameGUI.ClearMissionData();
			gameGUI.Enable = false;
			gamePaused = true;
			mission = new Mission();
			gameObjectMgr.DestroyGame();
		}

		private static void Inicialization(string missionFileName) {
			gameObjectMgr.Inicialization(missionFileName);
			gameGUI.SetSolarSystemName(SolarSystemManager.GetSolarSystemName(0));
			mission.Initialize();
			gameGUI.Enable = true;
			gamePaused = false;
			PrintToGameConsole("Game loaded");
		}

		public MouseControl GetMouseControl() {
			return mouseControl;
		}

		// Quit
		public void Quit() {
			gameGUI.Dispose();
		}

		// Pause
		public static void Pause(bool paused) {
			gamePaused = paused;
		}

		public static bool GameStatus {
			get { return gamePaused; }
		}

		public static bool KeyboardCaptured {
			get { return keyboardIsCaptured; }
			set { keyboardIsCaptured = value; }
		}

		public static bool MouseCaptured {
			get { return gameGUI.NumberOfPopUpPanels > 0; }
		}
	}
}
