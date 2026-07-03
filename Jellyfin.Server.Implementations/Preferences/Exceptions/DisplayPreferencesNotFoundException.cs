using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Exception thrown when requested display preferences cannot be located.
    /// </summary>
    public class DisplayPreferencesNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayPreferencesNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that failed to match display preferences.</param>
        public DisplayPreferencesNotFoundException(string identifier)
            : base(message: $"Display preferences with identifier '{identifier}' were not found.")
        {
        }
    }
}
