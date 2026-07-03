using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Devices.Brokers;

namespace Jellyfin.Server.Implementations.Devices.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IDeviceOptionsBroker"/>; validates,
    /// integrates, and cancels exception noise for device options operations (The-Standard 2.1).
    /// </summary>
    public partial class DeviceOptionsService : IDeviceOptionsService
    {
        private readonly IDeviceOptionsBroker deviceOptionsBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceOptionsService"/> class.
        /// </summary>
        /// <param name="deviceOptionsBroker">The neighboring device options broker.</param>
        public DeviceOptionsService(IDeviceOptionsBroker deviceOptionsBroker) =>
            this.deviceOptionsBroker = deviceOptionsBroker;

        /// <inheritdoc/>
        public ValueTask<DeviceOptions> AddDeviceOptionsAsync(DeviceOptions deviceOptions) =>
            TryCatch(async () =>
            {
                ValidateDeviceOptionsOnAdd(deviceOptions);

                return await this.deviceOptionsBroker.InsertDeviceOptionsAsync(deviceOptions)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<DeviceOptions>> RetrieveAllDeviceOptionsAsync() =>
            TryCatch(async () =>
            {
                return await this.deviceOptionsBroker.SelectAllDeviceOptionsAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<DeviceOptions> RetrieveDeviceOptionsByIdAsync(int deviceOptionsId) =>
            TryCatch(async () =>
            {
                ValidateDeviceOptionsById(deviceOptionsId);

                DeviceOptions? deviceOptions = await this.deviceOptionsBroker
                    .SelectDeviceOptionsByIdAsync(deviceOptionsId)
                    .ConfigureAwait(false);

                ValidateDeviceOptionsExists(
                    deviceOptions,
                    deviceOptionsId.ToString(System.Globalization.CultureInfo.InvariantCulture));

                return deviceOptions!;
            });

        /// <inheritdoc/>
        public ValueTask<DeviceOptions> RetrieveDeviceOptionsByDeviceIdAsync(string deviceId) =>
            TryCatch(async () =>
            {
                ValidateDeviceOptionsByDeviceId(deviceId);

                DeviceOptions? deviceOptions = await this.deviceOptionsBroker
                    .SelectDeviceOptionsByDeviceIdAsync(deviceId)
                    .ConfigureAwait(false);

                ValidateDeviceOptionsExists(deviceOptions, deviceId);

                return deviceOptions!;
            });

        /// <inheritdoc/>
        public ValueTask<DeviceOptions> ModifyDeviceOptionsAsync(DeviceOptions deviceOptions) =>
            TryCatch(async () =>
            {
                ValidateDeviceOptionsOnModify(deviceOptions);

                return await this.deviceOptionsBroker.UpdateDeviceOptionsAsync(deviceOptions)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<DeviceOptions> RemoveDeviceOptionsAsync(DeviceOptions deviceOptions) =>
            TryCatch(async () =>
            {
                ValidateDeviceOptionsOnRemove(deviceOptions);

                return await this.deviceOptionsBroker.DeleteDeviceOptionsAsync(deviceOptions)
                    .ConfigureAwait(false);
            });
    }
}
