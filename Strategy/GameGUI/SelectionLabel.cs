using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.UI.Controls;

namespace Strategy.GameGUI {
	class SelectionLabel : Label {
		protected int numberOfItem;
		protected PanelType panelToClose;
		protected object storedObject;


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

		public SelectionLabel(int position, object objectRef, PanelType panel)
			: this(position, panel) {
			storedObject = objectRef;
		}


		public int NumberOfItem {
			get { return numberOfItem; }
		}

		public object StoredObject {
			get { return storedObject; }
		}


		private void SelectLoadMission(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Button Action for SelectionLabel calls Game.Load and load given mission
			var selectionLabel = sender as SelectionLabel;
			if (selectionLabel != null) {
				Game.IGameGUI.ClosePanel(panelToClose);
				Game.Load((string)selectionLabel.StoredObject);
			}
		}

		private void SelectSolarSystem(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Button Action for SelectionLabel calls GroupManager function ChangeSolarSystem and close Panel
			var selectionLabel = sender as SelectionLabel;
			if (selectionLabel != null) {
				Game.GroupManager.ChangeSolarSystem(selectionLabel.NumberOfItem);
				Game.IGameGUI.ClosePanel(panelToClose);
			}
		}

		private void Travel(object sender, Miyagi.Common.Events.MouseButtonEventArgs e) {
			// Function calls CreateTraveler with selected number of SolarSystem and traveler(IMovableGameObject)
			var selectionLabel = sender as SelectionLabel;
			if (selectionLabel != null) {
				Game.GroupManager.CreateTraveler(selectionLabel.NumberOfItem, selectionLabel.StoredObject);
				Game.IGameGUI.ClosePanel(panelToClose);
			}
		}
	}
}
