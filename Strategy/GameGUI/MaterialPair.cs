using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.UI.Controls;
using Miyagi.Common.Data;

namespace Strategy.GameGUI {
	struct MaterialGUIPair {
		public Label name;
		public Label value;

		private const int rowHeight = 25;
		private static readonly Thickness padding = new Thickness(5, 1, 1, 1);


		/// <summary>
		/// This constructor creates pair of Myiagi Label. One with name of material and other with value.
		/// </summary>
		/// <param name="nameString">Name of material</param>
		/// <param name="valueInt">Number of units of a material</param>
		/// <param name="maxWidth">Maximum width of a row</param>
		/// <param name="position">Order</param>
		public MaterialGUIPair(string nameString, int valueInt, int maxWidth, int position) {
			
			// Label with name of material
			int newWidth = maxWidth / 5;
			name = new Label() {
				Size = new Size(newWidth * 3, rowHeight),
				Text = nameString,
				Location = new Point(0, position * (rowHeight + 1)),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleLeft,
					ForegroundColour = Colours.White
				},
				Padding = padding
			};

			// Label with number of units of a material
			value = new Label() {
				Size = new Size(newWidth, rowHeight),
				Text = valueInt.ToString(),
				Location = new Point(newWidth * 3, position * (rowHeight + 1)),
				TextStyle = {
					Alignment = Miyagi.Common.Alignment.MiddleRight,
					ForegroundColour = Colours.White
				},
				Padding = padding
			};
		}

	}
}
