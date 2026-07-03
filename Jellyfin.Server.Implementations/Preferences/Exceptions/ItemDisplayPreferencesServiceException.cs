using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Outer item display preferences service exception that wraps a
    /// <see cref="FailedItemDisplayPreferencesServiceException"/> as the public surface for item display
    /// preferences service failures (The-Standard 2.0.4.0).
    /// </summary>
    public class ItemDisplayPreferencesServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDisplayPreferencesServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public ItemDisplayPreferencesServiceException(FailedItemDisplayPreferencesServiceException innerException)
            : base(message: "Item display preferences service error occurred, contact support.", innerException)
        {
        }
    }
}
