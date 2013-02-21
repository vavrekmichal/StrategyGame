using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.Exceptions {
	class XmlLoadException : System.Exception {
		public XmlLoadException() {
        }

        public XmlLoadException(string message)
            : base(message) {
        }

		public XmlLoadException(string message, System.Exception inner)
            : base(message, inner) {
        }
	}
}
