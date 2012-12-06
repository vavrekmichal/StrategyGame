using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameMaterial;
using Strategy.GameGUI;
using Strategy.GroupControl;
using Strategy.TeamControl;

namespace Strategy {
	class GUIControler {
		protected static MyGUI myGUI;
        protected List<IMaterial> materials;

		public GUIControler(Mogre.RenderWindow mWindow, MOIS.Mouse m, MOIS.Keyboard k, TeamManager teamMgr) {
			myGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, m, k);
            materials = teamMgr.getMaterials();
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
