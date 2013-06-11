using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	/// <summary>
	/// Extension of the Label which has panelType to close (works like CloseButton), stored object and the order of object.
	/// </summary>
	class SelectionLabel : Label {
		protected int numberOfItem;
		protected PanelType panelToClose;
		protected object storedObject;

		/// <summary>
		/// Creates instance of Label and stored an order and a panelType. Also adds MouseClick action depending on the panelType.
		/// </summary>
		/// <param name="position">The order of object.</param>
		/// <param name="panel">The type of closing panel.</param>
		public SelectionLabel(int position, PanelType panel)
			: base() {
			numberOfItem = position;
			panelToClose = panel;
			switch (panel) {
				case PanelType.LoadPanel:
					MouseClick += SelectLoadMission;
					break;
				case PanelType.SolarSystemPanel:
					MouseClick += SelectSolarSystem;
					break;
				case PanelType.TravelPanel:
					MouseClick += Travel;
					break;
				default:
					break;
			}
		}

		/// <summary>
		/// Creates instance of Label and stored an order, a object to store and a panelType. 
		/// Also adds MouseClick action depending on the panelType.
		/// </summary>
		/// <param name="position">The order of object.</param>
		/// <param name="objectRef">The object to store.</param>
		/// <param name="panel">The type of closing panel.</param>
		public SelectionLabel(int position, object objectRef, PanelType panel)
			: this(position, panel) {
			storedObject = objectRef;
		}

		/// <summary>
		/// Calls Game.Load and load given mission from stored game mission path. Also closes the panel type.
		/// </summary>
		/// <param name="sender">The sender of action.</param>
		/// <param name="e">The arguments of action.</param>
		private void SelectLoadMission(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.IGameGUI.ClosePanel(panelToClose);
			Game.Load((string)storedObject);
		}

		/// <summary>
		/// Calls GroupManager function ChangeSolarSystem and closes the panel type
		/// </summary>
		/// <param name="sender">The sender of action.</param>
		/// <param name="e">The arguments of action.</param>
		private void SelectSolarSystem(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.GroupManager.ChangeSolarSystem(numberOfItem);
			Game.IGameGUI.ClosePanel(panelToClose);

		}

		/// <summary>
		/// Calls CreateTraveler with selected number of SolarSystem (numberOfItem) and traveler(storedObject)
		/// </summary>
		/// <param name="sender">The sender of action.</param>
		/// <param name="e">The arguments of action.</param>
		private void Travel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			Game.GroupManager.CreateTraveler(numberOfItem, storedObject);
			Game.IGameGUI.ClosePanel(panelToClose);

		}
	}
}
