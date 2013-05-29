using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	class PortalAction : IGameAction{

		private IGameObject gameObject;

		public PortalAction(IGameObject gameObject, object[] args) {
			this.gameObject = gameObject;
		}

		public void Update(float delay) {}

		public string OnMouseClick() {
			Game.IGameGUI.ShowSolarSystSelectionPanel(Game.GroupManager.GetAllSolarSystemNames(), "Choose where you'll travel", gameObject);
			return "Portal is open.";
		}

		public string IconPath() {
			return "../../media/icons/tunnel.png";
		}
	}
}
