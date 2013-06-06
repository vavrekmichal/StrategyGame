using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	class SaveGameButton : CloseButton{

		TextBox textBox;

		public SaveGameButton(Panel panel, TextBox textBox, BoolWrapper isClosed) :base(panel,isClosed){
			this.textBox = textBox;
		}

		public void SaveGame() {
			var saveName = textBox.Text;
			if (saveName=="") {
				saveName = "NoName";
			}
			Game.Save(saveName + ".save");
			ClosePanel();
		}
	}
}
