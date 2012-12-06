using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl;
using Mogre;
using Strategy.TeamControl;

namespace Strategy.MogreControl {
	class MouseControl {
		protected static Mogre.TutorialFramework.CameraMan cameraMan ;
		protected Mogre.SceneManager sceneMgr;
		protected TeamManager teamManager;

        protected int changeMe = 1;

        public MouseControl(Mogre.TutorialFramework.CameraMan c, Mogre.SceneManager m, TeamManager t) {
			cameraMan = c;
			sceneMgr = m;
            teamManager = t;
		}

		public void print() {
			Console.WriteLine("povedlo se");
		}

		/// <summary>
		/// This special function is called when any mouse button is pressed
		/// </summary>
		/// <param name="arg">argument of press</param>
		/// <param name="id">which button was pressed</param>
		/// <returns>was pressed true</returns>
		public bool OnMyMousePressed(MOIS.MouseEvent evt, MOIS.MouseButtonID id) {


            if (id == MOIS.MouseButtonID.MB_Left) {
                //mCameraMan.Freeze = true;
                using (Mogre.RaySceneQuery raySceneQuery = sceneMgr.CreateRayQuery(new Mogre.Ray())) {
                    float mouseX = (float)evt.state.X.abs / (float)evt.state.width;
                    float mouseY = (float)evt.state.Y.abs / (float)evt.state.height;

                    Mogre.Ray mouseRay = cameraMan.getCamera().GetCameraToViewportRay(mouseX, mouseY);
                    raySceneQuery.Ray = mouseRay;
                    raySceneQuery.SetSortByDistance(true);

                    using (Mogre.RaySceneQueryResult result = raySceneQuery.Execute()) {
                        foreach (Mogre.RaySceneQueryResultEntry entry in result) {
                            if (entry.movable.Name != "GroundEntity") {
                                GUIControler.targetObject(entry.movable.Name);
                            }

                        }
                    }
                }

            } else {
                teamManager.changeSolarSystem(changeMe);
                changeMe=(changeMe + 1) % 2;
            }
			return true;
		}

		/// <summary>
		/// This special function is called when any mouse button is released
		/// </summary>
		/// <param name="arg">argument of release</param>
		/// <param name="id">which button was released</param>
		/// <returns>was released true</returns>
		public bool OnMyMouseReleased(MOIS.MouseEvent arg, MOIS.MouseButtonID id) {
			if (id == MOIS.MouseButtonID.MB_Left) { }
			//mCameraMan.Freeze = false;

			return true;
		}


		//static method tests
		public static void move(string s) {
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

		public static void moveStop(string s) {
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
}
