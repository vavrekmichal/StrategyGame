using System.Collections.Generic;
using System.Linq;
using Strategy.GameObjectControl.Game_Objects.GameTargets;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.MissionControl {
	/// <summary>
	/// Represents a game mission. Can be loaded just one (before loading next mission the firts
	/// must be destroyd).
	/// </summary>
	public class Mission {

		List<ITarget> targetList;
		TeamControl.Team playerTeam;

		const string winText = "You are winner.";
		const string loseText = "You are winner.";
		const string targetCompEffect = "TargetComp.wav";

		/// <summary>
		/// Initializes Mission (no mission targets).
		/// </summary>
		public Mission() {
			targetList = new List<ITarget>();
		}

		/// <summary>
		/// Initializes mission targets. If the initialization of the target
		/// fails so the target is removed as complete.
		/// </summary>
		public void Initialize() {
			foreach (var target in new List<ITarget>(targetList)) {
				if (!target.Initialize()) {
					targetList.Remove(target);
				}
			}
			playerTeam = Game.TeamManager.GetTeam(Game.PlayerName);
		}

		/// <summary>
		/// Updates all mission targets and checks if any exists (if not, so the
		/// mission end.)
		/// </summary>
		/// <param name="delay">The delay between last two frames.</param>
		public void Update(float delay) {
			if (playerTeam.Count < 1) {
				Game.EndMission(loseText);
			}
			if (targetList.Count() == 0) {
				// No target -> mission ends.
				Game.EndMission(winText);
			}
			foreach (var target in new List<ITarget>(targetList)) {
				if (target.Check(delay)) {
					Game.PrintToGameConsole(target.GetTargetInfo().Value);
					Game.IEffectPlayer.PlayEffect(targetCompEffect);
					targetList.Remove(target);
				}
			}
		}

		/// <summary>
		/// Inserts non-initializes mission target.
		/// </summary>
		/// <param name="target">The inserting mission target.</param>
		public void AddTarget(ITarget target) {
			targetList.Add(target);
		}

		/// <summary>
		/// Collects informaion about all mission targets to the List.
		/// </summary>
		/// <returns>Returns the List with infomations about mission targets.</returns>
		public List<Property<string>> GetMissionInfo() {
			var list = new List<Property<string>>();
			foreach (var target in targetList) {
				list.Add(target.GetTargetInfo());
			}
			return list;
		}

		/// <summary>
		/// Returns all mission targets.
		/// </summary>
		/// <returns>Returns all mission targets.</returns>
		public List<ITarget> GetTargets() {
			return new List<ITarget>(targetList);
		}
	}
}
