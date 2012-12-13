using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GameMaterial {
    class Wolenarium :IMaterial{
        private string myName;
        private double actualQuantity;

        public Wolenarium() {
            myName = "Wolenarium";
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
