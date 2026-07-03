using Xeption;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    public class ApiKeyDependencyException : Xeption
    {
        public ApiKeyDependencyException(Xeption innerException)
            : base(message: "API key dependency error occurred, contact support.", innerException)
        {
        }
    }
}
