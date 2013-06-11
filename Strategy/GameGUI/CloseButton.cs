using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	/// <summary>
	/// Extension of the button which close given type of panel.
	/// </summary>
	class CloseButton : Button {

		private PanelType panelToClose;
		
		/// <summary>
		/// Creates instance of Button with infomation about closing panel type.
		/// </summary>
		/// <param name="type">The type of closing panel.</param>
		public CloseButton(PanelType type)
			: base() {
			MouseClick += DisposePanel;
			panelToClose = type;
		}

		/// <summary>
		/// MouseClick action which closing setted type of panel.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DisposePanel(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// Dispose the panel 
			Game.IGameGUI.ClosePanel(panelToClose);
		}

	}
}
