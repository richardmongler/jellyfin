using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Exception thrown when requested item display preferences cannot be located.
    /// </summary>
    public class ItemDisplayPreferencesNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDisplayPreferencesNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that failed to match item display preferences.</param>
        public ItemDisplayPreferencesNotFoundException(string identifier)
            : base(message: $"Item display preferences with identifier '{identifier}' were not found.")
        {
        }
    }
}
