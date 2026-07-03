using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions
{
    /// <summary>
    /// Local orchestration-layer validation exception thrown when an orchestration request fails
    /// its own structural validation (e.g. a null query) before any downstream call is made.
    /// </summary>
    public class InvalidActivityLogOrchestrationException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidActivityLogOrchestrationException"/> class.
        /// </summary>
        public InvalidActivityLogOrchestrationException()
            : base(message: "Invalid activity-log orchestration error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
