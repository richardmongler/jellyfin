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
    /// Entity broker integrating the item display preferences resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class ItemDisplayPreferencesBroker : IItemDisplayPreferencesBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDisplayPreferencesBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public ItemDisplayPreferencesBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<ItemDisplayPreferences> InsertItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ItemDisplayPreferences.Add(itemDisplayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return itemDisplayPreferences;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<ItemDisplayPreferences>> SelectAllItemDisplayPreferencesAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ItemDisplayPreferences
                .OrderBy(itemDisplayPreferences => itemDisplayPreferences.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<ItemDisplayPreferences?> SelectItemDisplayPreferencesByIdAsync(int itemDisplayPreferencesId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ItemDisplayPreferences
                .FindAsync(itemDisplayPreferencesId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        // ponytail: no unique index on (UserId, ItemId, Client) exists in ItemDisplayPreferencesConfiguration
        // (none is registered). Pick the most recently inserted match by Id to keep the lookup deterministic
        // when duplicates survive. Upgrade path: add a unique index mirroring DisplayPreferencesConfiguration,
        // then collapse this to FirstOrDefaultAsync without the ordering.
        public async ValueTask<ItemDisplayPreferences?> SelectItemDisplayPreferencesByUserItemClientAsync(
            Guid userId, Guid itemId, string client)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ItemDisplayPreferences
                .Where(itemDisplayPreferences =>
                    itemDisplayPreferences.UserId.Equals(userId)
                        && itemDisplayPreferences.ItemId.Equals(itemId)
                        && itemDisplayPreferences.Client == client)
                .OrderByDescending(itemDisplayPreferences => itemDisplayPreferences.Id)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<ItemDisplayPreferences> UpdateItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ItemDisplayPreferences.Update(itemDisplayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return itemDisplayPreferences;
        }

        /// <inheritdoc/>
        public async ValueTask<ItemDisplayPreferences> DeleteItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ItemDisplayPreferences.Remove(itemDisplayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return itemDisplayPreferences;
        }
    }
}
