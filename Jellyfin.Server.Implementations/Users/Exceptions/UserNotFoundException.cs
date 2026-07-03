using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Users.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested user cannot be located.
    /// </summary>
    public class UserNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that failed to match a user.</param>
        public UserNotFoundException(string identifier)
            : base(message: $"User with identifier '{identifier}' was not found.")
        {
        }
    }
}
