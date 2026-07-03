using Xeption;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    public class ApiKeyServiceException : Xeption
    {
        public ApiKeyServiceException(Xeption innerException)
            : base(message: "API key service error occurred, contact support.", innerException)
        {
        }
    }
}
