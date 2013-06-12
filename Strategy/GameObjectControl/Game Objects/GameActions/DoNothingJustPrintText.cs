﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	class DoNothingJustPrintText : IGameAction {

		private IGameObject gameObject;

		/// <summary>
		/// Just saves rafarence to owner.
		/// </summary>
		/// <param name="gameObject">The IGameAction's owner.</param>
		/// <param name="args">The arguments should be empty.</param>
		public DoNothingJustPrintText(IGameObject gameObject, object[] args) {
			this.gameObject = gameObject;
		}

		/// <summary>
		/// Do nothing on Update.
		/// </summary>
		/// <param name="delay">The delay between last to frames.</param>
		public void Update(float delay) {
		}

		/// <summary>
		/// Just returns text to print.
		/// </summary>
		/// <returns>Returns text to print.</returns>
		public string OnMouseClick() {
			return "This is just prototype of function.";
		}


		/// <summary>
		/// Returns path to a icon picture.
		/// </summary>
		/// <returns>Returns path to a icon picture.</returns>
		public string IconPath() {
			return "../../media/icons/write.png";
		}
	}
}
