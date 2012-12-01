using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameMaterial {
    class Wolenium :IMaterial{

        private string myName; 

        public Wolenium() {
            myName = "Wolenium";
        }

        public void registerProducer(IStaticGameObject isgo) {
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
