using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameMaterial {
    class Matrial :IMaterial{
        private string myName;
        private Property<int> actualQuantity;
		double remain;

        public Matrial(string name) {
            myName = name;
			actualQuantity = new Property<int>(0);
        }

        public string Name {
            get { return myName; }
        }

        public double DisplayChangees() {
            throw new NotImplementedException();
        }

		public Property<int> GetQuantityOfMaterial() {
			return actualQuantity;
        }

        public bool TryBuild(int wantedQuantity) {
            throw new NotImplementedException();
        }

        public int State {
            get { return (int)actualQuantity.Value; }
        }

        public void AddQuantity(double quantity) {
			remain += quantity;
			if (remain>1) {
				remain -= 1;
				actualQuantity.Value++;
			}
          
        }
    }
}
