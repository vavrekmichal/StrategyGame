using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.Exceptions {
	class PropertyMissingException : System.Exception {
		public PropertyMissingException() {
		}

		public PropertyMissingException(string message)
			: base(message) {
		}

		public PropertyMissingException(string message, System.Exception inner)
			: base(message, inner) {
		}
	}
}
