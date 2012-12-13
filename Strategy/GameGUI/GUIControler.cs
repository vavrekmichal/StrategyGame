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
        //protected List<IMaterial> materials; //maybe just number of it ...and names ...and make value list

        private static GUIControler instance;

        public static GUIControler getInstance(RenderWindow mWindow, Mouse m, Keyboard k) {
            if (instance==null) {
                instance = new GUIControler(mWindow, m, k);
            }
            return instance;
        }

        private GUIControler(RenderWindow mWindow, Mouse m, Keyboard k) {
            window = mWindow;
            mouse = m;
            keyboard = k;
            //materials = listMaterial;
		}

        public void inicialization( Dictionary<string, IMaterial> listMaterial) {
            myGUI = new MyGUI((int)window.Width, (int)window.Height, mouse, keyboard, listMaterial);
        }

		public void dispose() {
			myGUI.dispose();
		}

		public void update() {
			myGUI.update();
		}

		public static void targetObject(string s) {
			myGUI.showTargeted(s);
		}

        public void setSolarSystemName(string name) {
            myGUI.setSolarSystemName(name);
        }

        public void setMaterialState(string material, int inc) {
            myGUI.setMaterialState(material, inc);
        }
	}

}
