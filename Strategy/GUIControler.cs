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

namespace Strategy {
	class GUIControler {
		protected static MyGUI myGUI;
        //protected List<IMaterial> materials; //maybe just number of it ...and names ...and make value list

        private static GUIControler instance;

        public static GUIControler getInstance(RenderWindow mWindow, Mouse m, Keyboard k, List<IMaterial> listMaterial) {
            if (instance==null) {
                instance = new GUIControler(mWindow, m, k, listMaterial);
            }
            return instance;
        }

        private GUIControler(RenderWindow mWindow, Mouse m, Keyboard k, List<IMaterial> listMaterial) {
			myGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, m, k, listMaterial);
            //materials = listMaterial;
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

	}

}
