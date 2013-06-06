using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameGUI {
	class PopUpPanelControl {

		bool value;
		public bool Value {
			get { return value; }
			set {
				setValue(value);
			}
		}

		public PopUpPanelControl(bool value) {
			this.value = value;
		}

		public void CaptureMouse() {
			Game.MouseCaptured = true;
		}

		private void setValue(bool value) {
			this.value = value;
			Game.MouseCaptured = !value;
		}
	}
}
