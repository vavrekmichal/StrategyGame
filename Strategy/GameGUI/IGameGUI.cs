using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameMaterial;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.GameGUI {
	public interface IGameGUI {
		void Dispose();
		void Update();
		void ShowTargeted(GroupStatics group);
		void ShowTargeted(GroupMovables group);
		void SetSolarSystemName(string name);
		void PrintToGameConsole(string text);
		void ShowSolarSystSelectionPanel(List<string> possibilities, string topic, object gameObject);
		void UpdatePlayerMaterialDict(Dictionary<string, IMaterial> materialDict);
	}
}


/*
		//material update, set materials
*/