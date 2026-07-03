using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Devices.Brokers;

namespace Jellyfin.Server.Implementations.Devices.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IDeviceOptionsBroker"/>; provides validated
    /// device options operations in business language (The-Standard 2.1).
    /// </summary>
    public interface IDeviceOptionsService
    {
        /// <summary>
        /// Adds device options after structural and logical validation.
        /// </summary>
        /// <param name="deviceOptions">The device options to add.</param>
        /// <returns>The added device options.</returns>
        ValueTask<DeviceOptions> AddDeviceOptionsAsync(DeviceOptions deviceOptions);

        /// <summary>
        /// Retrieves all device options.
        /// </summary>
        /// <returns>All device options.</returns>
        ValueTask<IReadOnlyList<DeviceOptions>> RetrieveAllDeviceOptionsAsync();

        /// <summary>
        /// Retrieves device options by their persistence identifier.
        /// </summary>
        /// <param name="deviceOptionsId">The device options persistence identifier.</param>
        /// <returns>The matching device options.</returns>
        ValueTask<DeviceOptions> RetrieveDeviceOptionsByIdAsync(int deviceOptionsId);

        /// <summary>
        /// Retrieves device options by their external device identifier.
        /// </summary>
        /// <param name="deviceId">The external device identifier.</param>
        /// <returns>The matching device options.</returns>
        ValueTask<DeviceOptions> RetrieveDeviceOptionsByDeviceIdAsync(string deviceId);

        /// <summary>
        /// Modifies device options after validation.
        /// </summary>
        /// <param name="deviceOptions">The device options to modify.</param>
        /// <returns>The modified device options.</returns>
        ValueTask<DeviceOptions> ModifyDeviceOptionsAsync(DeviceOptions deviceOptions);

        /// <summary>
        /// Removes device options after validation.
        /// </summary>
        /// <param name="deviceOptions">The device options to remove.</param>
        /// <returns>The removed device options.</returns>
        ValueTask<DeviceOptions> RemoveDeviceOptionsAsync(DeviceOptions deviceOptions);
    }
}
