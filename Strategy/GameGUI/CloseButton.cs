using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	class CloseButton:Button {
		protected Panel panelToClose;

		public CloseButton(Panel panel) : base(){
			panelToClose = panel;
		}

		public Panel PanelToClose {
			get { return panelToClose; }
		}
	}
}
