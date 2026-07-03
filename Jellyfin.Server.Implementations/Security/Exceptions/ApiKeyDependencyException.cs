using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    /// <summary>
    /// Exception returned from the API key resource dependency (broker-neighboring layer).
    /// </summary>
    public class ApiKeyDependencyException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyDependencyException"/> class.
        /// </summary>
        /// <param name="innerException">The originating dependency exception.</param>
        public ApiKeyDependencyException(Xeption innerException)
            : base(message: "API key dependency error occurred, contact support.", innerException)
        {
        }
    }
}
