using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.GameMaterial {
    class Class1 :IMaterial{

        private double actualQuantity;

        public string Name {
            get { return "PIkopaso"; }
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
            get {
                return (int)actualQuantity;
            }
        }

        public void AddQuantity(double quantity) {
            actualQuantity+=quantity;
        }
    }
}
