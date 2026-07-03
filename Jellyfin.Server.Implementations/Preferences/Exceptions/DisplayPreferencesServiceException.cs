using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Outer display preferences service exception that wraps a
    /// <see cref="FailedDisplayPreferencesServiceException"/> as the public surface for display
    /// preferences service failures (The-Standard 2.0.4.0).
    /// </summary>
    public class DisplayPreferencesServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayPreferencesServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public DisplayPreferencesServiceException(FailedDisplayPreferencesServiceException innerException)
            : base(message: "Display preferences service error occurred, contact support.", innerException)
        {
        }
    }
}
