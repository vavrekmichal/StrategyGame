using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameMaterial;
using Strategy.GameGUI;
using Strategy.GroupControl;

namespace Strategy {
	class GUIControler {
		protected static MyGUI myGUI;
        protected List<IMaterial> materials;

		public GUIControler(Mogre.RenderWindow mWindow, MOIS.Mouse m, MOIS.Keyboard k) {
			myGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, m, k);
            materials=GroupManager.getInstance().getMaterials();
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

		

	}

}
