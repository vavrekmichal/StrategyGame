using Mogre.TutorialFramework;

namespace Strategy {
	/// <summary>
	/// Main class derives BaseApplication for easier work with MOGRE and initialzes the Mogre framework.
	/// </summary>
	class MyMogre : Mogre.TutorialFramework.BaseApplication {

		/// <summary>
		/// The m input MGR
		/// </summary>
		protected MOIS.InputManager mInputMgr; // Use for create control (mouse, keyborard) instance

		/// <summary>
		/// The fade colour
		/// </summary>
		protected Mogre.ColourValue fadeColour = new Mogre.ColourValue(0.05f, 0.05f, 0.05f); // Color of fog and shadow
		/// <summary>
		/// The camera start
		/// </summary>
		protected static readonly Mogre.Vector3 cameraStart = new Mogre.Vector3(0, 1000, 1000);

		/// <summary>
		/// The camera man
		/// </summary>
		protected CameraMan cameraMan;
		/// <summary>
		/// My game
		/// </summary>
		protected Game myGame;

		/// <summary>
		/// Starts the aplication.
		/// </summary>
		public static void Main() {
			new MyMogre().Go();
		}

		#region Create world and camera

		/// <summary>
		/// Initializes the MOGRE world.
		/// (shadows, lights, sky and loads fonts)
		/// </summary>
		protected override void CreateScene() {
			LoadFont();
			SetShadow(); 
			SetLight(); 
			SetFog(); 
		}

		/// <summary>
		/// Initializes camera and cameraMan. Sets the near clip and far clip distance.
		/// </summary>
		protected override void CreateCamera() {
			mCamera = mSceneMgr.CreateCamera("myCam");
			mCamera.Position = cameraStart;
			mCamera.LookAt(Mogre.Vector3.ZERO);
			mCamera.NearClipDistance = 1;
			mCamera.FarClipDistance = 60000;
			mCameraMan = new Mogre.TutorialFramework.CameraMan(mCamera);
			cameraMan = mCameraMan;
		}

		/// <summary>
		/// Sets camera and sets the viewport to camera.
		/// </summary>
		protected override void CreateViewports() {
			Mogre.Viewport viewport = mWindow.AddViewport(mCamera);
			viewport.BackgroundColour = new Mogre.ColourValue(0.1f, 0.1f, 0.1f);
			mCamera.AspectRatio = (float)viewport.ActualWidth / viewport.ActualHeight;
		}

		#endregion

		/// <summary>
		/// Inicializes SceneManager.
		/// </summary>
		protected override void ChooseSceneManager() {
			mSceneMgr = mRoot.CreateSceneManager(Mogre.SceneType.ST_EXTERIOR_CLOSE);
		}

