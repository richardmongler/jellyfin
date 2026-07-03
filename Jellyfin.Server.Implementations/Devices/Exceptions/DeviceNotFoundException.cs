using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested device cannot be located.
    /// </summary>
    public class DeviceNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that failed to match a device.</param>
        public DeviceNotFoundException(string identifier)
            : base(message: $"Device with identifier '{identifier}' was not found.")
        {
        }
    }
}
