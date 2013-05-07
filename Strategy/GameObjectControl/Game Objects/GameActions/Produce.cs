using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.TeamControl;
using Strategy.GameMaterial;

namespace Strategy.GameObjectControl.Game_Objects.GameActions {
    class Produce : IGameAction{

        //fce ma kdo ji muye volat a isgo bude mit list IGameAction kterz bude zkouset volat

        //private Dictionary<Object, List<KeyValuePair<string,double>>> productionDictionary;
        private Dictionary<Object, List<KeyValuePair<IMaterial,double>>> productionDictionary;

        public Produce() {
            productionDictionary = new Dictionary<Object, List<KeyValuePair<IMaterial, double>>>();
        }


        public void Execute(object executer, Team team) {
            foreach (var k in productionDictionary[executer]) {
                team.AddMaterial(k.Key.Name,k.Value);
            }
        }

        public void RegisterExecuter(object executer, IMaterial specificMaterial, double value) {
            if (!productionDictionary.ContainsKey(executer)) {
                productionDictionary.Add(executer, new List<KeyValuePair<IMaterial, double>>());
            }

            productionDictionary[executer].Add(new KeyValuePair<IMaterial, double>(specificMaterial, value));
        }

        public string Name{
			get { return "Produce"; }
        }
    }
}
