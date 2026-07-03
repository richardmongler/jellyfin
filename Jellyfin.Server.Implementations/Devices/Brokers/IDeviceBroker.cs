using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Devices.Brokers
{
    /// <summary>
    /// Provides integration operations against the device resource (no flow control).
    /// </summary>
    public interface IDeviceBroker
    {
        /// <summary>
        /// Inserts a device asynchronously.
        /// </summary>
        /// <param name="device">The device to insert.</param>
        /// <returns>The inserted device.</returns>
        ValueTask<Device> InsertDeviceAsync(Device device);

        /// <summary>
        /// Selects all devices asynchronously.
        /// </summary>
        /// <returns>All devices.</returns>
        ValueTask<IReadOnlyList<Device>> SelectAllDevicesAsync();

        /// <summary>
        /// Selects a device by its persistence identifier asynchronously.
        /// </summary>
        /// <param name="deviceId">The device persistence identifier.</param>
        /// <returns>The matching device, or <c>null</c> when absent.</returns>
        ValueTask<Device?> SelectDeviceByIdAsync(int deviceId);

        /// <summary>
        /// Selects a device by its external device identifier asynchronously.
        /// </summary>
        /// <param name="deviceId">The external device identifier.</param>
        /// <returns>The most recently active matching device, or <c>null</c> when absent.</returns>
        ValueTask<Device?> SelectDeviceByDeviceIdAsync(string deviceId);

        /// <summary>
        /// Selects all devices for a user asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Every device belonging to the user.</returns>
        ValueTask<IReadOnlyList<Device>> SelectDevicesByUserIdAsync(Guid userId);

        /// <summary>
        /// Updates a device asynchronously.
        /// </summary>
        /// <param name="device">The device to update.</param>
        /// <returns>The updated device.</returns>
        ValueTask<Device> UpdateDeviceAsync(Device device);

        /// <summary>
        /// Deletes a device asynchronously.
        /// </summary>
        /// <param name="device">The device to delete.</param>
        /// <returns>The deleted device.</returns>
        ValueTask<Device> DeleteDeviceAsync(Device device);
    }
}
