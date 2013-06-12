﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Styles;

namespace Strategy.GameGUI {
	class PopUpPanel : Panel {
		/// <summary>
		/// Extension of the Panel which has Button to close itself and Label with title.
		/// Position is calculate from given width and height (1/4 of width and 1/5 of height).
		/// Also panel's width and height is calculate as 1/2 of width and 4/7 of hight.
		/// </summary>
		/// <param name="screenWidth">The width od screen.</param>
		/// <param name="screenHeight">The height od screen.</param>
		/// <param name="text">The title text.</param>
		/// <param name="name">The name of panel.</param>
		/// <param name="rowHeight">The height of title label.</param>
		/// <param name="panelSkin">The skin of creating panel.</param>
		/// <param name="buttonSkin">The skin of the closing button.</param>
		public PopUpPanel(int screenWidth, int screenHeight, string text, string name, int rowHeight, Skin panelSkin, Skin buttonSkin) {
			Width = screenWidth / 2;
			Height = screenHeight * 4 / 7;
			Location = new Point(screenWidth / 4, screenHeight / 5);
			Skin = panelSkin;
			ResizeMode = ResizeModes.None;
			Padding = new Thickness(5, 10, 0, 0);
			Name = name;

			// Title label
			var label = new Label() {
				Size = new Size(Width / 2, rowHeight),
				Text = text,
				Location = new Point(Width / 4, 0),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.TopCenter
				}
			};

			Controls.Add(label);
			Button closeButton = new CloseButton(name) {
				Size = new Size(Width / 3, Height / 12),
				Location = new Point(Width * 5 / 8, Height * 7 / 8),
				Skin = buttonSkin,
				Text = "Cancel",
				TextStyle = new TextStyle {
					Alignment = Alignment.MiddleCenter
				}
			};

			Controls.Add(closeButton);

		}
	}
}
