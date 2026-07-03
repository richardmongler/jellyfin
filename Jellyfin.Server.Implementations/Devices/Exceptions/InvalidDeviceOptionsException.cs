using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Validation exception thrown when device options fail structural or logical validation.
    /// </summary>
    public class InvalidDeviceOptionsException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidDeviceOptionsException"/> class.
        /// </summary>
        public InvalidDeviceOptionsException()
            : base(message: "Invalid device options error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
