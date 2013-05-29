using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameObjectControl.Game_Objects.GameTargets {
	class SurviveGivenTimeTarget : ITarget{

		TimeSpan time;
		Property<string> targetInfo;

		const string text = "You must survive ";

		public SurviveGivenTimeTarget(object[] args) {
			time = TimeSpan.FromSeconds(Convert.ToInt32(args[0]));
			targetInfo = new Property<string>(text+ time.ToString());
		
		}

		public bool Check(float delay) {
			time -= TimeSpan.FromSeconds(delay);
			targetInfo.Value = text + time;
			return time < TimeSpan.Zero;

		}

		public Property<string> GetTargetInfo() {
			return targetInfo;
		}
	}
}
