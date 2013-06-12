using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	/// <summary>
	/// Extension of the Button which close the type of panel.
	/// </summary>
	class CloseButton : Button {

		private string panelToClose;

		/// <summary>
		/// Creates instance of the Button with infomation about closing panel type or panel reference. Also adds MouseClick action DisposePanel.
		/// </summary>
		/// <param name="type">The type of the closing panel.</param>
		/// <param name="panel">The reference on the closing Panel.</param>
		public CloseButton(string panelName)
			: base() {
			panelToClose = panelName;
			MouseClick += DisposePanel;
		}

		/// <summary>
		/// MouseClick action which closing setted type of the panel.
		/// </summary>
		/// <param name="sender">The sender of the action.</param>
		/// <param name="e">The arguments of the action.</param>
		private void DisposePanel(object sender, Miyagi.Common.Events.MouseEventArgs e) {
			// Dispose the panel 

			Game.IGameGUI.ClosePanel(panelToClose);

		}

	}
}
