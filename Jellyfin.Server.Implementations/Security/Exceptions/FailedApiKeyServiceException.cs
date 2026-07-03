using System;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    public class FailedApiKeyServiceException : Xeption
    {
        public FailedApiKeyServiceException(Exception innerException)
            : base(message: "Failed API key service error occurred, contact support.", innerException)
        {
        }
    }
}
