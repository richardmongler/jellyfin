using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Server.Implementations.Devices.Brokers
{
    /// <summary>
    /// Entity broker integrating the device options resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class DeviceOptionsBroker : IDeviceOptionsBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceOptionsBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public DeviceOptionsBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<DeviceOptions> InsertDeviceOptionsAsync(DeviceOptions deviceOptions)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.DeviceOptions.Add(deviceOptions);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return deviceOptions;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<DeviceOptions>> SelectAllDeviceOptionsAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.DeviceOptions
                .OrderBy(deviceOptions => deviceOptions.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<DeviceOptions?> SelectDeviceOptionsByIdAsync(int deviceOptionsId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.DeviceOptions
                .FindAsync(deviceOptionsId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<DeviceOptions?> SelectDeviceOptionsByDeviceIdAsync(string deviceId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.DeviceOptions
                .FirstOrDefaultAsync(deviceOptions => deviceOptions.DeviceId == deviceId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<DeviceOptions> UpdateDeviceOptionsAsync(DeviceOptions deviceOptions)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.DeviceOptions.Update(deviceOptions);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return deviceOptions;
        }

        /// <inheritdoc/>
        public async ValueTask<DeviceOptions> DeleteDeviceOptionsAsync(DeviceOptions deviceOptions)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.DeviceOptions.Remove(deviceOptions);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return deviceOptions;
        }
    }
}
