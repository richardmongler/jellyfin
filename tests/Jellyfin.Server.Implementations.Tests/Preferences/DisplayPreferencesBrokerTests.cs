using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Jellyfin.Server.Implementations.Preferences.Brokers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Preferences
{
    /// <summary>
    /// Standard Broker unit tests (Std 1.5): exercise the DisplayPreferences Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each routine is
    /// asserted purely on resource integration.
    /// </summary>
    public sealed class DisplayPreferencesBrokerTests : IDisposable, IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly DisplayPreferencesBroker _broker;
        private Guid _userId;

        public DisplayPreferencesBrokerTests()
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

            _broker = new DisplayPreferencesBroker(factory.Object);
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
        public async Task InsertDisplayPreferencesAsync_AssignsIdentityAndPersists()
        {
            // given
            var prefs = new DisplayPreferences(_userId, itemId: Guid.NewGuid(), client: "web");

            // when
            var inserted = await _broker.InsertDisplayPreferencesAsync(prefs);

            // then
            Assert.True(inserted.Id > 0);

            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.DisplayPreferences.Count(p => p.Id == inserted.Id));
        }

        [Fact]
        public async Task SelectAllDisplayPreferencesAsync_ReturnsEveryPersistedRow()
        {
            // given
            await _broker.InsertDisplayPreferencesAsync(new DisplayPreferences(_userId, Guid.NewGuid(), "web"));
            await _broker.InsertDisplayPreferencesAsync(new DisplayPreferences(_userId, Guid.NewGuid(), "tv"));

            // when
            IReadOnlyList<DisplayPreferences> all = await _broker.SelectAllDisplayPreferencesAsync();

            // then
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task SelectDisplayPreferencesByIdAsync_Existing_FindsRow()
        {
            // given
            var inserted = await _broker.InsertDisplayPreferencesAsync(
                new DisplayPreferences(_userId, Guid.NewGuid(), "web"));

            // when
            DisplayPreferences? found = await _broker.SelectDisplayPreferencesByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectDisplayPreferencesByIdAsync_Absent_ReturnsNull()
        {
            // when
            DisplayPreferences? found = await _broker.SelectDisplayPreferencesByIdAsync(777777);

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectDisplayPreferencesByUserItemClientAsync_Matching_FindsRow()
        {
            // given
            var itemId = Guid.NewGuid();
            const string client = "web";
            var inserted = await _broker.InsertDisplayPreferencesAsync(
                new DisplayPreferences(_userId, itemId, client));

            // when
            DisplayPreferences? found = await _broker.SelectDisplayPreferencesByUserItemClientAsync(_userId, itemId, client);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectDisplayPreferencesByUserItemClientAsync_NoMatch_ReturnsNull()
        {
            // when
            DisplayPreferences? found = await _broker
                .SelectDisplayPreferencesByUserItemClientAsync(_userId, Guid.NewGuid(), "missing");

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task UpdateDisplayPreferencesAsync_PersistsModifiedClient()
        {
            // given
            var inserted = await _broker.InsertDisplayPreferencesAsync(
                new DisplayPreferences(_userId, Guid.NewGuid(), "web"));
            inserted.ShowSidebar = true;

            // when
            var updated = await _broker.UpdateDisplayPreferencesAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.DisplayPreferences.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.NotNull(stored);
            Assert.True(stored!.ShowSidebar);
            Assert.Same(inserted, updated);
        }

        [Fact]
        public async Task DeleteDisplayPreferencesAsync_RemovesRowFromStore()
        {
            // given
            var inserted = await _broker.InsertDisplayPreferencesAsync(
                new DisplayPreferences(_userId, Guid.NewGuid(), "web"));

            // when
            await _broker.DeleteDisplayPreferencesAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.DisplayPreferences.FindAsync(
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
