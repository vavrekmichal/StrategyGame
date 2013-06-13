﻿using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	/// <summary>
	/// Controls given target's team.
	/// </summary>
	class OccupyTarget : ITarget {

		string targetName;
		Property<string> targetInfo;
		IGameObject gameObject;

		const string text1 = "You must occupy ";
		const string text2 = "Target completed. You're occupied ";

		/// <summary>
		/// Stored target's name to initialization. Also initializes info Property.
		/// </summary>
		/// <param name="args">The arguments should have just one member (target name).</param>
		public OccupyTarget(object[] args) {
			targetName = (string)args[0];
			targetInfo = new Property<string>(text1 + targetName);
		}

		/// <summary>
		/// Checks if the target's team is Player's team.
		/// </summary>
		/// <param name="delay">The delay between last two frames (seconds).</param>
		/// <returns>Returns if the target is in Player's team.</returns>
		public bool Check(float delay) {
			if (gameObject.Team.Name == Game.PlayerName) {
				targetInfo.Value = text2 + targetName;
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// Return Property with a mission's target info.
		/// </summary>
		/// <returns>Return reference to Property with a mission's target info.</returns>
		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

		/// <summary>
		/// Stores reference on the target object (gets by the name).
		/// </summary>
		/// <returns>Return if initialization was successful.</returns>
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
