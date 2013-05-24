﻿using System;
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
	public class Game {

		protected MouseControl mouseControl;


		public static string playerName = "Player";

		private static bool gamePaused;

		protected static SoundPlayer soundPlayer; // Background music
		protected static GameObjectManager gameObjectMgr;
		protected static IGameGUI gameGUI;
		protected static SceneManager sceneMgr;
		#region Singleton and constructor
		private static Game instance;

		public static Game getInstance(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			if (instance == null) {
				instance = new Game(sceneManager, c, mWindow, mouse, keyboard);
			}
			return instance;
		}

		private Game(SceneManager sceneManager, CameraMan c, RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			sceneMgr = sceneManager;
			gameObjectMgr = GameObjectManager.GetInstance(sceneManager, mouse, keyboard, mWindow);
			gameGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, mouse, keyboard);
			//guiControler = new GUIControler(mWindow, mouse, keyboard);
			mouseControl = MouseControl.GetInstance(c, sceneManager);
			gamePaused = false;
			soundPlayer = new SoundPlayer(mWindow); //music player
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

		public static IGameSoundMakerPlayer IGameSoundMakerPlayer {
			get {
				return soundPlayer;
			}
		}

		public static SceneManager SceneManager {
			get { return sceneMgr; }
		}

		public static void PrintToGameConsole(string text){
			gameGUI.PrintToGameConsole(text);
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
			soundPlayer.HideBox(delay);
			if (!gamePaused) {
				gameObjectMgr.Update(delay);
			}
		}


		public void Inicialization() {
			gameObjectMgr.Inicialization("StartMission");
			gameGUI.SetSolarSystemName(Game.GroupManager.GetSolarSystemName(0));
			PrintToGameConsole("Game loaded");
		}

		// Get
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
	}
}
