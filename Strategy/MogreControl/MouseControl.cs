using System;
using System.Collections.Generic;
using Strategy.GameObjectControl;
using Mogre;
using Mogre.TutorialFramework;
using MOIS;

namespace Strategy.MogreControl {
	/// <summary>
	/// Controls mouse movement and mouse clicks.
	/// </summary>
	public class MouseControl {
		protected static Mogre.TutorialFramework.CameraMan cameraMan;

		private static MouseControl instance;

		private static float mouseBoundY1 = BaseApplication.getRenderWindow().Height * 8 / 10;
		private static float mouseBoundY2 = BaseApplication.getRenderWindow().Height / 15;

		int upBorder;
		int leftBorder;
		int rightBorder;
		int bottomBorder;

		int upBorderStop;
		int leftBorderStop;
		int rightBorderStop;
		int bottomBorderStop;

		// Selection Rectangle items
		bool isRectagularSelect;
		Vector2 mStart, mStop;
		List<MovableObject> mSelected = new List<MovableObject>();
		SelectionRectangle mRect;
		bool bSelecting;

		/// <summary>
		/// Initializes MouseControl and stores CameraMan reference. 
		/// </summary>
		/// <param name="c">The reference to the game CameraMan.</param>
		/// <param name="sceneWidth">The width of the game window.</param>
		/// <param name="sceneHeight">The height of the game window.</param>
		/// <returns>Return initialized singleton instance.</returns>
		public static MouseControl GetInstance(CameraMan c, int sceneWidth, int sceneHeight) {
			if (instance == null) {
				instance = new MouseControl(c, sceneWidth, sceneHeight);
			}
			return instance;
		}

		/// <summary>
		/// Creates SelectionRectangle which is used for plane selects and registers it at SceneManager.
		/// Also sets the borders (for camera man movement borders).
		/// </summary>
		/// <param name="c">The reference to the game CameraMan.</param>
		/// <param name="sceneWidth">The width of the game window.</param>
		/// <param name="sceneHeight">The height of the game window.</param>
		private MouseControl(CameraMan c, int sceneWidth, int sceneHeight) {
			var stopBoundX = sceneHeight / 20;
			var stopBoundY = sceneWidth / 15;
			upBorder = sceneHeight / 6; ;
			upBorderStop = upBorder + stopBoundX;
			leftBorder = sceneWidth / 15;
			leftBorderStop = leftBorder + stopBoundY;
			rightBorder = sceneWidth - sceneWidth / 15;
			rightBorderStop = rightBorder - stopBoundY;
			bottomBorder = sceneHeight * 7 / 10;
			bottomBorderStop = bottomBorder - stopBoundX;

			cameraMan = c;
			mRect = new SelectionRectangle("RectangularSelect");
			Game.SceneManager.RootSceneNode.CreateChildSceneNode().AttachObject(mRect);
		}


		/// <summary>
		/// Controls if the mouse is in active area and is not captured. If the left button is pressed
		/// updates the selection rectangle.
		/// </summary>
		/// <param Name="arg">The argument of a mouse pressed.</param>
		/// <param Name="id">The pressed button.</param>
		/// <returns>Always returns true.</returns>
		public bool OnMyMousePressed(MouseEvent arg, MouseButtonID id) {
			if (arg.state.Y.abs > mouseBoundY1 || arg.state.Y.abs < mouseBoundY2 || Game.MouseCaptured) {
				StopCameremanMove();
				return true;
			}
			switch (id) {
				case MouseButtonID.MB_Left:
					StopCameremanMove();
					// Updates the selection rectangle.
					mStart.x = (float)arg.state.X.abs / (float)arg.state.width;
					mStart.y = (float)arg.state.Y.abs / (float)arg.state.height;
					mStop = mStart;
					bSelecting = true;
					break;
				default:
					break;
			}

			return true;
		}

