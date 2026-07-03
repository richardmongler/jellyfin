using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Devices.Brokers;

namespace Jellyfin.Server.Implementations.Devices.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IDeviceBroker"/>; validates,
    /// integrates, and cancels exception noise for device operations (The-Standard 2.1).
    /// </summary>
    public partial class DeviceService : IDeviceService
    {
        private readonly IDeviceBroker deviceBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceService"/> class.
        /// </summary>
        /// <param name="deviceBroker">The neighboring device broker.</param>
        public DeviceService(IDeviceBroker deviceBroker) =>
            this.deviceBroker = deviceBroker;

        /// <inheritdoc/>
        public ValueTask<Device> AddDeviceAsync(Device device) =>
            TryCatch(async () =>
            {
                ValidateDeviceOnAdd(device);

                return await this.deviceBroker.InsertDeviceAsync(device)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<Device>> RetrieveAllDevicesAsync() =>
            TryCatch(async () =>
            {
                return await this.deviceBroker.SelectAllDevicesAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<Device> RetrieveDeviceByIdAsync(int deviceId) =>
            TryCatch(async () =>
            {
                ValidateDeviceById(deviceId);

                Device? device = await this.deviceBroker.SelectDeviceByIdAsync(deviceId)
                    .ConfigureAwait(false);

                ValidateDeviceExists(device, deviceId.ToString(System.Globalization.CultureInfo.InvariantCulture));

                return device!;
            });

        /// <inheritdoc/>
        public ValueTask<Device> RetrieveDeviceByDeviceIdAsync(string deviceId) =>
            TryCatch(async () =>
            {
                ValidateDeviceByDeviceId(deviceId);

                Device? device = await this.deviceBroker.SelectDeviceByDeviceIdAsync(deviceId)
                    .ConfigureAwait(false);

                ValidateDeviceExists(device, deviceId);

                return device!;
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<Device>> RetrieveDevicesByUserIdAsync(Guid userId) =>
            TryCatch(async () =>
            {
                ValidateDeviceByUserId(userId);

                return await this.deviceBroker.SelectDevicesByUserIdAsync(userId)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<Device> ModifyDeviceAsync(Device device) =>
            TryCatch(async () =>
            {
                ValidateDeviceOnModify(device);

                return await this.deviceBroker.UpdateDeviceAsync(device)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<Device> RemoveDeviceAsync(Device device) =>
            TryCatch(async () =>
            {
                ValidateDeviceOnRemove(device);

                return await this.deviceBroker.DeleteDeviceAsync(device)
                    .ConfigureAwait(false);
            });
    }
}
