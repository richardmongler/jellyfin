using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Exceptions
{
    /// <summary>
    /// Exception thrown when requested device options cannot be located.
    /// </summary>
    public class DeviceOptionsNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceOptionsNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that failed to match device options.</param>
        public DeviceOptionsNotFoundException(string identifier)
            : base(message: $"Device options with identifier '{identifier}' were not found.")
        {
        }
    }
}