		/// <summary>
		/// Controls if the mouse is in active area and is not captured. If the left button is pressed
		/// checks if is simple click or the selection rectangle. Selets objects and reports it to GameObjectManager
		/// (OnLeftClick). Else if the right click gets object on clicked point and reports it to GameObjectManager
		/// (OnRightClick).
		/// </summary>
		/// <param Name="arg">The argument of a mouse released.</param>
		/// <param Name="id">The released button</param>
		/// <returns>Always returns true.</returns>
		public bool OnMyMouseReleased(MouseEvent arg, MouseButtonID id) {
			if (arg.state.Y.abs > mouseBoundY1 || arg.state.Y.abs < mouseBoundY2 || Game.MouseCaptured) {
				StopCameremanMove();
				return true;
			}
			switch (id) {
				case MouseButtonID.MB_Left:
					bSelecting = false;
					if (isRectagularSelect) {
						PerformSelection(mStart, mStop);
						mRect.Visible = false;
						isRectagularSelect = false;
					} else {
						GameObjectManager.GetInstance().OnLeftClick(SimpleClick(arg));
					}
					break;
				case MouseButtonID.MB_Right:
					Plane plane = new Plane(Mogre.Vector3.UNIT_Y, 0);
					Mogre.Vector3 v;
					using (Mogre.RaySceneQuery raySceneQuery = Game.SceneManager.CreateRayQuery(new Mogre.Ray())) {
						float mouseX = (float)arg.state.X.abs / (float)arg.state.width;
						float mouseY = (float)arg.state.Y.abs / (float)arg.state.height;

						Mogre.Ray mouseRay = cameraMan.getCamera().GetCameraToViewportRay(mouseX, mouseY);
						v = mouseRay.GetPoint(mouseRay.Intersects(plane).second);
					}
					GameObjectManager.GetInstance().OnRightClick(v, SimpleClick(arg));
					break;
			}
			return true;
		}

		/// <summary>
		/// Controls if the mouse is in active area and is not captured. 
		/// (Right button)If the selection rectangle is active updates bounds and sets new corner.
		/// (Middle button) Rotats with the game CameraMan.
		/// (Mouse wheel) Zoom out/in by the argument.
		/// </summary>
		/// <param name="arg">The argument of a mouse mouved.</param>
		/// <returns>Always returns true.</returns>
		public bool OnMyMouseMoved(MouseEvent arg) {
			if (arg.state.Y.abs > mouseBoundY1 || arg.state.Y.abs < mouseBoundY2 || Game.MouseCaptured) {
				StopCameremanMove();
				return true;
			}

			CheckMousePosition(arg);

			if (bSelecting) {
				if (!isRectagularSelect) {
					mRect.Clear();
					mRect.Visible = true;
					isRectagularSelect = true;
				}
				mStop.x = arg.state.X.abs / (float)arg.state.width;
				mStop.y = arg.state.Y.abs / (float)arg.state.height;

				mRect.SetCorners(mStart, mStop);
			}
			if (arg.state.ButtonDown(MouseButtonID.MB_Middle)) {
				cameraMan.MouseMovement(arg.state.X.rel, arg.state.Y.rel);
			}
			if (arg.state.Z.rel != 0) {
				cameraMan.MouseZoom(arg.state.Z.rel / 4);
			}
			return true;
		}

		/// <summary>
		/// Stops movement of the CameraMan.
		/// </summary>
		private void StopCameremanMove() {
			cameraMan.GoingUp = false;
			cameraMan.GoingForward = false;
			cameraMan.GoingBack = false;
			cameraMan.GoingLeft = false;
			cameraMan.GoingRight = false;
			cameraMan.GoingDown = false;
		}

		/// <summary>
		/// Check if the mouse is at the botders. If the mouse is in that position, so sets
		/// appropriate CameraMan move indicators.
		/// </summary>
		/// <param name="arg">The arguments with mouse position.</param>
		private void CheckMousePosition(MouseEvent arg) {
			// Checks move up
			if (arg.state.Y.abs < upBorderStop) {
				if (arg.state.Y.abs < upBorder) {
					// Move up
					cameraMan.GoingUp = true;
					cameraMan.GoingForward = true;
				} else {
					// Stop move up
					cameraMan.GoingUp = false;
					cameraMan.GoingForward = false;
				}
			}

			// Checks move left
			if (arg.state.X.abs < leftBorderStop) {
				if (arg.state.X.abs < leftBorder) {
					// Move left
					cameraMan.GoingLeft = true;
				} else {
					// Stop move left
					cameraMan.GoingLeft = false;
				}
			}

			// Checks move right
			if (arg.state.X.abs > rightBorderStop) {
				if (arg.state.X.abs > rightBorder) {
					// Move right
					cameraMan.GoingRight = true;
				} else {
					// Stop move right
					cameraMan.GoingRight = false;
				}
			}

			// Checks move down
			if (arg.state.Y.abs > bottomBorderStop) {
				if (arg.state.Y.abs > bottomBorder) {
					// Move down
					cameraMan.GoingDown = true;
					cameraMan.GoingBack = true;
				} else {
					// Stop move down
					cameraMan.GoingDown = false;
					cameraMan.GoingBack = false;
				}	
			}
		}

