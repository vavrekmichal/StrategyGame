using System;
using System.Linq;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	/// <summary>
	/// Constrols given target, its possition and its solar system.
	/// </summary>
	class EscortTarget : ITarget {

		Property<string> targetInfo;

		string targetName;
		string solSystName; // Doesn't check if exist, so unknown solar system means imposible target

		IGameObject gameObject;
		Mogre.Vector3 position;

		const int squaredMaxDistance = 5000;

		const string text2 = "Target is completed. You escorted ";

		/// <summary>
		/// Stores data to initialization. Stores target name, target solar system and target position.
		/// Also initializes info Property.
		/// </summary>
		/// <param name="args">The arguments should contains 3 members (target: name, solar system, position)</param>
		public EscortTarget(object[] args) {
			targetName = (string)args[0];
			solSystName = (string)args[1];
			position = ConvertStringToVector3((string)args[2]);

			targetInfo = new Property<string>("You must escort " + targetName + " to " + position.ToString() + " in solar system " + solSystName);
		}

		/// <summary>
		/// Checks if object is alive (if not si mission failed) after that if point is around the target position (radius is squaredMaxDistance)
		/// and finally checks if object is in the target solar system.
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		public bool Check(float delay) {

			if (gameObject.Hp<0) {
				Game.EndMission("Escorting target "+gameObject.Name +" is dead.");
			}

			if (PointIsAround(gameObject.Position, position)) {
				// Object is near to the target positon.
				var solSyst = Game.SolarSystemManager.GetSolarSystem(gameObject);
				if (solSyst != null) {
					Console.WriteLine(solSyst.Name);
					Console.WriteLine(solSystName);
				}
				if (solSyst != null && solSyst.Name == solSystName) {
					// Object is at the target solar system.
					targetInfo.Value = text2 + gameObject.Name;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Return Property with a mission target info.
		/// </summary>
		/// <returns>Return reference to Property with a mission target info.</returns>
		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

		/// <summary>
		/// Initializes mission target: gets target object and controls if position is correctly parsed.
		/// </summary>
		/// <returns>Returns if initialization was successful.</returns>
		public bool Initialize() {
			gameObject = Game.GetIGameObject(targetName);

			if (position == Mogre.Vector3.NEGATIVE_UNIT_Y) {
				return false;
			}
			if (gameObject == null) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// Converts string with Vector2 to Mogre.Vector3.
		/// String must has two members separate by ';'.
		/// </summary>
		/// <param name="text">The converting string.</param>
		/// <returns>Returns the converted string. If the  failed so returns NEGATIVE_UNIT_Y.</returns>
		private Mogre.Vector3 ConvertStringToVector3(string text) {
			var parsed = text.Split(',');
			if (parsed.Count() != 2) {
				return Mogre.Vector3.NEGATIVE_UNIT_Y;
			}
			var floatArray = new float[2];

			for (int i = 0; i < floatArray.Count(); i++) {
				floatArray[i] = Convert.ToSingle(parsed[i]);
			}
			return new Mogre.Vector3(floatArray[0], 0, floatArray[1]);
		}

		/// <summary>
		/// Checks if point is around given position. Maximum distance is squaredMaxDistance constant.
		/// </summary>
		/// <param name="targetPosition">The position of the target game object.</param>
		/// <param name="centerPoint">The target position.</param>
		/// <returns>Returns if object is in acceptable distance.</returns>
		private static bool PointIsAround(Mogre.Vector3 targetPosition, Mogre.Vector3 centerPoint) {
			var xd = targetPosition.x - centerPoint.x;
			var yd = targetPosition.z - centerPoint.z;
			var squaredDistance = xd * xd + yd * yd;

			return squaredDistance < squaredMaxDistance;
		}
	}
}
