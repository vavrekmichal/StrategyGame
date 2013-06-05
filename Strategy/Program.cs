using System;
using System.Collections.Generic;
using IrrKlang;
using Strategy.GameObjectControl;
using Strategy.MogreControl;
using Strategy.Sound;
using Strategy.TeamControl;
using Mogre.TutorialFramework;
using Strategy.Exceptions;





namespace Strategy {
	/// <summary>
	/// Main class derives BaseApplication for easier work with MOGRE
	/// </summary>
	/// 
	class MyMogre : Mogre.TutorialFramework.BaseApplication {

		protected MOIS.InputManager mInputMgr; // Use for create control (mouse, keyborard) instance
		protected float mTimer; // Float as timer to determine of duration overlay 
		protected bool exit = false; // Controlor if player is alive

		protected Mogre.ColourValue fadeColour = new Mogre.ColourValue(0.05f, 0.05f, 0.05f); // Color of fog and shadow
		protected static readonly Mogre.Vector3 cameraStart = new Mogre.Vector3(0, 1000, 1000);

		protected CameraMan cameraMan;

		protected Game myGame;


		public static void Main() {
			new MyMogre().Go();

		}
		#region Create world and camera

		/// <summary>
		/// This overriden class inicializes whole world (objects, mission, sounds, camera)
		/// </summary>
		protected override void CreateScene() {

			//myGame.Inicialization();

			LoadFont();

			SetShadow(); //Set shadow (color, technique)

			SetGround();	//Set a floor

			SetSun(); //Set lights 

			SetSky(); //Set sky texture


		}

		/// <summary>
		/// This method inicializes camere and cameraMan
		/// </summary>
		protected override void CreateCamera() {
			mCamera = mSceneMgr.CreateCamera("myCam");
			mCamera.Position = cameraStart;
			mCamera.LookAt(Mogre.Vector3.ZERO);
			mCamera.NearClipDistance = 5;
			mCamera.FarClipDistance = 30000;
			mCameraMan = new Mogre.TutorialFramework.CameraMan(mCamera);
			cameraMan = mCameraMan;
		}

		/// <summary>
		/// The CreateViewports Set camera and gives viewport to camera
		/// </summary>
		protected override void CreateViewports() {
			Mogre.Viewport viewport = mWindow.AddViewport(mCamera);
			viewport.BackgroundColour = new Mogre.ColourValue(0.05f, 0.05f, 0.05f);
			mCamera.AspectRatio = (float)viewport.ActualWidth / viewport.ActualHeight;
		}

		#endregion

		/// <summary>
		/// Here is inicialized SceneManager from Root
		/// </summary>
		protected override void ChooseSceneManager() {
			mSceneMgr = mRoot.CreateSceneManager(Mogre.SceneType.ST_EXTERIOR_CLOSE);
		}

		/// <summary>
		/// This method add new FrameListener (Ninja moving)
		/// </summary>
		protected override void CreateFrameListeners() {
			base.CreateFrameListeners();

		}

