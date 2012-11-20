using System;
using System.Collections.Generic;
using IrrKlang;
using Strategy.Game_Objects;
using Strategy.GroupControl;
using Strategy.MogreControl;
using Strategy.Sound;





namespace Strategy {
	/// <summary>
	/// Main class derives BaseApplication for easier work with MOGRE
	/// </summary>
	/// 


	class MyMogre : Mogre.TutorialFramework.BaseApplication {

		protected MOIS.InputManager mInputMgr; //use for create control (mouse, keyborard) instance
		protected Mogre.Light spotLight; //part of the "sun"
		protected float mTimer; //float as timer to determine of duration overlay 
		protected bool exit = false; //controlor if player is alive
		protected IGameSoundMaker songMaker; //to make background music
		protected Mogre.ColourValue fadeColour = new Mogre.ColourValue(0.05f, 0.05f, 0.05f); //color of fog and shadow
		protected static readonly Mogre.Vector3 cameraStart = new Mogre.Vector3(0, 1000, 1000);
		protected MouseControl mouseControl;
		protected Mogre.TutorialFramework.CameraMan cameraMan;
		protected GUIControler panelControler;
		protected GroupManager groupManager;

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
			panelControler = new GUIControler(mWindow, mMouse, mKeyboard);

			//Mogre.Entity e = mSceneMgr.CreateEntity("pokus","SpaceShip1.mesh");
			//Mogre.SceneNode s = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode",new Mogre.Vector3(0,500,0));
			//s.AttachObject(e);

			//Mogre.Entity e2 = mSceneMgr.CreateEntity("pokus2", "SpaceShip2.mesh");
			//Mogre.SceneNode s2 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode2", new Mogre.Vector3(0, 500, -1000));
			//s2.AttachObject(e2);

			//Mogre.Entity e3 = mSceneMgr.CreateEntity("pokus3", "mercury.mesh");
			//Mogre.SceneNode s3 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode3", new Mogre.Vector3(0, 500, 1000));
			//s3.AttachObject(e3);

			//Mogre.Entity e4 = mSceneMgr.CreateEntity("pokus4", "venus.mesh");
			//Mogre.SceneNode s4 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode4", new Mogre.Vector3(500, 500, 1000));
			//s4.AttachObject(e4);
			//Mogre.Entity e5 = mSceneMgr.CreateEntity("pokus5", "earth.mesh");
			//Mogre.SceneNode s5 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode5", new Mogre.Vector3(1000, 500, 1000));
			//s5.AttachObject(e5);
			//Mogre.Entity e6 = mSceneMgr.CreateEntity("pokus6", "mars.mesh");
			//Mogre.SceneNode s6 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode6", new Mogre.Vector3(1500, 500, 1000));
			//s6.AttachObject(e6);
			//Mogre.Entity e7 = mSceneMgr.CreateEntity("pokus7", "jupiter.mesh");
			//Mogre.SceneNode s7 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode7", new Mogre.Vector3(2000, 500, 1000));
			//s7.AttachObject(e7);
			//Mogre.Entity e8 = mSceneMgr.CreateEntity("pokus8", "saturn.mesh");
			//Mogre.SceneNode s8 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode8", new Mogre.Vector3(2500, 500, 1000));
			//s8.AttachObject(e8);
			//Mogre.Entity e9 = mSceneMgr.CreateEntity("pokus9", "uranus.mesh");
			//Mogre.SceneNode s9 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode9", new Mogre.Vector3(3000, 500, 1000));
			//s9.AttachObject(e9);
			//Mogre.Entity e10 = mSceneMgr.CreateEntity("pokus10", "neptune.mesh");
			//Mogre.SceneNode s10 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode10", new Mogre.Vector3(3500, 500, 1000));
			//s10.AttachObject(e10);

			//Mogre.Entity e11 = mSceneMgr.CreateEntity("pokus11", "airplane.mesh");
			//Mogre.SceneNode s11 = mSceneMgr.RootSceneNode.CreateChildSceneNode("pokusNode11", new Mogre.Vector3(0, 500, 500));
			//s11.AttachObject(e11);

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

			
			panelControler.update();
			base.UpdateScene(evt);
			if (mTimer > 0) { //if overlay showed
				mTimer -= evt.timeSinceLastFrame;
				if (mTimer <= 0) {
					Mogre.OverlayManager.Singleton.GetByName("Author").Hide(); //timer is timeout -> hide overlay
					exit = false;
				}
			}
			groupManager.update(evt.timeSinceLastEvent);
			songMaker.hideBox(evt.timeSinceLastFrame);

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

			groupManager = GroupManager.getInstance(mSceneMgr);
			mouseControl = new MouseControl(cameraMan, mSceneMgr, groupManager);

			mMouse.MousePressed += new MOIS.MouseListener.MousePressedHandler(mouseControl.OnMyMousePressed);
			mMouse.MouseReleased += new MOIS.MouseListener.MouseReleasedHandler(mouseControl.OnMyMouseReleased);
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
						} catch (Exception) {
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
				case MOIS.KeyCode.KC_SPACE:
					spotLight.Visible = !spotLight.Visible;
					break;

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
			panelControler.dispose();
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
			//Mogre.MeshManager.Singleton.CreatePlane("ground", Mogre.ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME, plane, 150000, 150000, 200, 200, true, 1, 500, 500, Mogre.Vector3.NEGATIVE_UNIT_Z);
			//Mogre.Entity groundEnt = mSceneMgr.CreateEntity("GroundEntity", "ground");
			//mSceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(groundEnt);
			//groundEnt.SetMaterialName("Examples/SpaceSkyBox");
			//groundEnt.CastShadows = false;
		}


		/// <summary>
		/// The function set lights one is directional and second is spot (to make shadows like sunshine)
		/// </summary>
		private void setSun() {

			Mogre.Light directionalLight = mSceneMgr.CreateLight("directionalLight");
			directionalLight.Type = Mogre.Light.LightTypes.LT_DIRECTIONAL;
			directionalLight.DiffuseColour = new Mogre.ColourValue(.2f, .2f, .2f);
			directionalLight.SpecularColour = new Mogre.ColourValue(.25f, .25f, 0);
			directionalLight.Direction = new Mogre.Vector3(0, -5, 1);

			spotLight = mSceneMgr.CreateLight("spotLight");
			spotLight.Type = Mogre.Light.LightTypes.LT_SPOTLIGHT;
			spotLight.DiffuseColour = Mogre.ColourValue.White;
			spotLight.SpecularColour = Mogre.ColourValue.Blue;
			spotLight.Direction = new Mogre.Vector3(0, -1, -3);
			spotLight.Position = new Mogre.Vector3(0, 5000, 9000);
			spotLight.SetSpotlightRange(new Mogre.Degree(35), new Mogre.Degree(90));
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