using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy {
	class GUIControler {
		protected static Strategy.GameGUI.MyGUI myGUI;

		public GUIControler(Mogre.RenderWindow mWindow, MOIS.Mouse m, MOIS.Keyboard k) {
			myGUI = new Strategy.GameGUI.MyGUI((int)mWindow.Width, (int)mWindow.Height, m, k);
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
