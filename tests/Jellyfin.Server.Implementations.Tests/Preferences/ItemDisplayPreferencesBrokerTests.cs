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
    /// Standard Broker unit tests (Std 1.5): exercise the ItemDisplayPreferences Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each routine is
    /// asserted purely on resource integration.
    /// </summary>
    public sealed class ItemDisplayPreferencesBrokerTests : IDisposable, IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly ItemDisplayPreferencesBroker _broker;
        private Guid _userId;

        public ItemDisplayPreferencesBrokerTests()
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

            _broker = new ItemDisplayPreferencesBroker(factory.Object);
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
        public async Task InsertItemDisplayPreferencesAsync_AssignsIdentityAndPersists()
        {
            // given
            var prefs = new ItemDisplayPreferences(_userId, itemId: Guid.NewGuid(), client: "web");

            // when
            var inserted = await _broker.InsertItemDisplayPreferencesAsync(prefs);

            // then
            Assert.True(inserted.Id > 0);

            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.ItemDisplayPreferences.Count(p => p.Id == inserted.Id));
        }

        [Fact]
        public async Task SelectAllItemDisplayPreferencesAsync_ReturnsEveryPersistedRow()
        {
            // given
            await _broker.InsertItemDisplayPreferencesAsync(new ItemDisplayPreferences(_userId, Guid.NewGuid(), "web"));
            await _broker.InsertItemDisplayPreferencesAsync(new ItemDisplayPreferences(_userId, Guid.NewGuid(), "tv"));

            // when
            IReadOnlyList<ItemDisplayPreferences> all = await _broker.SelectAllItemDisplayPreferencesAsync();

            // then
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task SelectItemDisplayPreferencesByIdAsync_Existing_FindsRow()
        {
            // given
            var inserted = await _broker.InsertItemDisplayPreferencesAsync(
                new ItemDisplayPreferences(_userId, Guid.NewGuid(), "web"));

            // when
            ItemDisplayPreferences? found = await _broker.SelectItemDisplayPreferencesByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectItemDisplayPreferencesByIdAsync_Absent_ReturnsNull()
        {
            // when
            ItemDisplayPreferences? found = await _broker.SelectItemDisplayPreferencesByIdAsync(777777);

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectItemDisplayPreferencesByUserItemClientAsync_Matching_FindsRow()
        {
            // given
            var itemId = Guid.NewGuid();
            const string client = "web";
            var inserted = await _broker.InsertItemDisplayPreferencesAsync(
                new ItemDisplayPreferences(_userId, itemId, client));

            // when
            ItemDisplayPreferences? found = await _broker.SelectItemDisplayPreferencesByUserItemClientAsync(_userId, itemId, client);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectItemDisplayPreferencesByUserItemClientAsync_NoMatch_ReturnsNull()
        {
            // when
            ItemDisplayPreferences? found = await _broker
                .SelectItemDisplayPreferencesByUserItemClientAsync(_userId, Guid.NewGuid(), "missing");

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectItemDisplayPreferencesByUserItemClientAsync_DuplicateNaturalKey_ReturnsMostRecentBy()
        {
            // given — two rows sharing the natural key (no unique index enforces singularity)
            var itemId = Guid.NewGuid();
            const string client = "web";
            var first = await _broker.InsertItemDisplayPreferencesAsync(
                new ItemDisplayPreferences(_userId, itemId, client));
            var second = await _broker.InsertItemDisplayPreferencesAsync(
                new ItemDisplayPreferences(_userId, itemId, client));

            // when
            ItemDisplayPreferences? found = await _broker
                .SelectItemDisplayPreferencesByUserItemClientAsync(_userId, itemId, client);

            // then — most recently inserted (greatest identity Id) wins
            Assert.NotNull(found);
            Assert.Equal(second.Id, found!.Id);
            Assert.True(second.Id > first.Id);
        }

        [Fact]
        public async Task SelectItemDisplayPreferencesByUserItemClientAsync_EmptyItemIdSentinel_FindsRow()
        {
            // given — Guid.Empty is the legacy "no item" sentinel; the broker must not exclude it
            const string client = "web";
            var inserted = await _broker.InsertItemDisplayPreferencesAsync(
                new ItemDisplayPreferences(_userId, Guid.Empty, client));

            // when
            ItemDisplayPreferences? found = await _broker
                .SelectItemDisplayPreferencesByUserItemClientAsync(_userId, Guid.Empty, client);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task UpdateItemDisplayPreferencesAsync_PersistsModifiedSortBy()
        {
            // given
            var inserted = await _broker.InsertItemDisplayPreferencesAsync(
                new ItemDisplayPreferences(_userId, Guid.NewGuid(), "web"));
            inserted.SortBy = "Random";

            // when
            var updated = await _broker.UpdateItemDisplayPreferencesAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.ItemDisplayPreferences.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.NotNull(stored);
            Assert.Equal("Random", stored!.SortBy);
            Assert.Same(inserted, updated);
        }

        [Fact]
        public async Task DeleteItemDisplayPreferencesAsync_RemovesRowFromStore()
        {
            // given
            var inserted = await _broker.InsertItemDisplayPreferencesAsync(
                new ItemDisplayPreferences(_userId, Guid.NewGuid(), "web"));

            // when
            await _broker.DeleteItemDisplayPreferencesAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.ItemDisplayPreferences.FindAsync(
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
