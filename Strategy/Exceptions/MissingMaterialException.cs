
namespace Strategy.Exceptions {
	/// <summary>
	/// Exception is thorwn when a team doesn't have the necessary amount of the material.
	/// Class inherits from ShutdownException to allow shutdown the program.
	/// </summary>
	class MissingMaterialException : ShutdownException {
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