		/// <summary>
		/// Casts the Ray and gets hitted objects. Hitted objects returns in the List.
		/// </summary>
		/// <param name="arg">The arguments with mouse position.</param>
		/// <returns>The List with hitted objects.</returns>
		private List<MovableObject> SimpleClick(MouseEvent arg) {
			using (Mogre.RaySceneQuery raySceneQuery = Game.SceneManager.CreateRayQuery(new Mogre.Ray())) {
				float mouseX = (float)arg.state.X.abs / (float)arg.state.width;
				float mouseY = (float)arg.state.Y.abs / (float)arg.state.height;

				Mogre.Ray mouseRay = cameraMan.getCamera().GetCameraToViewportRay(mouseX, mouseY);
				raySceneQuery.Ray = mouseRay;
				raySceneQuery.SetSortByDistance(true);

				List<MovableObject> list = new List<MovableObject>();
				using (Mogre.RaySceneQueryResult result = raySceneQuery.Execute()) {
					foreach (Mogre.RaySceneQueryResultEntry entry in result) {
						list.Add(entry.movable);
					}
				}
				return list;
			}
		}

		/// <summary>
		/// Casts the plane selection and selects all objects inside the SelectionRectangle. 
		/// Hitted objects sends to the GameObjectManager (OnLeftClick).
		/// </summary>
		/// <param name="first">The firts corner.</param>
		/// <param name="second">The second corner.</param>
		private void PerformSelection(Vector2 first, Vector2 second) {
			float left = first.x, right = second.x,
			top = first.y, bottom = second.y;

			if (left > right) Swap(ref left, ref right);
			if (top > bottom) Swap(ref top, ref bottom);

			if ((right - left) * (bottom - top) < 0.0001) return;

			Camera c = Game.SceneManager.GetCamera("myCam");
			Ray topLeft = c.GetCameraToViewportRay(left, top);
			Ray topRight = c.GetCameraToViewportRay(right, top);
			Ray bottomLeft = c.GetCameraToViewportRay(left, bottom);
			Ray bottomRight = c.GetCameraToViewportRay(right, bottom);

			PlaneBoundedVolume vol = new PlaneBoundedVolume();
			vol.planes.Add(new Plane(topLeft.GetPoint(3), topRight.GetPoint(3), bottomRight.GetPoint(3)));    // Front plane
			vol.planes.Add(new Plane(topLeft.Origin, topLeft.GetPoint(100), topRight.GetPoint(100)));         // Top plane
			vol.planes.Add(new Plane(topLeft.Origin, bottomLeft.GetPoint(100), topLeft.GetPoint(100)));       // Left plane
			vol.planes.Add(new Plane(bottomLeft.Origin, bottomRight.GetPoint(100), bottomLeft.GetPoint(100)));// Bottom plane
			vol.planes.Add(new Plane(topRight.Origin, topRight.GetPoint(100), bottomRight.GetPoint(100)));    // Right plane

			PlaneBoundedVolumeList volList = new PlaneBoundedVolumeList();
			volList.Add(vol);
			PlaneBoundedVolumeListSceneQuery volQuery = Game.SceneManager.CreatePlaneBoundedVolumeQuery(volList);
			SceneQueryResult result = volQuery.Execute();

			List<MovableObject> list = new List<MovableObject>(result.movables);

			GameObjectManager.GetInstance().OnLeftClick(list);

			Game.SceneManager.DestroyQuery(volQuery);
		}

		#region Static

		/// <summary>
		/// Swaps given floats.
		/// </summary>
		/// <param name="x">The firts number.</param>
		/// <param name="y">The second number.</param>
		static void Swap(ref float x, ref float y) {
			float tmp = x;
			x = y;
			y = tmp;
		}
		#endregion
	}
}
