using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Users.Exceptions
{
    /// <summary>
    /// Outer user service exception that wraps a <see cref="FailedUserServiceException"/>
    /// as the public surface for user service failures (The-Standard 2.0.4.0).
    /// </summary>
    public class UserServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public UserServiceException(FailedUserServiceException innerException)
            : base(message: "User service error occurred, contact support.", innerException)
        {
        }
    }
}
