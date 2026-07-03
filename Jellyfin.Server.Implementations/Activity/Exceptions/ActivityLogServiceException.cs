using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Exceptions
{
    /// <summary>
    /// Outer activity-log service exception that wraps a <see cref="FailedActivityLogServiceException"/>
    /// as the public surface for activity-log service failures (The-Standard 2.0.4.0).
    /// </summary>
    public class ActivityLogServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public ActivityLogServiceException(FailedActivityLogServiceException innerException)
            : base(message: "ActivityLog service error occurred, contact support.", innerException)
        {
        }
    }
}
