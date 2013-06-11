using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;
using Strategy.GameObjectControl.Game_Objects.GameActions;

namespace Strategy.GameGUI {
	/// <summary>
	/// Extension of the PictureBox which have reference on the action and calls OnMouseClick when user clicks on it.
	/// </summary>
	class GameActionIconBox : PictureBox {
		private IGameAction action;

		/// <summary>
		/// Creates instance of GameActionIconBox and store reference on action. Also adds MouseClick action GameActionClicked.
		/// </summary>
		/// <param name="action"></param>
		public GameActionIconBox(IGameAction action) {
			this.action = action;
			Load(action.IconPath());
			Size = new Miyagi.Common.Data.Size(25, 25);
			MouseClick += GameActionClicked;
		}

		/// <summary>
		/// MouseClick action which calls OnMouseClick() and print answer to the game console.
		/// </summary>
		/// <param name="sender">The sender of action.</param>
		/// <param name="e">The arguments of action.</param>
		private void GameActionClicked(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.PrintToGameConsole(action.OnMouseClick());
		}
	}
}
