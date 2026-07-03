using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Validation exception thrown when a device fails structural or logical validation.
    /// </summary>
    public class InvalidDeviceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidDeviceException"/> class.
        /// </summary>
        public InvalidDeviceException()
            : base(message: "Invalid device error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
