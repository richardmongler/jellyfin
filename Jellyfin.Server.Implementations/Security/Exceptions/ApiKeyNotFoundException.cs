using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested API key cannot be located.
    /// </summary>
    public class ApiKeyNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier (id or access token) that was not found.</param>
        public ApiKeyNotFoundException(string identifier)
            : base(message: $"API key not found for identifier: {identifier}.")
        {
        }
    }
}
