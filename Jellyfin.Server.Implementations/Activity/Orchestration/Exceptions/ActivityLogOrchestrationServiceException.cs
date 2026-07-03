using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions
{
    /// <summary>
    /// Outer activity-log orchestration exception that wraps a
    /// <see cref="FailedActivityLogOrchestrationServiceException"/> as the public surface for
    /// orchestration failures (The-Standard 2.0.4.0).
    /// </summary>
    public class ActivityLogOrchestrationServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogOrchestrationServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public ActivityLogOrchestrationServiceException(FailedActivityLogOrchestrationServiceException innerException)
            : base(message: "ActivityLog orchestration service error occurred, contact support.", innerException)
        {
        }
    }
}
