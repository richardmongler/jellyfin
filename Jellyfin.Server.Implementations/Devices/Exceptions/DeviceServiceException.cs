using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Outer device service exception that wraps a <see cref="FailedDeviceServiceException"/>
    /// as the public surface for device service failures (The-Standard 2.0.4.0).
    /// </summary>
    public class DeviceServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public DeviceServiceException(FailedDeviceServiceException innerException)
            : base(message: "Device service error occurred, contact support.", innerException)
        {
        }
    }
}
