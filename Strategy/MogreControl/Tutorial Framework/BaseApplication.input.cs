﻿using MOIS;
using System;
using Strategy;

namespace Mogre.TutorialFramework {
	public abstract partial class BaseApplication {
		protected Keyboard mKeyboard;
		protected Mouse mMouse;

		//pravin inserted
		protected MOIS.InputManager inputMgr;

		protected virtual void InitializeInput() {
			LogManager.Singleton.LogMessage("*** Initializing OIS ***");

			mRenderWindow = mWindow;

			int windowHnd;
			mWindow.GetCustomAttribute("WINDOW", out windowHnd);
			inputMgr = MOIS.InputManager.CreateInputSystem((uint)windowHnd);

			mKeyboard = (MOIS.Keyboard)inputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
			mMouse = (MOIS.Mouse)inputMgr.CreateInputObject(MOIS.Type.OISMouse, true);

			MOIS.MouseState_NativePtr state = mMouse.MouseState;
			state.width = mWindow.GetViewport(0).ActualWidth;
			state.height = mWindow.GetViewport(0).ActualHeight;

			mKeyboard.KeyPressed += new KeyListener.KeyPressedHandler(OnKeyPressed);
			mKeyboard.KeyReleased += new KeyListener.KeyReleasedHandler(OnKeyReleased);
			mMouse.MouseMoved += new MouseListener.MouseMovedHandler(OnMouseMoved);
			mMouse.MousePressed += new MouseListener.MousePressedHandler(OnMousePressed);
			mMouse.MouseReleased += new MouseListener.MouseReleasedHandler(OnMouseReleased);
		}

		protected void ProcessInput() {
			mKeyboard.Capture();
			mMouse.Capture();
		}

		protected virtual bool OnKeyPressed(KeyEvent evt) {
			if (!Game.KeyboardCaptured) {
				switch (evt.key) {
					case KeyCode.KC_W:
						mCameraMan.GoingUp = true;
						mCameraMan.GoingForward = true;
						break;
					case KeyCode.KC_UP:
						mCameraMan.GoingForward = true;
						break;

					case KeyCode.KC_S:
						mCameraMan.GoingDown = true;
						mCameraMan.GoingBack = true;
						break;
					case KeyCode.KC_DOWN:
						mCameraMan.GoingBack = true;
						break;

					case KeyCode.KC_A:
					case KeyCode.KC_LEFT:
						mCameraMan.GoingLeft = true;
						break;

					case KeyCode.KC_D:
					case KeyCode.KC_RIGHT:
						mCameraMan.GoingRight = true;
						break;

					case KeyCode.KC_E:
					case KeyCode.KC_PGUP:
						mCameraMan.GoingUp = true;
						break;

					case KeyCode.KC_Q:
					case KeyCode.KC_PGDOWN:
						mCameraMan.GoingDown = true;
						break;

					case KeyCode.KC_LSHIFT:
					case KeyCode.KC_RSHIFT:
						mCameraMan.FastMove = true;
						break;

					case KeyCode.KC_T:
						CycleTextureFilteringMode();
						break;

					case KeyCode.KC_SYSRQ:
						TakeScreenshot();
						break;
				}
			}
			return true;
		}

		protected virtual bool OnKeyReleased(KeyEvent evt) {
			if (!Game.KeyboardCaptured) {
				switch (evt.key) {
					case KeyCode.KC_F5:
						Game.Save("QuickSave.save");
						break;
					case KeyCode.KC_W:
						mCameraMan.GoingForward = false;
						mCameraMan.GoingUp = false;
						break;
					case KeyCode.KC_UP:
						mCameraMan.GoingForward = false;
						break;

					case KeyCode.KC_S:
						mCameraMan.GoingBack = false;
						mCameraMan.GoingDown = false;
						break;
					case KeyCode.KC_DOWN:
						mCameraMan.GoingBack = false;
						break;

					case KeyCode.KC_A:
					case KeyCode.KC_LEFT:
						mCameraMan.GoingLeft = false;
						break;

					case KeyCode.KC_D:
					case KeyCode.KC_RIGHT:
						mCameraMan.GoingRight = false;
						break;

					case KeyCode.KC_E:
					case KeyCode.KC_PGUP:
						mCameraMan.GoingUp = false;
						break;

					case KeyCode.KC_Q:
					case KeyCode.KC_PGDOWN:
						mCameraMan.GoingDown = false;
						break;

					case KeyCode.KC_LSHIFT:
					case KeyCode.KC_RSHIFT:
						mCameraMan.FastMove = false;
						break;
				}
			}
			return true;
		}

		protected virtual bool OnMouseMoved(MouseEvent evt) {
			return true;
		}

		protected virtual bool OnMousePressed(MouseEvent evt, MouseButtonID id) {
			return true;
		}

		protected virtual bool OnMouseReleased(MouseEvent evt, MouseButtonID id) {
			return true;
		}
	}
}