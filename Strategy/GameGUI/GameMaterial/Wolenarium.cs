using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GameMaterial {
    class Wolenarium :IMaterial{
        private string myName;

        public Wolenarium() {
            myName = "Wolenarium";
        }

        public void registerProducer(GroupControl.Game_Objects.StaticGameObjectBox.IStaticGameObject isgo) {
            throw new NotImplementedException();
        }

        public void produce(float delay) {
            throw new NotImplementedException();
        }

        public double displayChangees() {
            throw new NotImplementedException();
        }

        public int getQuantityOfMaterial() {
            throw new NotImplementedException();
        }

        public bool tryBuild(int wantedQuantity) {
            throw new NotImplementedException();
        }

        public string name {
            get { return myName; }
        }
    }
}
