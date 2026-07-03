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
    /// Standard Broker unit tests (Std 1.5): exercise the CustomItemDisplayPreferences Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each routine is asserted
    /// purely on resource integration.
    /// </summary>
    public sealed class CustomItemDisplayPreferencesBrokerTests : IDisposable, IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly CustomItemDisplayPreferencesBroker _broker;
        private Guid _userId;

        public CustomItemDisplayPreferencesBrokerTests()
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

            _broker = new CustomItemDisplayPreferencesBroker(factory.Object);
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
        public async Task InsertCustomItemDisplayPreferencesAsync_AssignsIdentityAndPersists()
        {
            // given
            var prefs = new CustomItemDisplayPreferences(_userId, itemId: Guid.NewGuid(), client: "web", key: "k1", value: "v1");

            // when
            var inserted = await _broker.InsertCustomItemDisplayPreferencesAsync(prefs);

            // then
            Assert.True(inserted.Id > 0);

            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.CustomItemDisplayPreferences.Count(p => p.Id == inserted.Id));
        }

        [Fact]
        public async Task SelectAllCustomItemDisplayPreferencesAsync_ReturnsEveryPersistedRow()
        {
            // given
            await _broker.InsertCustomItemDisplayPreferencesAsync(new CustomItemDisplayPreferences(_userId, Guid.NewGuid(), "web", "k1", "v1"));
            await _broker.InsertCustomItemDisplayPreferencesAsync(new CustomItemDisplayPreferences(_userId, Guid.NewGuid(), "tv", "k2", "v2"));

            // when
            IReadOnlyList<CustomItemDisplayPreferences> all = await _broker.SelectAllCustomItemDisplayPreferencesAsync();

            // then
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByIdAsync_Existing_FindsRow()
        {
            // given
            var inserted = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, Guid.NewGuid(), "web", "k1", "v1"));

            // when
            CustomItemDisplayPreferences? found = await _broker.SelectCustomItemDisplayPreferencesByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByIdAsync_Absent_ReturnsNull()
        {
            // when
            CustomItemDisplayPreferences? found = await _broker.SelectCustomItemDisplayPreferencesByIdAsync(777777);

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByUserItemClientAsync_ReturnsAllKeysForTriple()
        {
            // given — same (user, item, client) triple, two distinct keys (a key/value map resource)
            var itemId = Guid.NewGuid();
            const string client = "web";
            var first = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, itemId, client, key: "k1", value: "v1"));
            var second = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, itemId, client, key: "k2", value: "v2"));

            // when
            IReadOnlyList<CustomItemDisplayPreferences> rows = await _broker
                .SelectCustomItemDisplayPreferencesByUserItemClientAsync(_userId, itemId, client);

            // then
            Assert.Equal(2, rows.Count);
            Assert.Contains(rows, r => r.Id == first.Id);
            Assert.Contains(rows, r => r.Id == second.Id);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByUserItemClientAsync_OrdersByKey()
        {
            // given — insert out of key order
            var itemId = Guid.NewGuid();
            const string client = "web";
            await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, itemId, client, key: "zeta", value: "v1"));
            await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, itemId, client, key: "alpha", value: "v2"));

            // when
            IReadOnlyList<CustomItemDisplayPreferences> rows = await _broker
                .SelectCustomItemDisplayPreferencesByUserItemClientAsync(_userId, itemId, client);

            // then
            Assert.Equal(2, rows.Count);
            Assert.Equal("alpha", rows[0].Key);
            Assert.Equal("zeta", rows[1].Key);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByUserItemClientAsync_NoMatch_ReturnsEmptyList()
        {
            // given — empty result is a valid "no custom prefs set" state for the multi-row lookup
            // when
            IReadOnlyList<CustomItemDisplayPreferences> rows = await _broker
                .SelectCustomItemDisplayPreferencesByUserItemClientAsync(_userId, Guid.NewGuid(), "missing");

            // then
            Assert.Empty(rows);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByUserItemClientAsync_EmptyItemIdSentinel_FindsRow()
        {
            // given — Guid.Empty is the legacy "no item" sentinel; the broker must not exclude it
            const string client = "web";
            var inserted = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, Guid.Empty, client, key: "k1", value: "v1"));

            // when
            IReadOnlyList<CustomItemDisplayPreferences> rows = await _broker
                .SelectCustomItemDisplayPreferencesByUserItemClientAsync(_userId, Guid.Empty, client);

            // then
            Assert.Single(rows);
            Assert.Equal(inserted.Id, rows[0].Id);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync_Existing_FindsSingleRow()
        {
            // given — unique index on (UserId, ItemId, Client, Key) pins the lookup to a single row
            var itemId = Guid.NewGuid();
            const string client = "web";
            const string key = "k1";
            var inserted = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, itemId, client, key, value: "v1"));
            await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, itemId, client, key: "k2", value: "v2"));

            // when
            CustomItemDisplayPreferences? found = await _broker
                .SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(_userId, itemId, client, key);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
            Assert.Equal("v1", found.Value);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync_Absent_ReturnsNull()
        {
            // when
            CustomItemDisplayPreferences? found = await _broker
                .SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(_userId, Guid.NewGuid(), "web", "missing");

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync_EmptyItemIdSentinel_FindsRow()
        {
            // given — Guid.Empty ItemId is a valid stored coordinate
            const string client = "web";
            const string key = "k1";
            var inserted = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, Guid.Empty, client, key, value: "v1"));

            // when
            CustomItemDisplayPreferences? found = await _broker
                .SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(_userId, Guid.Empty, client, key);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task UpdateCustomItemDisplayPreferencesAsync_PersistsModifiedValue()
        {
            // given
            var inserted = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, Guid.NewGuid(), "web", "k1", value: "v1"));
            inserted.Value = "updated";

            // when
            var updated = await _broker.UpdateCustomItemDisplayPreferencesAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.CustomItemDisplayPreferences.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.NotNull(stored);
            Assert.Equal("updated", stored!.Value);
            Assert.Same(inserted, updated);
        }

        [Fact]
        public async Task DeleteCustomItemDisplayPreferencesAsync_RemovesRowFromStore()
        {
            // given
            var inserted = await _broker.InsertCustomItemDisplayPreferencesAsync(
                new CustomItemDisplayPreferences(_userId, Guid.NewGuid(), "web", "k1", value: "v1"));

            // when
            await _broker.DeleteCustomItemDisplayPreferencesAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.CustomItemDisplayPreferences.FindAsync(
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
