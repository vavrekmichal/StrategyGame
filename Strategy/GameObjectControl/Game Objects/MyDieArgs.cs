using System;

namespace Strategy.GameObjectControl.Game_Objects {
	/// <summary>
	/// Stores information about death.
	/// </summary>
	public class MyDieArgs : EventArgs {
		int hp;

		/// <summary>
		/// Creates an instance with the number of negative hp.
		/// </summary>
		/// <param name="hp">The negatives hp.</param>
		public MyDieArgs(int hp) {
			this.hp = hp;
		}

		/// <summary>
		/// Gets the number of negative hp (how much was the hit stronger than the lives of the damaged object). 
		/// </summary>
		public int Hp {
			get { return hp; }
		}

	}
}
