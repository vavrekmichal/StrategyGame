using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	/// <summary>
	/// Extension of the Label which has panelType to close (works like CloseButton), stored object and the order of the object.
	/// </summary>
	class SelectionLabel : Label {
		protected int numberOfItem;
		protected string panelToClose;
		protected object storedObject;

		/// <summary>
		/// Creates instance of the Label and stored an order and a panelType. Also adds MouseClick action depending on the panelType.
		/// </summary>
		/// <param name="position">The order of the object.</param>
		/// <param name="panel">The type of the closing panel.</param>
		public SelectionLabel(int position, string panelName)
			: base() {
			numberOfItem = position;
			panelToClose = panelName;
			switch (panelName) {
				case "LoadPanel":
					MouseClick += SelectLoadMission;
					break;
				case "SolarSystemPanel":
					MouseClick += SelectSolarSystem;
					break;
				default:
					MouseClick += Travel;
					break;
			}
		}

		/// <summary>
		/// Creates instance of the Label and stored an order, a object to store and a panelType and closing panel reference. 
		/// Also adds MouseClick action depending on the panelType.
		/// </summary>
		/// <param name="position">The order of the object.</param>
		/// <param name="objectRef">The object to store.</param>
		/// <param name="panel">The type of the closing panel.</param>
		/// <param name="closingPanel">The reference on the closing Panel</param>
		public SelectionLabel(int position, object objectRef, string panelName)
			: this(position, panelName) {
			storedObject = objectRef;
		}

		/// <summary>
		/// Calls Game.Load and load given mission from stored game mission path. Also closes the panel type.
		/// </summary>
		/// <param name="sender">The sender of the action.</param>
		/// <param name="e">The arguments of the action.</param>
		private void SelectLoadMission(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.IGameGUI.ClosePanel(panelToClose);
			Game.Load((string)storedObject);
		}

		/// <summary>
		/// Calls GroupManager function ChangeSolarSystem and closes the panel type
		/// </summary>
		/// <param name="sender">The sender of the action.</param>
		/// <param name="e">The arguments of the action.</param>
		private void SelectSolarSystem(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.SolarSystemManager.ChangeSolarSystem(numberOfItem);
			Game.IGameGUI.ClosePanel(panelToClose);
		}

		/// <summary>
		/// Calls CreateTraveler with selected number of the SolarSystem (numberOfItem) and traveler(storedObject)
		/// </summary>
		/// <param name="sender">The sender of the action.</param>
		/// <param name="e">The arguments of the action.</param>
		private void Travel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.SolarSystemManager.CreateTraveler(numberOfItem, storedObject);
			Game.IGameGUI.ClosePanel(panelToClose);
		}
	}
}
