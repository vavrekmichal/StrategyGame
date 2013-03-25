using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	class SelectionLabel : Label {
		protected int numberOfItem;
		protected Panel panelToClose;

		public SelectionLabel(int position, Panel panel)
			: base() {
			numberOfItem = position;
			panelToClose = panel;
		}

		public int NumberOfItem {
			get { return numberOfItem; }
		}

		public Panel PanelToClose {
			get { return panelToClose; }
		}
	}
}
