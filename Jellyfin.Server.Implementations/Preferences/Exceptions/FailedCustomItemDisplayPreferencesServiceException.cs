using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Describes a failed custom item display preferences service operation originating outside the domain rules.
    /// </summary>
    public class FailedCustomItemDisplayPreferencesServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedCustomItemDisplayPreferencesServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedCustomItemDisplayPreferencesServiceException(Exception innerException)
            : base(message: "Failed custom item display preferences service error occurred, contact support.", innerException)
        {
        }
    }
}
