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
	public class GUIControler {
		protected static MyGUI myGUI;

		public GUIControler(RenderWindow mWindow, Mouse mouse, Keyboard keyboard) {
			myGUI = new MyGUI((int)mWindow.Width, (int)mWindow.Height, mouse, keyboard);
		}

		/// <summary>
		/// GUI inicialization			
		/// </summary>
		/// <param Name="listMaterial">List with player's materials</param>
		public void Inicialization(Dictionary<string, IMaterial> listMaterial) {

			myGUI.LoadMaterials(listMaterial);
		}


		public void Dispose() {
			myGUI.Dispose();
		}

		public void Update() {
			myGUI.Update();
		}

		public void ShowTargeted(GroupStatics group) {
			myGUI.ShowTargeted(group);
		}

		public void ShowTargeted(GroupMovables group) {
			myGUI.ShowTargeted(group);
		}

		public void SetSolarSystemName(string name) {
			myGUI.SetSolarSystemName(name);
		}

		public void SetMaterialState(string material, int inc) {
			myGUI.SetMaterialState(material, inc);
		}

		public void ShowSolarSystSelectionPanel(List<string> possibilities, string topic, object gameObject) {
			myGUI.ShowSolarSystSelectionPanel(possibilities, topic, gameObject);
		}
	}

}
