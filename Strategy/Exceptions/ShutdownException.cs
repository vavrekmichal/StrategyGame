
namespace Strategy.Exceptions {
	/// <summary>
	/// Exception is catched at OnFrameRenderingQueued (BaseApplication) and ends program. 
	/// </summary>
	public class ShutdownException : System.Exception {
        public ShutdownException() {
        }

        public ShutdownException(string message)
            : base(message) {
        }

        public ShutdownException(string message, System.Exception inner)
            : base(message, inner) {
        }
	}
}
