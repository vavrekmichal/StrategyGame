
namespace Strategy.Exceptions {
	/// <summary>
	/// Exception is thorwn when any object requires Property<> whitch is not in a compiling script .
	/// Class inherits from ShutdownException to allow shutdown the program.
	/// </summary>
	class PropertyMissingException : ShutdownException {
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
