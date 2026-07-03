using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Outer device options service exception that wraps a
    /// <see cref="FailedDeviceOptionsServiceException"/> as the public surface for device
    /// options service failures (The-Standard 2.0.4.0).
    /// </summary>
    public class DeviceOptionsServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceOptionsServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public DeviceOptionsServiceException(FailedDeviceOptionsServiceException innerException)
            : base(message: "Device options service error occurred, contact support.", innerException)
        {
        }
    }
}
