using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Users.Exceptions
{
    /// <summary>
    /// Validation exception thrown when a user fails structural or logical validation.
    /// </summary>
    public class InvalidUserException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUserException"/> class.
        /// </summary>
        public InvalidUserException()
            : base(message: "Invalid user error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
