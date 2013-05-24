using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
	class DoNothingJustPrintText : IGameAction {
	

		public void Update(float delay) {
			
		}

		public string OnMouseClick() {
			Game.PrintToGameConsole("Neklikej na me blbecku.");
			return "Myslim to vazne.";
		}



		public string IconPath() {
			return "../../media/icons/Pokus.jpg";
		}
	}
}
