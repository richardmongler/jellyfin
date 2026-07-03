using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Server.Implementations.Users.Brokers
{
    /// <summary>
    /// Entity broker integrating the user resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class UserBroker : IUserBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public UserBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<User> InsertUserAsync(User user)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return user;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<User>> SelectAllUsersAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.Users
                .OrderBy(user => user.Username)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<User?> SelectUserByIdAsync(Guid userId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.Users
                .FindAsync(userId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<User> UpdateUserAsync(User user)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return user;
        }

        /// <inheritdoc/>
        public async ValueTask<User> DeleteUserAsync(User user)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return user;
        }
    }
}
