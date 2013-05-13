using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl;
using Mogre;
using Strategy.TeamControl;
using Mogre.TutorialFramework;
using Strategy.GameGUI;
using MOIS;

namespace Strategy.MogreControl {
	class MouseControl {
		protected static Mogre.TutorialFramework.CameraMan cameraMan;
		protected SceneManager sceneMgr;
		protected GUIControler guiControl; //TODO:mys nebude mit guicko

		protected int changeMe = 1;

		private static MouseControl instance;

		private static float mouseBoundY = BaseApplication.getRenderWindow().Height * 8 / 10;

		// Rectangular Select items
		bool isRectagularSelect;
		Vector2 mStart, mStop;
		List<MovableObject> mSelected = new List<MovableObject>();
		SelectionRectangle mRect;
		bool bSelecting;


		public static MouseControl GetInstance(CameraMan c, SceneManager m, GUIControler guiControl) {
			if (instance == null) {
				instance = new MouseControl(c, m, guiControl);
			}
			return instance;
		}

		private MouseControl(CameraMan c, SceneManager m, GUIControler guiControl) {
			cameraMan = c;
			sceneMgr = m;
			this.guiControl = guiControl;
			mRect = new SelectionRectangle("RectangularSelect");
			sceneMgr.RootSceneNode.CreateChildSceneNode().AttachObject(mRect);
		}


		/// <summary>
		/// This special function is called when any mouse button is pressed
		/// </summary>
		/// <param Name="arg">Argument of press</param>
		/// <param Name="id">Pressed button</param>
		/// <returns>True</returns>
		public bool OnMyMousePressed(MouseEvent arg, MouseButtonID id) {
			if (arg.state.Y.abs > mouseBoundY) {
				return true;
			}
			switch (id) {
				case MouseButtonID.MB_Button3:
					break;
				case MouseButtonID.MB_Button4:
					break;
				case MouseButtonID.MB_Button5:
					break;
				case MouseButtonID.MB_Button6:
					break;
				case MouseButtonID.MB_Button7:
					break;
				case MouseButtonID.MB_Left:
					// Rectangular Select
					mStart.x = (float)arg.state.X.abs / (float)arg.state.width;
					mStart.y = (float)arg.state.Y.abs / (float)arg.state.height;
					mStop = mStart;
					bSelecting = true;

					break;

				case MouseButtonID.MB_Right:
					break;
				default:
					break;
			}

			return true;
		}

		/// <summary>
		/// This special function is called when any mouse button is released
		/// </summary>
		/// <param Name="arg">Argument of release</param>
		/// <param Name="id">Released button</param>
		/// <returns>True</returns>
		public bool OnMyMouseReleased(MouseEvent arg, MouseButtonID id) {
			if (arg.state.Y.abs > mouseBoundY) {
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
					using (Mogre.RaySceneQuery raySceneQuery = sceneMgr.CreateRayQuery(new Mogre.Ray())) {
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


		public bool OnMyMouseMoved(MouseEvent arg) {
			if (arg.state.Y.abs > mouseBoundY) {
				return true;
			}
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

		private List<MovableObject> SimpleClick(MouseEvent arg) {
			using (Mogre.RaySceneQuery raySceneQuery = sceneMgr.CreateRayQuery(new Mogre.Ray())) {
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


		private void PerformSelection(Vector2 first, Vector2 second) {
			float left = first.x, right = second.x,
			top = first.y, bottom = second.y;

			if (left > right) Swap(ref left, ref right);
			if (top > bottom) Swap(ref top, ref bottom);

			if ((right - left) * (bottom - top) < 0.0001) return;

			Camera c = sceneMgr.GetCamera("myCam");
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
			PlaneBoundedVolumeListSceneQuery volQuery = sceneMgr.CreatePlaneBoundedVolumeQuery(volList);
			SceneQueryResult result = volQuery.Execute();

			List<MovableObject> list = new List<MovableObject>(result.movables);

			GameObjectManager.GetInstance().OnLeftClick(list);

			sceneMgr.DestroyQuery(volQuery);
		}

		// Static method tests

		static void Swap(ref float x, ref float y) {
			float tmp = x;
			x = y;
			y = tmp;
		}

		public static void Move(string s) {
			switch (s) {
				case "topSection":
					cameraMan.GoingUp = true;
					cameraMan.GoingForward = true;
					break;
				case "backSection":
					cameraMan.GoingDown = true;
					cameraMan.GoingBack = true;
					break;
				case "leftSection":
					cameraMan.GoingLeft = true;
					break;
				case "rightSection":
					cameraMan.GoingRight = true;
					break;
				case "rightUpSection":
					cameraMan.GoingUp = true;
					cameraMan.GoingForward = true;
					cameraMan.GoingRight = true;
					break;
				case "rightDownSection":
					cameraMan.GoingDown = true;
					cameraMan.GoingBack = true;
					cameraMan.GoingRight = true;
					break;
				case "leftDownSection":
					cameraMan.GoingDown = true;
					cameraMan.GoingBack = true;
					cameraMan.GoingLeft = true;
					break;
				case "leftUpSection":
					cameraMan.GoingUp = true;
					cameraMan.GoingForward = true;
					cameraMan.GoingLeft = true;
					break;
				default: Console.WriteLine("Error mouse move");
					break;
			}

		}

		public static void MoveStop(string s) {
			switch (s) {
				case "topSection":
					cameraMan.GoingUp = false;
					cameraMan.GoingForward = false;
					break;
				case "backSection":
					cameraMan.GoingDown = false;
					cameraMan.GoingBack = false;
					break;
				case "leftSection":
					cameraMan.GoingLeft = false;
					break;
				case "rightSection":
					cameraMan.GoingRight = false;
					break;
				case "rightUpSection":
					cameraMan.GoingUp = false;
					cameraMan.GoingForward = false;
					cameraMan.GoingRight = false;
					break;
				case "rightDownSection":
					cameraMan.GoingDown = false;
					cameraMan.GoingBack = false;
					cameraMan.GoingRight = false;
					break;
				case "leftDownSection":
					cameraMan.GoingDown = false;
					cameraMan.GoingBack = false;
					cameraMan.GoingLeft = false;
					break;
				case "leftUpSection":
					cameraMan.GoingUp = false;
					cameraMan.GoingForward = false;
					cameraMan.GoingLeft = false;
					break;
				default: Console.WriteLine("Error mouse move");
					break;
			}
		}
	}

	public class SelectionRectangle : ManualObject {
		public SelectionRectangle(String name)
			: base(name) {
			RenderQueueGroup = (byte)RenderQueueGroupID.RENDER_QUEUE_OVERLAY; // when using this, ensure Depth Check is Off in the material
			UseIdentityProjection = true;
			UseIdentityView = true;
			QueryFlags = 0;
		}

		/**
		* Sets the corners of the SelectionRectangle.  Every parameter should be in the
		* range [0, 1] representing a percentage of the screen the SelectionRectangle
		* should take up.
		*/
		void SetCorners(float left, float top, float right, float bottom) {
			left = left * 2 - 1;
			right = right * 2 - 1;
			top = 1 - top * 2;
			bottom = 1 - bottom * 2;
			Clear();
			Begin("", RenderOperation.OperationTypes.OT_LINE_STRIP);
			Position(left, top, -1);
			Position(right, top, -1);
			Position(right, bottom, -1);
			Position(left, bottom, -1);
			Position(left, top, -1);
			End();
			BoundingBox.SetInfinite();
		}

		public void SetCorners(Vector2 topLeft, Vector2 bottomRight) {

			SetCorners(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
		}
	}
}
