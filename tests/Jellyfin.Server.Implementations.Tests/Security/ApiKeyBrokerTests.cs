using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Jellyfin.Server.Implementations.Security.Brokers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Security
{
    /// <summary>
    /// Standard Broker unit tests (Std 1.5): exercise the ApiKey Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each
    /// routine is asserted purely on resource integration.
    /// </summary>
    public sealed class ApiKeyBrokerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly ApiKeyBroker _broker;

        public ApiKeyBrokerTests()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            _dbOptions = new DbContextOptionsBuilder<JellyfinDbContext>()
                .UseSqlite(_connection)
                .Options;

            using var ctx = CreateDbContext();
            ctx.Database.EnsureCreated();

            var factory = new Mock<IDbContextFactory<JellyfinDbContext>>();
            factory.Setup(f => f.CreateDbContext()).Returns(CreateDbContext);
            factory.Setup(f => f.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreateDbContext);

            _broker = new ApiKeyBroker(factory.Object);
        }

        [Fact]
        public async Task InsertApiKeyAsync_AssignsIdentityAndPersists()
        {
            // given
            var apiKey = new ApiKey("broker-insert-app");

            // when
            var inserted = await _broker.InsertApiKeyAsync(apiKey);

            // then
            Assert.True(inserted.Id > 0);
            Assert.False(string.IsNullOrEmpty(inserted.AccessToken));

            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.ApiKeys.Count(k => k.Id == inserted.Id));
        }

        [Fact]
        public async Task SelectAllApiKeysAsync_ReturnsEveryPersistedKey()
        {
            // given
            await _broker.InsertApiKeyAsync(new ApiKey("first"));
            await _broker.InsertApiKeyAsync(new ApiKey("second"));

            // when
            IReadOnlyList<ApiKey> all = await _broker.SelectAllApiKeysAsync();

            // then
            Assert.Equal(2, all.Count);
        }

        [Fact]
        public async Task SelectApiKeyByIdAsync_Existing_FindsKey()
        {
            // given
            var inserted = await _broker.InsertApiKeyAsync(new ApiKey("found"));

            // when
            ApiKey? found = await _broker.SelectApiKeyByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
            Assert.Equal(inserted.AccessToken, found.AccessToken);
        }

        [Fact]
        public async Task SelectApiKeyByIdAsync_Absent_ReturnsNull()
        {
            // when
            ApiKey? found = await _broker.SelectApiKeyByIdAsync(777777);

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task SelectApiKeyByAccessTokenAsync_MatchingToken_FindsKey()
        {
            // given
            var inserted = await _broker.InsertApiKeyAsync(new ApiKey("token-app"));

            // when
            ApiKey? found = await _broker.SelectApiKeyByAccessTokenAsync(inserted.AccessToken);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectApiKeyByAccessTokenAsync_NoMatch_ReturnsNull()
        {
            // when
            ApiKey? found = await _broker.SelectApiKeyByAccessTokenAsync("does-not-exist");

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task DeleteApiKeyAsync_RemovesKeyFromStore()
        {
            // given
            var inserted = await _broker.InsertApiKeyAsync(new ApiKey("doomed"));

            // when
            await _broker.DeleteApiKeyAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            Assert.Null(await ctx.ApiKeys.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken));
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