		/// <summary>
		/// Antibug
		/// </summary>
		private void LoadFont() {
			Mogre.FontManager.Singleton.Load("BlueHighway", Mogre.ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
		}
		#region Animation


		/// <summary>
		/// This method is called in each time in game loop method Update scene and call walk method of
		/// movable objects
		/// </summary>
		/// <param Name="evt">delay between last frames</param>
		protected override void UpdateScene(Mogre.FrameEvent evt) {

			float f = evt.timeSinceLastFrame;

			myGame.Update(f);

			base.UpdateScene(evt);
			if (mTimer > 0) { //if overlay showed
				mTimer -= f;
				if (mTimer <= 0) {
					Mogre.OverlayManager.Singleton.GetByName("Author").Hide(); //timer is timeout -> hide overlay
					exit = false;
				}
			}
		}
		#endregion

		/// <summary>
		/// Here is initialization of eventHadlers (keyboard and mouse)
		/// </summary>
		protected override void InitializeInput() {
			base.InitializeInput();

			int windowHandle;
			mWindow.GetCustomAttribute("WINDOW", out windowHandle);
			mInputMgr = MOIS.InputManager.CreateInputSystem((uint)windowHandle);

			myGame = Game.getInstance(mSceneMgr, cameraMan, mWindow, mMouse, mKeyboard);


			mMouse.MousePressed += new MOIS.MouseListener.MousePressedHandler(myGame.GetMouseControl().OnMyMousePressed);
			mMouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(myGame.GetMouseControl().OnMyMouseReleased);
			mMouse.MouseMoved += new MOIS.MouseListener.MouseMovedHandler(myGame.GetMouseControl().OnMyMouseMoved);
		}

		#region Keyboard control


		/// <summary>
		/// This function is called when any key is pressed
		/// </summary>
		/// <param Name="evt">Wwhich button was pressed</param>
		/// <returns>kKey was pressed -> true</returns>
		protected override bool OnKeyPressed(MOIS.KeyEvent evt) {
			base.OnKeyPressed(evt);
			switch (evt.key) {
				case MOIS.KeyCode.KC_RETURN:
					if (exit) {
						try {
							Shutdown();
						} catch (System.Exception) {
							Quit();
						}
					} else {
						var messageBox = Mogre.OverlayManager.Singleton.GetOverlayElement("Author/MessageBox");
						messageBox.Left = (mWindow.Width - messageBox.Width) / 2;
						messageBox.Top = (mWindow.Height - messageBox.Height);

						var messageBody = Mogre.OverlayManager.Singleton.GetOverlayElement("Author/MessageBox/Body");
						messageBody.Caption = "Made by Wolen\nPress ENTER to exit";
						messageBody.Top = messageBox.Height / 2;

						Mogre.OverlayManager.Singleton.GetByName("Author").Show();
						mTimer = 1;
						exit = true;
					}
					break;
				// Music section
				case MOIS.KeyCode.KC_NUMPAD9:
					Game.IGameSoundMakerPlayer.VolumeUp();
					break;
				case MOIS.KeyCode.KC_NUMPAD6:
					Game.IGameSoundMakerPlayer.VolumeDown();
					break;
				case MOIS.KeyCode.KC_N:
					Game.IGameSoundMakerPlayer.StopActualSong();
					break;
				case MOIS.KeyCode.KC_M:
					Game.IGameSoundMakerPlayer.Pause();
					break;
				case MOIS.KeyCode.KC_B:
					Game.IGameSoundMakerPlayer.ActualPlaying();
					break;
				// End of music section
				case MOIS.KeyCode.KC_R:
					RestartCamera();
					break;

				case MOIS.KeyCode.KC_ESCAPE:
					Quit();
					break;
			}

			return true;
		}


		#endregion

		private void Quit() {
			myGame.Quit();
			throw new ShutdownException();
		}

		private void RestartCamera() {
			mCamera.Position = cameraStart;
			mCamera.LookAt(Mogre.Vector3.ZERO);
		}


		/// <summary>
		/// This override function Set basic setting to RenderSystem (Setup RenderSystem by code)
		/// </summary>
		/// <returns>True</returns>
		protected override bool Configure() {
			Mogre.RenderSystem rs = mRoot.GetRenderSystemByName("OpenGL Rendering Subsystem");
			rs.SetConfigOption("Full Screen", "No");
			rs.SetConfigOption("Video Mode", "1280 x 720 @ 32-bit colour");
			rs.SetConfigOption("FSAA", "0");
			mRoot.RenderSystem = rs;
			mWindow = mRoot.Initialise(true, "Render Window");
			return true;
		}



		/// <summary>
		/// The setShadow() is function to Set color and type of shadows
		/// </summary>
		private void SetShadow() {
			mSceneMgr.AmbientLight = new Mogre.ColourValue(.1f, .1f, .1f);
			mSceneMgr.ShadowTechnique = Mogre.ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE;
		}

		/// <summary>
		/// The function to Set the ground
		/// </summary>
		private void SetGround() {
			//Mogre.Plane plane = new Mogre.Plane(Mogre.Vector3.UNIT_Y, 0);
			////Inicialized ground
			////my nota - poslendi  dve jsou hustota 4-5 je 2D vektor na velikost
			//Mogre.MeshManager.Singleton.CreatePlane("ground", Mogre.ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, plane, 
			//	150000, 150000, 200, 200, true, 1, 500, 500, Mogre.Vector3.NEGATIVE_UNIT_Z);
			//Mogre.Entity groundEnt = mSceneMgr.CreateEntity("GroundEntity", "ground");
			////register under root
			//mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(groundEnt);
			//groundEnt.CastShadows = false;
		}


		/// <summary>
		/// The function Set lights one is directional and second is spot (to make shadows like sunshine)
		/// </summary>
		private void SetSun() {

			Mogre.Light directionalLight = mSceneMgr.CreateLight("directionalLight");
			directionalLight.Type = Mogre.Light.LightTypes.LT_DIRECTIONAL;
			directionalLight.DiffuseColour = Mogre.ColourValue.White;
			directionalLight.SpecularColour = Mogre.ColourValue.Blue;
			directionalLight.Direction = new Mogre.Vector3(0, -1000, 0);

			Mogre.Light pointLight = mSceneMgr.CreateLight("pointLight");
			pointLight.Type = Mogre.Light.LightTypes.LT_POINT;
			pointLight.DiffuseColour = Mogre.ColourValue.White;
			pointLight.SpecularColour = Mogre.ColourValue.Blue;
			pointLight.Position = new Mogre.Vector3(0, 0, 0);

			Mogre.Light spotLight = mSceneMgr.CreateLight("spotLight");
			spotLight.Type = Mogre.Light.LightTypes.LT_SPOTLIGHT;
			spotLight.DiffuseColour = Mogre.ColourValue.White;
			spotLight.SpecularColour = Mogre.ColourValue.Blue;
			spotLight.Direction = new Mogre.Vector3(-1, -1, 0);
			spotLight.Position = new Mogre.Vector3(-50, 5000, -3000);

		}

		/// <summary>
		/// The function Set sky and fog
		/// </summary>
		private void SetSky() {
			//mSceneMgr.SetSkyDome(true, "Examples/SpaceSkyBox", 10, 20);
			mSceneMgr.SetFog(Mogre.FogMode.FOG_EXP2, fadeColour, 0.0003f);
		}
	}


}