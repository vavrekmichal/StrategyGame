using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	/// <summary>
	/// Controls if given target is alive (should be death).
	/// </summary>
	class KillTarget : ITarget {

		string targetName;
		Property<string> targetInfo;
		IGameObject gameObject;

		const string text1 = "You must kill ";
		const string text2 = "Target completed. You killed ";

		/// <summary>
		/// Stores target name to initialization and initialize info Property.
		/// </summary>
		/// <param name="args">The arguments should have just one member (target name).</param>
		public KillTarget(object[] args) {
			targetName = (string)args[0];
			targetInfo = new Property<string>(text1 + targetName);
		}

		/// <summary>
		/// Checks of the target is still alive.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		/// <returns>Returns if the target is death.</returns>
		public bool Check(float delay) {
			if (gameObject.Hp < 0) {
				targetInfo.Value = text2 + targetName;
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Return Property with a mission target info.
		/// </summary>
		/// <returns>Return reference to Property with a mission target info.</returns>
		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

		/// <summary>
		/// Stores reference to the target object (gets by the name).
		/// </summary>
		/// <returns>Return if initialization was successful (target exists).</returns>
		public bool Initialize() {
			gameObject = Game.GetIGameObject(targetName);
			if (gameObject == null) {
				return false;
			} else {
				return true;
			}
		}
	}
}
