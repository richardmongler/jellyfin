using Xeption;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    public class InvalidApiKeyException : Xeption
    {
        public InvalidApiKeyException()
            : base(message: "Invalid API key error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
