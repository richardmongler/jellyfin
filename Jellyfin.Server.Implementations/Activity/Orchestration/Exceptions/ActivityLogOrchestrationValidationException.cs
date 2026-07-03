using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions
{
    /// <summary>
    /// Categorical orchestration-layer validation exception aggregating downstream validation
    /// failures raised by the ActivityLog and User Foundation services (The-Standard 2.3.3.0.2).
    /// </summary>
    public class ActivityLogOrchestrationValidationException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogOrchestrationValidationException"/> class.
        /// </summary>
        /// <param name="innerException">The originating localized validation exception.</param>
        public ActivityLogOrchestrationValidationException(Xeption innerException)
            : base(message: "ActivityLog orchestration validation error occurred, fix errors and try again.", innerException)
        {
        }
    }
}
