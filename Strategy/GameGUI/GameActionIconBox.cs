using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;
using Strategy.GameObjectControl.Game_Objects.GameActions;

namespace Strategy.GameGUI {
	class GameActionIconBox : PictureBox {
		private IGameAction action;

		public GameActionIconBox(IGameAction action) {
			this.action = action;
			Load(action.IconPath());
			Size = new Miyagi.Common.Data.Size(25, 25);
		}

		public void MouseClicked() {
			Game.PrintToGameConsole(action.OnMouseClick());
		}
	}
}
