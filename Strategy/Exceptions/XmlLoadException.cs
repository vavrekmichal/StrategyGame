
namespace Strategy.Exceptions {
	/// <summary>
	/// Exception is thrown when Xml mission file has some invalid nodes or is not valid.
	/// Class inherits from ShutdownException to allow shutdown the program.
	/// </summary>
	public class XmlLoadException : ShutdownException {
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
