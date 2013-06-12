using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Strategy.GameObjectControl.RuntimeProperty;

namespace Strategy.GameMaterial {
	/// <summary>
	/// Represents an universal game material (name is setted by constructor).
	/// </summary>
    class Matrial :IMaterial{
        private string myName;
        private Property<int> currentQuantity;
		double remain;

		/// <summary>
		/// Creates instance and sets name of the material.
		/// </summary>
		/// <param name="name"></param>
        public Matrial(string name) {
            myName = name;
			currentQuantity = new Property<int>(0);
        }

		/// <summary>
		/// Returns material name.
		/// </summary>
        public string Name {
            get { return myName; }
        }

		/// <summary>
		/// Returns reference to currentQuantity Property. 
		/// </summary>
		/// <returns>Returns reference to Property with a current quantity.</returns>
		public Property<int> GetQuantityOfMaterial() {
			return currentQuantity;
        }

		/// <summary>
		/// Returns current quantity of the material. 
		/// </summary>
        public int State {
            get { return (int)currentQuantity.Value; }
        }

		/// <summary>
		/// Adds given quantity of the material. To currentQuantity adds just integers. Fractional part stores and adds to it a next time. 
		/// </summary>
		/// <param name="quantity">The adding quantities of material.</param>
        public void AddQuantity(double quantity) {
			remain += quantity;
			if (remain>1) {
				int increment = (int)Math.Truncate(remain);
				remain -= increment;
				currentQuantity.Value += increment;
			}
          
        }
    }
}
