using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GameObjectControl.RuntimeProperty {
	public class Property<T> : ICloneable{
		private T value;

		public Property(T value) {
			this.value = value;
		}

		public T Value {
			set { this.value = value; }
			get { return this.value; }
		}

		public object Clone() {
			return this.MemberwiseClone();
		}
	}
}
