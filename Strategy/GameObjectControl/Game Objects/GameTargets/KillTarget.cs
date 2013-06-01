using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	class KillTarget : ITarget {

		string targetName;
		Property<string> targetInfo;
		IGameObject gameObject;

		const string text1 = "You must kill ";
		const string text2 = "Target completed. You killed ";

		public KillTarget(object[] args) {
			targetName = (string)args[0];
			targetInfo = new Property<string>(text1 + targetName);
		}

		public bool Check(float delay) {
			if (gameObject.Hp < 0) {
				targetInfo.Value = text2 + targetName;
				return true;
			} else {
				return false;
			}
		}

		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

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
