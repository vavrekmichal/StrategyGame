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
using Strategy.Exceptions;

namespace Strategy {
	public class Game {

		protected MouseControl mouseControl;

		public static string PlayerName = "Player";
		public static string SavesGamePath = "../../Media/Mission/Saves";
		public static string NewGamePath = "../../Media/Mission";

		protected static SoundPlayer soundPlayer; // Background music
		protected static GameObjectManager gameObjectMgr;
		protected static IGameGUI gameGUI;
		protected static SceneManager sceneMgr;
		protected static Mission mission;

		private static bool paused;
		private static bool initialized = false;

		#region Singleton, constructor and static properties and functions
		private static Game instance;

		/// <summary>
		/// If the game is not initializes,so creates GUI, MouseControl, GameObjectManager ,SoundPlayer and mission.
		/// Returns singleton instance.
		/// </summary>
		/// <param name="sceneManager">The Mogre SceneManager.</param>
		/// <param name="c">The game CameraMan for the MouseControl.</param>
		/// <param name="mWindow">The RednerWindow for sending the width and height of the window.</param>
		/// <param name="mouse">The Mogre Mouse for GUI.</param>
		/// <param name="keyboard">The Mogre Keyboard for GUI.</param>
		/// <returns>Returns initializes Game singleton instance.</returns>
		public static Game GetInstance(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			if (instance == null) {
				instance = new Game(sceneManager, c, mWindow, mouse, keyboard);
			}
			return instance;
		}

		/// <summary>
		/// Initialzes the Game.
		/// </summary>
		/// <param name="sceneManager">The Mogre SceneManager.</param>
		/// <param name="camera">The game CameraMan for the MouseControl.</param>
		/// <param name="mWindow">The RednerWindow for sending the width and height of the window.</param>
		/// <param name="mouse">The Mogre Mouse for GUI.</param>
		/// <param name="keyboard">The Mogre Keyboard for GUI.</param>
		/// <returns>Returns initializes Game singleton instance.</returns>
		private Game(SceneManager sceneManager, CameraMan camera, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			sceneMgr = sceneManager;
			gameObjectMgr = GameObjectManager.GetInstance();
			gameGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, mouse, keyboard);
			mouseControl = MouseControl.GetInstance(camera, (int)mWindow.Width, (int)mWindow.Height);
			paused = true;
			soundPlayer = new SoundPlayer(mWindow);
			mission = new Mission();
		}

		/// <summary>
		/// Checks if the IGameGUI is initializes (if not throws a exception) and if is initialized,
		/// so returns it.
		/// </summary>
		public static IGameGUI IGameGUI {
			get {
				if (gameGUI == null) {
					throw new NullReferenceException("IGameGUI is not initialized.");
				}
				return gameGUI;
			}
		}

		/// <summary>
		/// Returns instance of GroupManager from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized).
		/// </summary>
		public static GroupManager GroupManager {
			get {
				return gameObjectMgr.GroupManager;
			}
		}

		/// <summary>
		/// Returns instance of IGameObjectCreator from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized). 
		/// </summary>
		public static IGameObjectCreator IGameObjectCreator {
			get {
				return gameObjectMgr.IGameObjectCreator;
			}
		}

		/// <summary>
		/// Returns instance of IFightManager from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized).  
		/// </summary>
		public static IFightManager IFightManager {
			get {
				return gameObjectMgr.IFightManager;
			}
		}

		/// <summary>
		/// Returns instance of IMoveManager from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized). 
		/// </summary>
		public static IMoveManager IMoveManager {
			get {
				return gameObjectMgr.IMoveManager;
			}
		}

		/// <summary>
		/// Returns instance of HitTest from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized).  
		/// </summary>
		public static HitTest HitTest {
			get {
				return gameObjectMgr.HitTest;
			}
		}

		/// <summary>
		/// Returns instance of SolarSystemManager from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized).  
		/// </summary>
		public static SolarSystemManager SolarSystemManager {
			get {
				return gameObjectMgr.SolarSystemManager;
			}
		}

		/// <summary>
		/// Returns instance of TeamManager from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized).  
		/// </summary>
		public static TeamManager TeamManager {
			get {
				return gameObjectMgr.TeamManager;
			}
		}

		/// <summary>
		/// Returns instance of PropertyManager from GameObjectManager. The manager contorls if it is 
		/// initialized and can throws a exception (if is not initialized).  
		/// </summary>
		public static PropertyManager PropertyManager {
			get {
				return gameObjectMgr.PropertyManager;
			}
		}

		/// <summary>
		/// Checks if the IEffectPlayer is initializes (if not throws a exception) and if is initialized,
		/// so returns it.
		/// </summary>
		public static IEffectPlayer IEffectPlayer {
			get {
				if (soundPlayer == null) {
					throw new NullReferenceException("IEffectPlayer is not initialized.");
				}
				return soundPlayer;
			}
		}

		/// <summary>
		/// Checks if the IGameMusicPlayer is initializes (if not throws a exception) and if is initialized,
		/// so returns it.
		/// </summary>
		public static IGameMusicPlayer IGameSoundMakerPlayer {
			get {
				if (soundPlayer == null) {
					throw new NullReferenceException("IGameMusicPlayer is not initialized.");
				}
				return soundPlayer;
			}
		}

		/// <summary>
		/// Checks if the Mission is initializes (if not throws a exception) and if is initialized,
		/// so returns it.
		/// </summary>
		public static Mission Mission {
			get {
				if (mission == null) {
					throw new NullReferenceException("Mission is not initialized.");
				}
				return mission;
			}
		}

