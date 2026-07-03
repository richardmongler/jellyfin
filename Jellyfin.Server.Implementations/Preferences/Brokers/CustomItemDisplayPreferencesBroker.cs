using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Server.Implementations.Preferences.Brokers
{
    /// <summary>
    /// Entity broker integrating the custom item display preferences resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class CustomItemDisplayPreferencesBroker : ICustomItemDisplayPreferencesBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemDisplayPreferencesBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public CustomItemDisplayPreferencesBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<CustomItemDisplayPreferences> InsertCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.CustomItemDisplayPreferences.Add(customItemDisplayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return customItemDisplayPreferences;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> SelectAllCustomItemDisplayPreferencesAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.CustomItemDisplayPreferences
                .OrderBy(customItemDisplayPreferences => customItemDisplayPreferences.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<CustomItemDisplayPreferences?> SelectCustomItemDisplayPreferencesByIdAsync(int customItemDisplayPreferencesId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.CustomItemDisplayPreferences
                .FindAsync(customItemDisplayPreferencesId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> SelectCustomItemDisplayPreferencesByUserItemClientAsync(
            Guid userId, Guid itemId, string client)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.CustomItemDisplayPreferences
                .Where(customItemDisplayPreferences =>
                    customItemDisplayPreferences.UserId.Equals(userId)
                        && customItemDisplayPreferences.ItemId.Equals(itemId)
                        && customItemDisplayPreferences.Client == client)
                .OrderBy(customItemDisplayPreferences => customItemDisplayPreferences.Key)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        // ponytail: CustomItemDisplayPreferencesConfiguration declares a unique index on
        // (UserId, ItemId, Client, Key), so the lookup is single-row by construction — no ordering
        // or most-recent ambiguity. Upgrade path: none required while the unique index is enforced.
        public async ValueTask<CustomItemDisplayPreferences?> SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(
            Guid userId, Guid itemId, string client, string key)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.CustomItemDisplayPreferences
                .FirstOrDefaultAsync(customItemDisplayPreferences =>
                    customItemDisplayPreferences.UserId.Equals(userId)
                        && customItemDisplayPreferences.ItemId.Equals(itemId)
                        && customItemDisplayPreferences.Client == client
                        && customItemDisplayPreferences.Key == key)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<CustomItemDisplayPreferences> UpdateCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.CustomItemDisplayPreferences.Update(customItemDisplayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return customItemDisplayPreferences;
        }

        /// <inheritdoc/>
        public async ValueTask<CustomItemDisplayPreferences> DeleteCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.CustomItemDisplayPreferences.Remove(customItemDisplayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return customItemDisplayPreferences;
        }
    }
}
