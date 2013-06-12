using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameMaterial;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.GameGUI {
	public interface IGameGUI {
		/// <summary>
		/// Disposes GUI system.
		/// </summary>
		void Dispose();

		/// <summary>
		/// Updates all GUI system.
		/// </summary>
		void Update();

		/// <summary>
		/// Shows actual targeted (given) GroupStatics group (properties, count,...).
		/// </summary>
		/// <param name="group">The actual targeted group.</param>
		void ShowTargeted(GroupStatics group);

		/// <summary>
		/// Shows actual targeted (given) GroupMovables group (properties, count,...).
		/// </summary>
		/// <param name="group">The actual targeted group.</param>/>
		void ShowTargeted(GroupMovables group);

		/// <summary>
		/// Sets SolarSystem's name in the relevant Label. 
		/// </summary>
		/// <param name="name">The name of SolarSystem</param>
		void SetSolarSystemName(string name);

		/// <summary>
		/// Prints given text to game console.
		/// </summary>
		/// <param name="text">The text to print.</param>
		void PrintToGameConsole(string text);

		/// <summary>
		/// Shows panel with travel destinations (SolarSystem's names). Users answer will be sends
		/// to Game.GroupManager.CreateTraveler().
		/// </summary>
		/// <param name="possibilities">The list with names of SolarSystems</param>
		/// <param name="gameObject">The potential traveler</param>
		void ShowTravelSelectionPanel(List<string> possibilities, object gameObject);

		/// <summary>
		/// Creates information about materials in the dictionary. Prints names and values.
		/// </summary>
		/// <param name="materialDict">The dictionary with name of material and appropriate instance of material.</param>
		void UpdatePlayerMaterialDict(Dictionary<string, IMaterial> materialDict);

		/// <summary>
		/// Prints information about end of the mission.
		/// </summary>
		/// <param name="printText">The information about end of the mission.</param>
		void MissionEnd(string printText);

		/// <summary>
		/// Sets GUI enable or disable. Enable GUI display mission's informations. Disable GUI alows just Load or Exit.
		/// </summary>
		bool Enable { get; set; }

		/// <summary>
		/// Clears all mission information (material states, properties,...).
		/// </summary>
		void ClearMissionData();

		/// <summary>
		/// Closes panel by the name.
		/// </summary>
		/// <param name="panelName">The name of the closing panel.</param>
		void ClosePanel(string panelName);

		/// <summary>
		/// Returns number of actual opened panels.
		/// </summary>
		int NumberOfPopUpPanels { get; }
	}
}
