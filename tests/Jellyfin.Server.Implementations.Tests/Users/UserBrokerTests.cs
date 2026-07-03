using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Locking;
using Jellyfin.Database.Providers.Sqlite;
using Jellyfin.Server.Implementations.Users.Brokers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Users
{
    /// <summary>
    /// Standard Broker unit tests (Std 1.5): exercise the User Entity Broker against
    /// a real EF Core SQLite pipeline. No flow control lives in the broker, so each
    /// routine is asserted purely on resource integration.
    /// </summary>
    public sealed class UserBrokerTests : IDisposable, IAsyncLifetime
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<JellyfinDbContext> _dbOptions;
        private readonly UserBroker _broker;

        public UserBrokerTests()
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

            _broker = new UserBroker(factory.Object);
        }

        public async ValueTask InitializeAsync()
        {
            using var ctx = CreateDbContext();
            await ctx.Database.EnsureCreatedAsync();
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task InsertUserAsync_PersistsUserWithClientGeneratedId()
        {
            // given
            User input = NewUser(name: "alpha");

            // when
            var inserted = await _broker.InsertUserAsync(input);

            // then
            Assert.NotEqual(Guid.Empty, inserted.Id);
            using var ctx = CreateDbContext();
            Assert.Equal(1, ctx.Users.Count(u => u.Id.Equals(inserted.Id)));
        }

        [Fact]
        public async Task SelectAllUsersAsync_ReturnsEveryPersistedUser()
        {
            // given
            await _broker.InsertUserAsync(NewUser(name: "alpha"));
            await _broker.InsertUserAsync(NewUser(name: "beta"));

            // when
            IReadOnlyList<User> all = await _broker.SelectAllUsersAsync();

            // then
            Assert.Equal(2, all.Count);
            Assert.Equal(new[] { "alpha", "beta" }, all.Select(u => u.Username.ToLowerInvariant()));
        }

        [Fact]
        public async Task SelectUserByIdAsync_Existing_FindsUser()
        {
            // given
            var inserted = await _broker.InsertUserAsync(NewUser(name: "alpha"));

            // when
            User? found = await _broker.SelectUserByIdAsync(inserted.Id);

            // then
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public async Task SelectUserByIdAsync_Absent_ReturnsNull()
        {
            // when
            User? found = await _broker.SelectUserByIdAsync(Guid.NewGuid());

            // then
            Assert.Null(found);
        }

        [Fact]
        public async Task UpdateUserAsync_PersistsModifiedUsername()
        {
            // given
            var inserted = await _broker.InsertUserAsync(NewUser(name: "alpha"));
            inserted.Username = "renamed";

            // when
            var updated = await _broker.UpdateUserAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.Users.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.NotNull(stored);
            Assert.Equal("renamed", stored!.Username);
        }

        [Fact]
        public async Task DeleteUserAsync_RemovesUserFromStore()
        {
            // given
            var inserted = await _broker.InsertUserAsync(NewUser(name: "doomed"));

            // when
            await _broker.DeleteUserAsync(inserted);

            // then
            using var ctx = CreateDbContext();
            var stored = await ctx.Users.FindAsync(
                new object?[] { inserted.Id },
                TestContext.Current.CancellationToken);
            Assert.Null(stored);
        }

        private static User NewUser(string name) =>
            new User(name, "default", "default");

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
