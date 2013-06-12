using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	/// <summary>
	/// Represents mission's targets. All ITargets should not initialize in the constructor but 
	/// in Initialize() function.
	/// </summary>
	public interface ITarget {
		/// <summary>
		/// Initializes mission's target.
		/// </summary>
		/// <returns>Return if initialization was successful.</returns>
		bool Initialize();

		/// <summary>
		/// Checks if the target is complete.
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		/// <returns>Return if the target is complete.</returns>
		bool Check(float delay);

		/// <summary>
		/// Returns actual state of the target (information to complete it).
		/// </summary>
		/// <returns>Return reference to the target's state like Property (changes are seen for anyone who 
		/// has reference)</returns>
		Property<string> GetTargetInfo();
	}
}
