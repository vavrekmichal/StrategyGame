using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	class EscortTarget : ITarget {

		Property<string> targetInfo;

		string targetName;
		string solSystName; // Doesn't check if exist, so unknown solar system means imposible target

		IGameObject gameObject;
		Mogre.Vector3 position;

		const int sqaredMaxDistance = 5000;

		const string text2 = "Target is completed. You escorted ";

		public EscortTarget(object[] args) {
			targetName = (string)args[0];
			solSystName = (string)args[1];
			position = ConvertStringToVector3((string)args[2]);

			targetInfo = new Property<string>("You must escort " + targetName + " to " + position.ToString() + " in solar system " + solSystName);
		}

		public bool Check(float delay) {

			if (gameObject.Hp<0) {
				Game.EndGame("Escorting target "+gameObject.Name +" is dead.");
			}

			if (PointIsAround(gameObject.Position, position)) {
				var solSyst = Game.GroupManager.GetSolarSystem(gameObject);
				if (solSyst != null) {
					Console.WriteLine(solSyst.Name);
					Console.WriteLine(solSystName);
				}
				if (solSyst != null && solSyst.Name == solSystName) {
					targetInfo.Value = text2 + gameObject.Name;
					return true;
				}
			}
			return false;

		}

		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

		public bool Initialize() {
			gameObject = Game.GetIGameObject(targetName);

			if (position == Mogre.Vector3.NEGATIVE_UNIT_X) {
				return false;
			}
			if (gameObject == null) {
				return false;
			}

			return true;
		}

		private Mogre.Vector3 ConvertStringToVector3(string text) {
			var parsed = text.Split(',');
			if (parsed.Count() != 2) {
				return Mogre.Vector3.NEGATIVE_UNIT_X;
			}
			var floatArray = new float[2];

			for (int i = 0; i < floatArray.Count(); i++) {
				floatArray[i] = Convert.ToSingle(parsed[i]);
			}

			return new Mogre.Vector3(floatArray[0], 0, floatArray[1]);
		}

		private static bool PointIsAround(Mogre.Vector3 targetPosition, Mogre.Vector3 centerPoint) {
			var xd = targetPosition.x - centerPoint.x;
			var yd = targetPosition.z - centerPoint.z;
			var squaredDistance = xd * xd + yd * yd;

			return squaredDistance < sqaredMaxDistance;
		}
	}
}
