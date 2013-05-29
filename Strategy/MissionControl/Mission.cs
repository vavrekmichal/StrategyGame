using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.GameTargets;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.MissionControl {
	public class Mission {

		List<ITarget> targetList;

		const string winText = "You are winner.";

		public Mission() {
			targetList = new List<ITarget>();
		}

		public void Update(float delay) {
			if (targetList.Count() == 0) {
				Game.EndGame(winText);
			}
			foreach (var target in new List<ITarget>(targetList)) {
				if (target.Check(delay)) {
					targetList.Remove(target);
				}
			}
		}

		public void AddTarget(ITarget target) {
			targetList.Add(target);
		}

		public List<Property<string>> GetMissionInfo() {
			var list = new List<Property<string>>();
			foreach (var target in targetList) {
				list.Add(target.GetTargetInfo());
			}
			return list;
		}
	}
}
