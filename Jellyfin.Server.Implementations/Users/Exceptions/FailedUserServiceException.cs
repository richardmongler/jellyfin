using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Users.Exceptions
{
    /// <summary>
    /// Describes a failed user service operation originating outside the user domain rules.
    /// </summary>
    public class FailedUserServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedUserServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedUserServiceException(Exception innerException)
            : base(message: "Failed user service error occurred, contact support.", innerException)
        {
        }
    }
}