		/// <summary>
		/// Loads fonts to Mogre system.
		/// </summary>
		private void LoadFont() {
			Mogre.FontManager.Singleton.Load("BlueHighway", Mogre.ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
		}

		#region Update

		/// <summary>
		/// Updates the Game and the Mogre system.
		/// </summary>
		/// <param name="evt">The evt.</param>
		protected override void UpdateScene(Mogre.FrameEvent evt) {
			float f = evt.timeSinceLastFrame;

			myGame.Update(f);
			base.UpdateScene(evt);
		}
		#endregion

		/// <summary>
		/// Initializes the MOIS event listeners (mouse and keyboard).
		/// </summary>
		protected override void InitializeInput() {
			base.InitializeInput();

			int windowHandle;
			mWindow.GetCustomAttribute("WINDOW", out windowHandle);
			mInputMgr = MOIS.InputManager.CreateInputSystem((uint)windowHandle);

			myGame = Game.GetInstance(mSceneMgr, cameraMan, mWindow, mMouse, mKeyboard);


			mMouse.MousePressed += new MOIS.MouseListener.MousePressedHandler(myGame.GetMouseControl().OnMyMousePressed);
			mMouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(myGame.GetMouseControl().OnMyMouseReleased);
			mMouse.MouseMoved += new MOIS.MouseListener.MouseMovedHandler(myGame.GetMouseControl().OnMyMouseMoved);
		}

		/// <summary>
		/// Processes input from keyboard (MusicPlayer control, Game pause, save or exit).
		/// </summary>
		/// <param name="evt">The evt.</param>
		/// <returns>
		/// Returns always true.
		/// </returns>
		protected override bool OnKeyPressed(MOIS.KeyEvent evt) {
			base.OnKeyPressed(evt);
			switch (evt.key) {
				case MOIS.KeyCode.KC_RETURN:
					if (Game.Initialized) {
						Game.Paused = !Game.Paused;
					}
					break;
				case MOIS.KeyCode.KC_NUMPAD9:
					Game.IGameSoundMakerPlayer.VolumeUp();
					break;
				case MOIS.KeyCode.KC_NUMPAD6:
					Game.IGameSoundMakerPlayer.VolumeDown();
					break;
				case MOIS.KeyCode.KC_N:
					Game.IGameSoundMakerPlayer.PlayNextSong();
					break;
				case MOIS.KeyCode.KC_M:
					Game.IGameSoundMakerPlayer.Pause();
					break;
				case MOIS.KeyCode.KC_B:
					Game.IGameSoundMakerPlayer.ShowCurrentlyPlayingSong();
					break;
				case MOIS.KeyCode.KC_R:
					RestartCamera();
					break;
				case MOIS.KeyCode.KC_ESCAPE:
					// Close the program.
					myGame.Quit();
					break;
				case MOIS.KeyCode.KC_F5:
					Game.Save("QuickSave.save");
					break;
			}
			return true;
		}

		/// <summary>
		/// Restarts the camera position. Sets start position and sets look at ZERO vector.
		/// </summary>
		private void RestartCamera() {
			mCamera.Position = cameraStart;
			mCamera.LookAt(Mogre.Vector3.ZERO);
		}


		/// <summary>
		/// Sets rendering window properties and rendering system.
		/// (OpenGL, 1280x720, Window mode)
		/// </summary>
		/// <returns>
		/// Always returns true.
		/// </returns>
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
		/// Sets the shadow.
		/// </summary>
		private void SetShadow() {
			mSceneMgr.AmbientLight = new Mogre.ColourValue(.1f, .1f, .1f);
			mSceneMgr.ShadowTechnique = Mogre.ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE;
		}

		/// <summary>
		/// Sets lights to imitate sunlight.
		/// </summary>
		private void SetLight() {

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

			pointLight = mSceneMgr.CreateLight("pointLight2");
			pointLight.Type = Mogre.Light.LightTypes.LT_POINT;
			pointLight.DiffuseColour = Mogre.ColourValue.White;
			pointLight.SpecularColour = Mogre.ColourValue.Blue;
			pointLight.Position = new Mogre.Vector3(0, 1000, 0);

			pointLight = mSceneMgr.CreateLight("pointLight3");
			pointLight.Type = Mogre.Light.LightTypes.LT_POINT;
			pointLight.DiffuseColour = Mogre.ColourValue.White;
			pointLight.SpecularColour = Mogre.ColourValue.Blue;
			pointLight.Position = new Mogre.Vector3(0, -1000, 0);

			Mogre.Light spotLight = mSceneMgr.CreateLight("spotLight");
			spotLight.Type = Mogre.Light.LightTypes.LT_SPOTLIGHT;
			spotLight.DiffuseColour = Mogre.ColourValue.White;
			spotLight.SpecularColour = Mogre.ColourValue.Blue;
			spotLight.Direction = new Mogre.Vector3(-1, -1, 0);
			spotLight.Position = new Mogre.Vector3(-50, 5000, -3000);
		}


		/// <summary>
		/// Sets the fog.
		/// </summary>
		private void SetFog() {
			mSceneMgr.SetFog(Mogre.FogMode.FOG_EXP2, fadeColour, 0.0003f);
		}
	}


}