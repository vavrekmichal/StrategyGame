using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GameMaterial {
    class Class1 :IMaterial{

        private double actualQuantity;

        public string name {
            get { return "PIcopaso"; }
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
            get {
                return (int)actualQuantity;
            }
        }

        public void addQuantity(double quantity) {
            actualQuantity+=quantity;
        }
    }
}
