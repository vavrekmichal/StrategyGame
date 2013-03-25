using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameMaterial;
using Strategy.GameGUI;
using Strategy.GroupControl;
using Strategy.TeamControl;
using Mogre;
using MOIS;

namespace Strategy.GameGUI {
	class GUIControler {
		protected static MyGUI myGUI;
        protected RenderWindow window;
        protected Mouse mouse;
        protected Keyboard keyboard;
		protected GroupManager groupMgr;

        private static GUIControler instance;

        public static GUIControler getInstance(RenderWindow mWindow, Mouse m, Keyboard k, GroupManager groupMgr) {
            if (instance==null) {
                instance = new GUIControler(mWindow, m, k, groupMgr);
            }
            return instance;
        }

		private GUIControler(RenderWindow mWindow, Mouse m, Keyboard k, GroupManager groupMgr) {
            window = mWindow;
            mouse = m;
            keyboard = k;
			this.groupMgr = groupMgr;
		}

        public void inicialization( Dictionary<string, IMaterial> listMaterial) {
            myGUI = new MyGUI((int)window.Width, (int)window.Height, mouse, keyboard, listMaterial, groupMgr);
        }

		public void dispose() {
			myGUI.dispose();
		}

		public void update() {
			myGUI.update();
		}

		public void showTargeted(GroupControl.GroupStatics group) {
			myGUI.showTargeted(group);
		}

		public void showTargeted(GroupControl.GroupMovables group) {
			myGUI.showTargeted(group);
		}

        public void setSolarSystemName(string name) {
            myGUI.setSolarSystemName(name);
        }

        public void setMaterialState(string material, int inc) {
            myGUI.setMaterialState(material, inc);
        }
	}

}
