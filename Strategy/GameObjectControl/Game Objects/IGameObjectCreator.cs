using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Strategy.GameObjectControl.Game_Objects.MovableGameObjectBox;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameObjectControl.Game_Objects {
	public interface IGameObjectCreator {
		IStaticGameObject CreateIsgo(string typeName, object[] args, SolarSystem solSyst);
		IMovableGameObject CreateImgo(string typeName, object[] args, SolarSystem solSyst);
		string GetUnusedName(string name);
	}

}
