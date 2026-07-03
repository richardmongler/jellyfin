using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Server.Implementations.Devices.Brokers
{
    /// <summary>
    /// Entity broker integrating the device resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class DeviceBroker : IDeviceBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public DeviceBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<Device> InsertDeviceAsync(Device device)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.Devices.Add(device);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return device;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<Device>> SelectAllDevicesAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.Devices
                .OrderBy(device => device.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<Device?> SelectDeviceByIdAsync(int deviceId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.Devices
                .FindAsync(deviceId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        // ponytail: external DeviceId is not guaranteed unique; pick the most recently active
        public async ValueTask<Device?> SelectDeviceByDeviceIdAsync(string deviceId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.Devices
                .Where(device => device.DeviceId == deviceId)
                .OrderByDescending(device => device.DateLastActivity)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<Device>> SelectDevicesByUserIdAsync(Guid userId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.Devices
                .Where(device => device.UserId.Equals(userId))
                .OrderBy(device => device.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<Device> UpdateDeviceAsync(Device device)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.Devices.Update(device);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return device;
        }

        /// <inheritdoc/>
        public async ValueTask<Device> DeleteDeviceAsync(Device device)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.Devices.Remove(device);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return device;
        }
    }
}
