using Xeption;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    public class ApiKeyNotFoundException : Xeption
    {
        public ApiKeyNotFoundException(string accessToken)
            : base(message: $"API key not found with access token: {accessToken}.")
        {
        }
    }
}
