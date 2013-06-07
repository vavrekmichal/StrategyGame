using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	class CloseButton : Button {

		private PanelType panelToClose;

		public CloseButton(PanelType type)
			: base() {
			MouseClick += DisposePanel;
			panelToClose = type;
		}

		private void DisposePanel(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// On click button Dispose the panel 
			Game.IGameGUI.ClosePanel(panelToClose);
		}

	}
}
