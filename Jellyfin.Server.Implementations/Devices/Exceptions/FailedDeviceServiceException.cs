using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Describes a failed device service operation originating outside the device domain rules.
    /// </summary>
    public class FailedDeviceServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedDeviceServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedDeviceServiceException(Exception innerException)
            : base(message: "Failed device service error occurred, contact support.", innerException)
        {
        }
    }
}
