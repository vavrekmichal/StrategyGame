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
		protected object storedObject;
		protected PopUpPanelControl isClosed;

		public SelectionLabel(int position, Panel panel)
			: base() {
			numberOfItem = position;
			panelToClose = panel;
		}

		public SelectionLabel(int position, object objectRef, Panel panel, PopUpPanelControl isClosed)
			: this(position, panel) {
			storedObject = objectRef;
			this.isClosed = isClosed;
		}

		public SelectionLabel(int position, object objectRef, Panel panel)
			: this(position, panel) {
			storedObject = objectRef;
			this.isClosed = new PopUpPanelControl(false);
		}

		public int NumberOfItem {
			get { return numberOfItem; }
		}

		public Panel PanelToClose {
			get { return panelToClose; }
		}

		public void ClosePanel() {
			panelToClose.Dispose();
			isClosed.Value = true;
		}

		public object StoredObject {
			get { return storedObject; }
		}
	}
}
