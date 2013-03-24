using System;
using System.Collections.Generic;
using IrrKlang;
using Strategy.Game_Objects;
using Strategy.GroupControl;
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

		public static string playerName = "Player";

		protected MOIS.InputManager mInputMgr; //use for create control (mouse, keyborard) instance
		protected float mTimer; //float as timer to determine of duration overlay 
		protected bool exit = false; //controlor if player is alive
		
		protected Mogre.ColourValue fadeColour = new Mogre.ColourValue(0.05f, 0.05f, 0.05f); //color of fog and shadow
		protected static readonly Mogre.Vector3 cameraStart = new Mogre.Vector3(0, 1000, 1000);
		
		protected CameraMan cameraMan;

        protected IGameSoundMaker songMaker; //to make background music

        protected Game myGame;

		public static void Main() {
			new MyMogre().Go();

		}
		#region Create world and camera

		/// <summary>
		/// This overriden class inicializes whole world (objects, mission, sounds, camera)
		/// </summary>
		protected override void CreateScene() {
			#region BAD MIY..
			//createBars();
			#endregion

            myGame.inicialization();

			loadFont();

			setShadow(); //set shadow (color, technique)

			setGround();	//set a floor

			setSun(); //set lights 

			setSky(); //set sky texture

			//hudba<
			songMaker = new SoundMaker("../../media/music", mWindow); //music player


			
		}

		/// <summary>
		/// This method inicializes camere and cameraMan
		/// </summary>
		protected override void CreateCamera() {

			mCamera = mSceneMgr.CreateCamera("myCam");
			mCamera.Position = cameraStart;
			mCamera.LookAt(Mogre.Vector3.ZERO);
			mCamera.NearClipDistance = 50;
			mCameraMan = new Mogre.TutorialFramework.CameraMan(mCamera);
			cameraMan = mCameraMan;
		}

		/// <summary>
		/// The CreateViewports set camera and gives viewport to camera
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
		/// antibug
		/// </summary>
		private void loadFont(){
			Mogre.FontManager.Singleton.Load("BlueHighway", Mogre.ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
		}
		#region Animation


		//float barval = 0;
		/// <summary>
		/// This method is called in each time in game loop method update scene and call walk method of
		/// movable objects
		/// </summary>
		/// <param name="evt">delay between last frames</param>
		protected override void UpdateScene(Mogre.FrameEvent evt) {

			float f = evt.timeSinceLastFrame;
			myGame.update(f);
			base.UpdateScene(evt);
			if (mTimer > 0) { //if overlay showed
				mTimer -= f;
				if (mTimer <= 0) {
					Mogre.OverlayManager.Singleton.GetByName("Author").Hide(); //timer is timeout -> hide overlay
					exit = false;
				}
			}

			//groupManager.update(evt.timeSinceLastEvent);
			songMaker.hideBox(f);

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


            mMouse.MousePressed += new MOIS.MouseListener.MousePressedHandler(myGame.getMouseControl().OnMyMousePressed);
			mMouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(myGame.getMouseControl().OnMyMouseReleased);
			mMouse.MouseMoved += new MOIS.MouseListener.MouseMovedHandler(myGame.getMouseControl().OnMyMouseMoved);
		}

		#region Keyboard control


		/// <summary>
		/// This function is called when any key is pressed
		/// </summary>
		/// <param name="evt">which button was pressed</param>
		/// <returns>key was pressed -> true</returns>
		protected override bool OnKeyPressed(MOIS.KeyEvent evt) {
			base.OnKeyPressed(evt);
			switch (evt.key) {
				case MOIS.KeyCode.KC_RETURN:
					if (exit) {
						try {
							Shutdown();
						} catch (System.Exception) {
							quit();
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
				//Music section
				case MOIS.KeyCode.KC_NUMPAD9:
					songMaker.volumeUp();
					break;
				case MOIS.KeyCode.KC_NUMPAD6:
					songMaker.volumeDown();
					break;
				case MOIS.KeyCode.KC_N:
					songMaker.stopActualSong();
					break;
				case MOIS.KeyCode.KC_M:
					songMaker.pause();
					break;
				case MOIS.KeyCode.KC_B:
					songMaker.actualPlaying();
					break;
				//End of music section
				case MOIS.KeyCode.KC_R:
					restartCamera();
					break;

				case MOIS.KeyCode.KC_ESCAPE:
					quit();
					break;
			}

			return true;
		}


		#endregion

		private void quit() {
			myGame.quit();
			throw new ShutdownException();
		}

		private void restartCamera(){
			mCamera.Position = cameraStart;
			mCamera.LookAt(Mogre.Vector3.ZERO);
		}

		#region Mouse control

		

		

		#endregion

		/// <summary>
		/// This override function set basic setting to RenderSystem (Setup RenderSystem by code)
		/// </summary>
		/// <returns></returns>
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
		/// The setShadow() is function to set color and type of shadows
		/// </summary>
		private void setShadow() {
			mSceneMgr.AmbientLight = new Mogre.ColourValue(.1f, .1f, .1f);
			mSceneMgr.ShadowTechnique = Mogre.ShadowTechnique.SHADOWTYPE_STENCIL_ADDITIVE;
		}

		/// <summary>
		/// The function to set the ground
		/// </summary>
		private void setGround() {
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
		/// The function set lights one is directional and second is spot (to make shadows like sunshine)
		/// </summary>
		private void setSun() {

			Mogre.Light directionalLight = mSceneMgr.CreateLight("directionalLight");
			directionalLight.Type = Mogre.Light.LightTypes.LT_DIRECTIONAL;
			directionalLight.DiffuseColour =  Mogre.ColourValue.White;
			directionalLight.SpecularColour =  Mogre.ColourValue.Blue;
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
		/// The function set sky and fog
		/// </summary>
		private void setSky() {
			//mSceneMgr.SetSkyDome(true, "Examples/SpaceSkyBox", 10, 20);
			mSceneMgr.SetFog(Mogre.FogMode.FOG_EXP2, fadeColour, 0.0003f);
		}
	}


}