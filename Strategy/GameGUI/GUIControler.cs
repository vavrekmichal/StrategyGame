using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameMaterial;
using Strategy.GameGUI;
using Strategy.GameObjectControl.GroupMgr;
using Strategy.GameObjectControl;
using Strategy.TeamControl;
using Mogre;
using MOIS;

namespace Strategy.GameGUI {
	class GUIControler {
		protected static MyGUI myGUI;

		public GUIControler(RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			myGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, mouse, keyboard);
		}

		/// <summary>
		/// GUI inicialization			
		/// </summary>
		/// <param name="listMaterial">List with player's materials</param>
		public void inicialization(Dictionary<string, IMaterial> listMaterial) {

			myGUI.loadMaterials(listMaterial);
		}


		public void dispose() {
			myGUI.dispose();
		}

		public void update() {
			myGUI.update();
		}

		public void showTargeted(GroupStatics group) {
			myGUI.showTargeted(group);
		}

		public void showTargeted(GroupMovables group) {
			myGUI.showTargeted(group);
		}

		public void setSolarSystemName(string name) {
			myGUI.setSolarSystemName(name);
		}

		public void setMaterialState(string material, int inc) {
			myGUI.setMaterialState(material, inc);
		}

		public void showSolarSystSelectionPanel(List<string> possibilities, string topic, object gameObject) {
			myGUI.showSolarSystSelectionPanel(possibilities, topic, gameObject);
		}
	}

}
