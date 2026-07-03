using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions
{
    /// <summary>
    /// Describes a failed activity-log orchestration operation originating outside the domain rules.
    /// </summary>
    public class FailedActivityLogOrchestrationServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedActivityLogOrchestrationServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedActivityLogOrchestrationServiceException(Exception innerException)
            : base(message: "Failed activity-log orchestration service error occurred, contact support.", innerException)
        {
        }
    }
}
