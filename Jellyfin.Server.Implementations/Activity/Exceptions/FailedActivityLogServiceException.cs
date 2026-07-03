using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Exceptions
{
    /// <summary>
    /// Describes a failed activity-log service operation originating outside the domain rules.
    /// </summary>
    public class FailedActivityLogServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedActivityLogServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedActivityLogServiceException(Exception innerException)
            : base(message: "Failed activity-log service error occurred, contact support.", innerException)
        {
        }
    }
}
