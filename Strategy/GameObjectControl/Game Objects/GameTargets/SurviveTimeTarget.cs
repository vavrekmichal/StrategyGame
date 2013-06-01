using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	class SurviveTimeTarget : ITarget{

		TimeSpan time;
		Property<string> targetInfo;

		const string text1 = "You must survive ";
		const string text2 = "Target is completed. You survived given time.";

		public SurviveTimeTarget(object[] args) {
			time = TimeSpan.FromSeconds(Convert.ToInt32(args[0]));
			targetInfo = new Property<string>(text1+ time.ToString());	
		}

		public bool Check(float delay) {
			time -= TimeSpan.FromSeconds(delay);
			if (time < TimeSpan.Zero) {
				targetInfo.Value = text2;
				return true;
			} else {
				targetInfo.Value = text1 + time;
				return false;
			}

		}

		public Property<string> GetTargetInfo() {
			return targetInfo;
		}

		public bool Initialize() {
			return true;
		}
	}
}
