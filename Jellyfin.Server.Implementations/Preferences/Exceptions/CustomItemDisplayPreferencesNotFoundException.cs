using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Exception thrown when requested custom item display preferences cannot be located.
    /// </summary>
    public class CustomItemDisplayPreferencesNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemDisplayPreferencesNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that failed to match custom item display preferences.</param>
        public CustomItemDisplayPreferencesNotFoundException(string identifier)
            : base(message: $"Custom item display preferences with identifier '{identifier}' were not found.")
        {
        }
    }
}
