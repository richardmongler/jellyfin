using System;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    /// <summary>
    /// Exception describing a failed API key service operation.
    /// </summary>
    public class FailedApiKeyServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedApiKeyServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedApiKeyServiceException(Exception innerException)
            : base(message: "Failed API key service error occurred, contact support.", innerException)
        {
        }
    }
}
