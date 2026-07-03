using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions
{
    /// <summary>
    /// Categorical orchestration-layer dependency exception wrapping downstream dependency/service
    /// failures raised by the ActivityLog and User Foundation services (The-Standard 2.3.3.0.2).
    /// </summary>
    public class ActivityLogOrchestrationDependencyException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogOrchestrationDependencyException"/> class.
        /// </summary>
        /// <param name="innerException">The originating dependency/service exception.</param>
        public ActivityLogOrchestrationDependencyException(Xeption innerException)
            : base(message: "ActivityLog orchestration dependency error occurred, contact support.", innerException)
        {
        }
    }
}
