namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    /// <summary>
    /// Exception returned from the API key Foundation Service layer.
    /// </summary>
    public class ApiKeyServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating service exception.</param>
        public ApiKeyServiceException(Xeption innerException)
            : base(message: "API key service error occurred, contact support.", innerException)
        {
        }
    }
}
