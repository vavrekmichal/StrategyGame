using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.GroupControl.RuntimeProperty {
	class Property<T> {
		private T value;

		public Property(T value) {
			this.value = value;
		}

		public void setValue(T value) {
			this.value = value;
		}
	}
}
