using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Describes a failed item display preferences service operation originating outside the domain rules.
    /// </summary>
    public class FailedItemDisplayPreferencesServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedItemDisplayPreferencesServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedItemDisplayPreferencesServiceException(Exception innerException)
            : base(message: "Failed item display preferences service error occurred, contact support.", innerException)
        {
        }
    }
}
