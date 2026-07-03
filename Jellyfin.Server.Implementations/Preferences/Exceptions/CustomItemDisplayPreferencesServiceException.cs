using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Outer custom item display preferences service exception that wraps a
    /// <see cref="FailedCustomItemDisplayPreferencesServiceException"/> as the public surface for custom item
    /// display preferences service failures (The-Standard 2.0.4.0).
    /// </summary>
    public class CustomItemDisplayPreferencesServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemDisplayPreferencesServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public CustomItemDisplayPreferencesServiceException(FailedCustomItemDisplayPreferencesServiceException innerException)
            : base(message: "Custom item display preferences service error occurred, contact support.", innerException)
        {
        }
    }
}
