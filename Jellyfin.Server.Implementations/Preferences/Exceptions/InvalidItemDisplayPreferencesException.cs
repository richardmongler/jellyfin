using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Validation exception thrown when item display preferences fail structural or logical validation.
    /// </summary>
    public class InvalidItemDisplayPreferencesException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidItemDisplayPreferencesException"/> class.
        /// </summary>
        public InvalidItemDisplayPreferencesException()
            : base(message: "Invalid item display preferences error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