		/// <summary>
		/// Returns the instance of SceneManager initialized by Mogre.
		/// </summary>
		public static SceneManager SceneManager {
			get { return sceneMgr; }
		}

		/// <summary>
		/// Returns the object if is controllable, else returns null.
		/// The object is returns by GameObjectManager.
		/// </summary>
		/// <param name="name">The name of the searching object.</param>
		/// <returns>Returns the game object searched by the given name.</returns>
		public static IGameObject GetIGameObject(string name) {
			if (gameObjectMgr.HitTest.IsObjectControllable(name)) {
				return gameObjectMgr.HitTest.GetGameObject(name);
			} else {
				return null;
			}
		}

		/// <summary>
		/// Saves the current mission.
		/// </summary>
		/// <param name="name">The name of the creating save.</param>
		public static void Save(string name) {
			Console.WriteLine(name);
			new XDocument(
				new XElement("root",
					new XElement("someNode2", "someValue")
				)
			)
			.Save("../../Media/Mission/Saves/foo.xml");
		}

		/// <summary>
		/// Loads the mission by givne name and initilazed whole game.
		/// </summary>
		/// <param name="name">The name of the loading game.</param>
		public static void Load(string name) {
			Inicialization(name);
		}

		/// <summary>
		/// Prints the given text to the game console.
		/// </summary>
		/// <param name="text">The printing text.</param>
		public static void PrintToGameConsole(string text) {
			gameGUI.PrintToGameConsole(text);
		}

		/// <summary>
		/// Shows the panel with the possible choices (all SolarSystems) and if the user selects any,
		/// so is created a Traveler.
		/// </summary>
		/// <param name="gameObject">The possible traveler.</param>
		public static void CreateInterstellarTravel(IGameObject gameObject) {
			gameGUI.ShowTravelSelectionPanel(gameObjectMgr.SolarSystemManager.GetAllSolarSystemNames(), gameObject);
		}

		/// <summary>
		/// Ends the current Mission and destoys the Mission.
		/// </summary>
		/// <param name="printText">The text with the reason of ending the mission.</param>
		public static void EndMission(string printText) {
			gameGUI.MissionEnd(printText);
			DestroyMission();
		}

		/// <summary>
		/// Remove the given game object from the game (destroys the object).
		/// </summary>
		/// <param name="gameObject">The removing game object.</param>
		public static void RemoveObject(IGameObject gameObject) {
			gameObjectMgr.RemoveObject(gameObject);
		}

		/// <summary>
		/// Changes the game object Team (sets the given Team).
		/// </summary>
		/// <param name="gameObject">The object which will have the changed Team.</param>
		/// <param name="newTeam">The new Team for the object.</param>
		public static void ChangeObjectsTeam(IGameObject gameObject, Team newTeam) {
			gameObjectMgr.ChangeObjectsTeam(gameObject, newTeam);
		}

		/// <summary>
		/// Destroys the current Mission and initializes new empty game contol objets.
		/// </summary>
		public static void DestroyMission() {
			gameGUI.ClearMissionData();
			gameGUI.Enable = false;
			paused = true;
			mission = new Mission();
			gameObjectMgr.DestroyGame();
			initialized = false;
		}

		/// <summary>
		/// Initializes the mission by the given name. Loads game objects and starts the game.
		/// </summary>
		/// <param name="missionFileName">The name of a file with the mission.</param>
		private static void Inicialization(string missionFileName) {
			gameObjectMgr.Inicialization(missionFileName);
			gameGUI.SetSolarSystemName(SolarSystemManager.GetSolarSystemName(0));
			mission.Initialize();
			gameGUI.Enable = true;
			paused = false;
			PrintToGameConsole("Game loaded");
			initialized = true;
		}

		/// <summary>
		/// Indicatas if the game is initialized. 
		/// </summary>
		public static bool Initialized {
			get { return initialized; }
		}

		/// <summary>
		/// Indicatas if the game is paused or not (if objects are updating or not).
		/// </summary>
		public static bool Paused {
			get { return paused; }
			set {
				if (value) {
					PrintToGameConsole("Game is paused.");
				} else {
					PrintToGameConsole("Game continues.");
				}
				paused = value;
			}
		}

		/// <summary>
		/// Indicatas if the keyboard is captured. 
		/// </summary>
		public static bool KeyboardCaptured { get; set; }

		/// <summary>
		/// Indicatas if the mouse is captured. Mouse is captured if any Panel is registed at
		/// GUI's PopUpPanels.
		/// </summary>
		public static bool MouseCaptured {
			get { return gameGUI.NumberOfPopUpPanels > 0; }
		}
		#endregion

		/// <summary>
		/// Updates GUI system and MusicPlayer if the game is not paused, so 
		/// </summary>
		/// <param name="delay"></param>
		public void Update(float delay) {
			gameGUI.Update();

			soundPlayer.Update(delay);
			if (!Paused) {
				gameObjectMgr.Update(delay);
				mission.Update(delay);
			}
		}

		/// <summary>
		/// Returns the initializes MouseControl.
		/// </summary>
		/// <returns></returns>
		public MouseControl GetMouseControl() {
			return mouseControl;
		}

		/// <summary>
		/// Destroys the GUI system and ends the program.
		/// </summary>
		public void Quit() {
			gameGUI.Dispose();
			throw new ShutdownException();
		}
	}
}
