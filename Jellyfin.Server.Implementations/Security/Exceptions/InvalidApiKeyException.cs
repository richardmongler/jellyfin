namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    /// <summary>
    /// Validation exception thrown when an API key fails structural or logical validation.
    /// </summary>
    public class InvalidApiKeyException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidApiKeyException"/> class.
        /// </summary>
        public InvalidApiKeyException()
            : base(message: "Invalid API key error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
