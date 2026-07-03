using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Describes a failed display preferences service operation originating outside the domain rules.
    /// </summary>
    public class FailedDisplayPreferencesServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedDisplayPreferencesServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedDisplayPreferencesServiceException(Exception innerException)
            : base(message: "Failed display preferences service error occurred, contact support.", innerException)
        {
        }
    }
}
