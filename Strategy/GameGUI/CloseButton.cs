using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	class CloseButton : Button {
		protected Panel panelToClose;
		protected PopUpPanelControl isClosed;

		public CloseButton(Panel panel)
			: base() {
			panelToClose = panel;
		}

		public CloseButton(Panel panel, PopUpPanelControl isClosed)
			: base() {
			panelToClose = panel;
			this.isClosed = isClosed;
		}

		public Panel PanelToClose {
			get { return panelToClose; }
		}

		public void ClosePanel() {
			panelToClose.Dispose();
			isClosed.Value = true;
		}

	}
}
