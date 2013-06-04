using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Strategy.GameObjectControl.Game_Objects.GameLoad {
	interface IGameLoader {

		XmlNodeList GetUsedObjects();
		XmlNodeList GetISGObjects();
		XmlNodeList GetIMGObjects();
		XmlNodeList GetMissionTargets();
		XmlNodeList GetTeams();
	}
}
