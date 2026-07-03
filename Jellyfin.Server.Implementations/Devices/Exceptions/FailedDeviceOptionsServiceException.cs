using System;
using Jellyfin.Server.Implementations.Standard.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Describes a failed device options service operation originating outside the domain rules.
    /// </summary>
    public class FailedDeviceOptionsServiceException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedDeviceOptionsServiceException"/> class.
        /// </summary>
        /// <param name="innerException">The originating failure exception.</param>
        public FailedDeviceOptionsServiceException(Exception innerException)
            : base(message: "Failed device options service error occurred, contact support.", innerException)
        {
        }
    }
}
