using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameMaterial {
    class Wolenium :IMaterial{

        private string myName;
        private double actualQuantity;


        public Wolenium() {
            myName = "Wolenium";
        }

        public string Name {
            get { return myName; }
        }

        public double DisplayChangees() {
            throw new NotImplementedException();
        }

        public int GetQuantityOfMaterial() {
            throw new NotImplementedException();
        }

        public bool TryBuild(int wantedQuantity) {
            throw new NotImplementedException();
        }

        public int State {
            get { return (int)actualQuantity; }
        }

        public void AddQuantity(double quantity) {
            actualQuantity += quantity;
        }
    }
}
