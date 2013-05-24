using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.GroupMgr;

namespace Strategy.GameGUI {
	public interface IGameGUI {
		void Inicialization();
		void Dispose();
		void Update();
		void ShowTargeted(GroupStatics group);
		void ShowTargeted(GroupMovables group);
		void SetSolarSystemName(string name);
		void PrintToGameConsole(string text);
		void ShowSolarSystSelectionPanel(List<string> possibilities, string topic, object gameObject);
	}
}


/*
		//material update, set materials
*/