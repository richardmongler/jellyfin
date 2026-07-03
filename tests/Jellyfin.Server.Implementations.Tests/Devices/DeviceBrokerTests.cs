using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
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
    /// Standard Broker unit tests (Std 1.5): exercise the Device Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each
    /// routine is asserted purely on resource integration.
    /// </summary>
    public sealed class DeviceBrokerTests : IDisposable, IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly DeviceBroker _broker;
        private Guid _userId;

        public DeviceBrokerTests()
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

            _broker = new DeviceBroker(factory.Object);
        }

        public async ValueTask InitializeAsync()
        {
            using var ctx = CreateDbContext();
            await ctx.Database.EnsureCreatedAsync();

            var user = new User("standard-user", "default", "default");
            _userId = user.Id;
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task InsertDeviceAsync_AssignsIdentityAndPersists()
        {
            // given
            var device = new Device(_userId, "app", "1.0", "name", "device-1");

            // when
            var inserted = await _broker.InsertDeviceAsync(device);

            // then
            Assert.True(inserted.Id > 0);
            Assert.False(string.IsNullOrEmpty(inserted.AccessToken));

            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.Devices.Count(d => d.Id == inserted.Id));
        }

        [Fact]
        public async Task SelectAllDevicesAsync_ReturnsEveryPersistedDevice()
        {
            // given
            await _broker.InsertDeviceAsync(new Device(_userId, "a", "1", "n", "d1"));
            await _broker.InsertDeviceAsync(new Device(_userId, "b", "1", "n", "d2"));

            // when
            IReadOnlyList<Device> all = await _broker.SelectAllDevicesAsync();

            // then
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task SelectDeviceByIdAsync_Existing_FindsDevice()
        {
            // given
            var inserted = await _broker.InsertDeviceAsync(new Device(_userId, "a", "1", "n", "d1"));

            // when
            Device? found = await _broker.SelectDeviceByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectDeviceByIdAsync_Absent_ReturnsNull()
        {
            // when
            Device? found = await _broker.SelectDeviceByIdAsync(777777);

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectDeviceByDeviceIdAsync_Matching_FindsMostRecentlyActiveDevice()
        {
            // given
            var older = await _broker.InsertDeviceAsync(new Device(_userId, "a", "1", "n", "shared"));
            older.DateLastActivity = older.DateCreated.AddHours(-1);
            await _broker.UpdateDeviceAsync(older);

            var newer = await _broker.InsertDeviceAsync(new Device(_userId, "a", "1", "n", "shared"));

            // when
            Device? found = await _broker.SelectDeviceByDeviceIdAsync("shared");

            // then
            Assert.NotNull(found);
            Assert.Equal(newer.Id, found!.Id);
        }

        [Fact]
        public async Task SelectDeviceByDeviceIdAsync_NoMatch_ReturnsNull()
        {
            // when
            Device? found = await _broker.SelectDeviceByDeviceIdAsync("does-not-exist");

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectDevicesByUserIdAsync_ReturnsOnlyThatUsersDevices()
        {
            // given
            var otherUserId = Guid.NewGuid();
            using (var ctx = CreateDbContext())
            {
                var otherUser = new User("other-user", "default", "default") { Id = otherUserId };
                ctx.Users.Add(otherUser);
                await ctx.SaveChangesAsync(TestContext.Current.CancellationToken);
            }

            await _broker.InsertDeviceAsync(new Device(_userId, "a", "1", "n", "mine"));
            await _broker.InsertDeviceAsync(new Device(otherUserId, "a", "1", "n", "theirs"));

            // when
            IReadOnlyList<Device> devices = await _broker.SelectDevicesByUserIdAsync(_userId);

            // then
            Assert.Single(devices);
            Assert.All(devices, d => Assert.Equal(_userId, d.UserId));
        }

        [Fact]
        public async Task UpdateDeviceAsync_PersistsModifiedDeviceName()
        {
            // given
            var inserted = await _broker.InsertDeviceAsync(new Device(_userId, "a", "1", "n", "d1"));
            inserted.DeviceName = "renamed";

            // when
            var updated = await _broker.UpdateDeviceAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.Devices.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.NotNull(stored);
            Assert.Equal("renamed", stored!.DeviceName);
        }

        [Fact]
        public async Task DeleteDeviceAsync_RemovesDeviceFromStore()
        {
            // given
            var inserted = await _broker.InsertDeviceAsync(new Device(_userId, "a", "1", "n", "doomed"));

            // when
            await _broker.DeleteDeviceAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.Devices.FindAsync(
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
