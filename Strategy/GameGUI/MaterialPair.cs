using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miyagi.UI.Controls;
using Miyagi.Common.Data;

namespace Strategy.GameGUI {
	public struct MaterialGUIPair {
		public Label name;
		public Label value;

		private const int rowHeight = 25;
		private static readonly Thickness padding = new Thickness(5, 1, 1, 1);


		/// <summary>
		/// This constructor creates pair of Myiagi Label. One with Name of material and other with value.
		/// </summary>
		/// <param Name="nameString">Name of material</param>
		/// <param Name="valueInt">Number of units of a material</param>
		/// <param Name="maxWidth">Maximum width of a row</param>
		/// <param Name="position">Order</param>
		public MaterialGUIPair(string nameString, int valueInt, int maxWidth, int position) {
			
			// Label with Name of material
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
