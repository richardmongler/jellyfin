using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Jellyfin.Server.Implementations.Activity.Brokers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Activity
{
    /// <summary>
    /// Standard Broker unit tests (Std 1.5): exercise the ActivityLog Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each routine is
    /// asserted purely on resource integration.
    /// </summary>
    public sealed class ActivityLogBrokerTests : System.IDisposable, IAsyncLifetime
    {
        private static readonly System.Guid TestUserId = System.Guid.NewGuid();
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly ActivityLogBroker _broker;

        public ActivityLogBrokerTests()
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

            _broker = new ActivityLogBroker(factory.Object);
        }

        public async ValueTask InitializeAsync()
        {
            using var ctx = CreateDbContext();
            await ctx.Database.EnsureCreatedAsync();
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task InsertActivityLogAsync_AssignsIdentityAndPersists()
        {
            // given
            var activityLog = new ActivityLog("name-1", "type-1", TestUserId);

            // when
            var inserted = await _broker.InsertActivityLogAsync(activityLog);

            // then
            Assert.True(inserted.Id > 0);
            Assert.Equal("name-1", inserted.Name);
            Assert.Equal("type-1", inserted.Type);
            Assert.Equal(TestUserId, inserted.UserId);

            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.ActivityLogs.Count(log => log.Id == inserted.Id));
        }

        [Fact]
        public async Task SelectAllActivityLogsAsync_ReturnsEveryPersistedRecord()
        {
            // given
            await _broker.InsertActivityLogAsync(new ActivityLog("n1", "t1", TestUserId));
            await _broker.InsertActivityLogAsync(new ActivityLog("n2", "t2", TestUserId));

            // when
            IReadOnlyList<ActivityLog> all = await _broker.SelectAllActivityLogsAsync();

            // then
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task SelectActivityLogByIdAsync_Existing_FindsRecord()
        {
            // given
            var inserted = await _broker.InsertActivityLogAsync(new ActivityLog("n1", "t1", TestUserId));

            // when
            ActivityLog? found = await _broker.SelectActivityLogByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectActivityLogByIdAsync_Absent_ReturnsNull()
        {
            // when
            ActivityLog? found = await _broker.SelectActivityLogByIdAsync(777777);

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task UpdateActivityLogAsync_PersistsModifiedOverview()
        {
            // given
            var inserted = await _broker.InsertActivityLogAsync(new ActivityLog("n1", "t1", TestUserId));
            inserted.Overview = "renamed";

            // when
            var updated = await _broker.UpdateActivityLogAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.ActivityLogs.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.NotNull(stored);
            Assert.Equal("renamed", stored!.Overview);
        }

        [Fact]
        public async Task DeleteActivityLogAsync_RemovesRecordFromStore()
        {
            // given
            var inserted = await _broker.InsertActivityLogAsync(new ActivityLog("doomed", "t1", TestUserId));

            // when
            await _broker.DeleteActivityLogAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.ActivityLogs.FindAsync(
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
