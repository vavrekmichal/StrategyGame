using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strategy.Exceptions {
    class MissingMaterialException : System.Exception {
        public MissingMaterialException() {
        }

        public MissingMaterialException(string message)
            : base(message) {
        }

        public MissingMaterialException(string message, System.Exception inner)
            : base(message, inner) {
        }
    }
}
