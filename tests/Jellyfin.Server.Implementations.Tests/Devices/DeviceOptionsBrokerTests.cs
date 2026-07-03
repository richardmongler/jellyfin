using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Jellyfin.Server.Implementations.Devices.Brokers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Devices
{
    /// <summary>
    /// Standard Broker unit tests (Std 1.5): exercise the DeviceOptions Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each routine is
    /// asserted purely on resource integration.
    /// </summary>
    public sealed class DeviceOptionsBrokerTests : System.IDisposable, IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly DeviceOptionsBroker _broker;

        public DeviceOptionsBrokerTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _dbOptions = new DbContextOptionsBuilder<JellyfinDbContext>()
                .UseSqlite(_connection)
                .Options;

            var factory = new Mock<IDbContextFactory<JellyfinDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(CreateDbContext);
            factory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateDbContext);

            _broker = new DeviceOptionsBroker(factory.Object);
        }

        public async ValueTask InitializeAsync()
        {
            using var ctx = CreateDbContext();
            await ctx.Database.EnsureCreatedAsync();
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task InsertDeviceOptionsAsync_AssignsIdentityAndPersists()
        {
            // given
            var deviceOptions = new DeviceOptions("device-1") { CustomName = "name-1" };

            // when
            var inserted = await _broker.InsertDeviceOptionsAsync(deviceOptions);

            // then
            Assert.True(inserted.Id > 0);
            Assert.Equal("device-1", inserted.DeviceId);
            Assert.Equal("name-1", inserted.CustomName);

            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.DeviceOptions.Count(o => o.Id == inserted.Id));
        }

        [Fact]
        public async Task SelectAllDeviceOptionsAsync_ReturnsEveryPersistedRecord()
        {
            // given
            await _broker.InsertDeviceOptionsAsync(new DeviceOptions("d1"));
            await _broker.InsertDeviceOptionsAsync(new DeviceOptions("d2"));

            // when
            IReadOnlyList<DeviceOptions> all = await _broker.SelectAllDeviceOptionsAsync();

            // then
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task SelectDeviceOptionsByIdAsync_Existing_FindsRecord()
        {
            // given
            var inserted = await _broker.InsertDeviceOptionsAsync(new DeviceOptions("d1"));

            // when
            DeviceOptions? found = await _broker.SelectDeviceOptionsByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectDeviceOptionsByIdAsync_Absent_ReturnsNull()
        {
            // when
            DeviceOptions? found = await _broker.SelectDeviceOptionsByIdAsync(777777);

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectDeviceOptionsByDeviceIdAsync_Matching_FindsRecord()
        {
            // given
            await _broker.InsertDeviceOptionsAsync(new DeviceOptions("d1") { CustomName = "alpha" });

            // when
            DeviceOptions? found = await _broker.SelectDeviceOptionsByDeviceIdAsync("d1");

            // then
            Assert.NotNull(found);
            Assert.Equal("alpha", found!.CustomName);
        }

        [Fact]
        public async Task SelectDeviceOptionsByDeviceIdAsync_NoMatch_ReturnsNull()
        {
            // when
            DeviceOptions? found = await _broker.SelectDeviceOptionsByDeviceIdAsync("does-not-exist");

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task UpdateDeviceOptionsAsync_PersistsModifiedCustomName()
        {
            // given
            var inserted = await _broker.InsertDeviceOptionsAsync(new DeviceOptions("d1"));
            inserted.CustomName = "renamed";

            // when
            var updated = await _broker.UpdateDeviceOptionsAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.DeviceOptions.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.NotNull(stored);
            Assert.Equal("renamed", stored!.CustomName);
        }

        [Fact]
        public async Task DeleteDeviceOptionsAsync_RemovesRecordFromStore()
        {
            // given
            var inserted = await _broker.InsertDeviceOptionsAsync(new DeviceOptions("doomed"));

            // when
            await _broker.DeleteDeviceOptionsAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.DeviceOptions.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.Null(stored);
        }

        private JellyfinDbContext CreateDbContext()
        {
            return new JellyfinDbContext(
                _dbOptions,
                NullLogger<JellyfinDbContext>.Instance,
                new SqliteDatabaseProvider(null!, NullLogger<SqliteDatabaseProvider>.Instance),
                new NoLockBehavior(NullLogger<NoLockBehavior>.Instance));
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
