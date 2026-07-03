using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Exceptions
{
    /// <summary>
    /// Validation exception thrown when display preferences fail structural or logical validation.
    /// </summary>
    public class InvalidDisplayPreferencesException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidDisplayPreferencesException"/> class.
        /// </summary>
        public InvalidDisplayPreferencesException()
            : base(message: "Invalid display preferences error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
