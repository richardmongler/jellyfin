using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Devices.Brokers;

namespace Jellyfin.Server.Implementations.Devices.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IDeviceBroker"/>; provides validated
    /// device operations in business language (The-Standard 2.1).
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Adds a device after structural and logical validation.
        /// </summary>
        /// <param name="device">The device to add.</param>
        /// <returns>The added device.</returns>
        ValueTask<Device> AddDeviceAsync(Device device);

        /// <summary>
        /// Retrieves all devices.
        /// </summary>
        /// <returns>All devices.</returns>
        ValueTask<IReadOnlyList<Device>> RetrieveAllDevicesAsync();

        /// <summary>
        /// Retrieves a device by its persistence identifier.
        /// </summary>
        /// <param name="deviceId">The device persistence identifier.</param>
        /// <returns>The matching device.</returns>
        ValueTask<Device> RetrieveDeviceByIdAsync(int deviceId);

        /// <summary>
        /// Retrieves a device by its external device identifier.
        /// </summary>
        /// <param name="deviceId">The external device identifier.</param>
        /// <returns>The matching device.</returns>
        ValueTask<Device> RetrieveDeviceByDeviceIdAsync(string deviceId);

        /// <summary>
        /// Retrieves every device belonging to a user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>The user's devices.</returns>
        ValueTask<IReadOnlyList<Device>> RetrieveDevicesByUserIdAsync(Guid userId);

        /// <summary>
        /// Modifies a device after validation.
        /// </summary>
        /// <param name="device">The device to modify.</param>
        /// <returns>The modified device.</returns>
        ValueTask<Device> ModifyDeviceAsync(Device device);

        /// <summary>
        /// Removes a device after validation.
        /// </summary>
        /// <param name="device">The device to remove.</param>
        /// <returns>The removed device.</returns>
        ValueTask<Device> RemoveDeviceAsync(Device device);
    }
}
