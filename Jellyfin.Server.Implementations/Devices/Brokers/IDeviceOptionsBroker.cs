using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Devices.Brokers
{
    /// <summary>
    /// Provides integration operations against the device options resource (no flow control).
    /// </summary>
    public interface IDeviceOptionsBroker
    {
        /// <summary>
        /// Inserts device options asynchronously.
        /// </summary>
        /// <param name="deviceOptions">The device options to insert.</param>
        /// <returns>The inserted device options.</returns>
        ValueTask<DeviceOptions> InsertDeviceOptionsAsync(DeviceOptions deviceOptions);

        /// <summary>
        /// Selects all device options asynchronously.
        /// </summary>
        /// <returns>All device options.</returns>
        ValueTask<IReadOnlyList<DeviceOptions>> SelectAllDeviceOptionsAsync();

        /// <summary>
        /// Selects device options by their persistence identifier asynchronously.
        /// </summary>
        /// <param name="deviceOptionsId">The device options persistence identifier.</param>
        /// <returns>The matching device options, or <c>null</c> when absent.</returns>
        ValueTask<DeviceOptions?> SelectDeviceOptionsByIdAsync(int deviceOptionsId);

        /// <summary>
        /// Selects device options by their external device identifier asynchronously.
        /// </summary>
        /// <param name="deviceId">The external device identifier.</param>
        /// <returns>The matching device options, or <c>null</c> when absent.</returns>
        ValueTask<DeviceOptions?> SelectDeviceOptionsByDeviceIdAsync(string deviceId);

        /// <summary>
        /// Updates device options asynchronously.
        /// </summary>
        /// <param name="deviceOptions">The device options to update.</param>
        /// <returns>The updated device options.</returns>
        ValueTask<DeviceOptions> UpdateDeviceOptionsAsync(DeviceOptions deviceOptions);

        /// <summary>
        /// Deletes device options asynchronously.
        /// </summary>
        /// <param name="deviceOptions">The device options to delete.</param>
        /// <returns>The deleted device options.</returns>
        ValueTask<DeviceOptions> DeleteDeviceOptionsAsync(DeviceOptions deviceOptions);
    }
}
