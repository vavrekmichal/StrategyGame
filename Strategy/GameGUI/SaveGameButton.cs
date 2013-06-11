using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	/// <summary>
	/// Extension of the CloseButton which has reference to TextBox to get text from it. 
	/// TextBox text is used as name of saving game.
	/// </summary>
	class SaveGameButton : CloseButton {

		TextBox textBox;
		PanelType panelToClose;

		/// <summary>
		/// Creates instance of CloseButton and stored textBox reference. Also adds MouseClick action SaveGame.
		/// </summary>
		/// <param name="panel">The panel type to close.</param>
		/// <param name="textBox">The text box with name of save game.</param>
		public SaveGameButton(PanelType panel, TextBox textBox)
			: base(panel) {
			this.textBox = textBox;
			panelToClose = panel;
			MouseClick += SaveGame;
		}

		/// <summary>
		/// Gets text from textBox and calls Save with the text from textBox. After that close panel.
		/// </summary>
		/// <param name="sender">The sender of action.</param>
		/// <param name="e">The arguments of action.</param>
		private void SaveGame(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			var saveName = textBox.Text;
			if (saveName == "") {
				saveName = "NoName";
			}
			Game.Save(saveName + ".save");
			Game.IGameGUI.ClosePanel(panelToClose);

		}
	}
}
