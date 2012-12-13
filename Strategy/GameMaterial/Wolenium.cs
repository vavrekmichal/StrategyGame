using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GroupControl.Game_Objects.StaticGameObjectBox;

namespace Strategy.GameMaterial {
    class Wolenium :IMaterial{

        private string myName;
        private double actualQuantity;


        public Wolenium() {
            myName = "Wolenium";
        }

        public string name {
            get { return myName; }
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

        public int state {
            get { return (int)actualQuantity; }
        }

        public void addQuantity(double quantity) {
            actualQuantity += quantity;
        }
    }
}
