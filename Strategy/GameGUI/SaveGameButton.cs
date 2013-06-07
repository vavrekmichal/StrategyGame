using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	class SaveGameButton : CloseButton{

		TextBox textBox;
		PanelType panelToClose;


		public SaveGameButton(PanelType panel, TextBox textBox) :base(panel){
			this.textBox = textBox;
			panelToClose = panel;
			MouseClick += SaveGame;
		}


		private void SaveGame(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {

			var casted = sender as SaveGameButton;
			if (casted != null) {
				var saveName = textBox.Text;
				if (saveName == "") {
					saveName = "NoName";
				}
				Game.Save(saveName + ".save");
				Game.IGameGUI.ClosePanel(panelToClose);
			}
		}
	}
}
