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
    /// Entity broker integrating the display preferences resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class DisplayPreferencesBroker : IDisplayPreferencesBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayPreferencesBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public DisplayPreferencesBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<DisplayPreferences> InsertDisplayPreferencesAsync(DisplayPreferences displayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.DisplayPreferences.Add(displayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return displayPreferences;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<DisplayPreferences>> SelectAllDisplayPreferencesAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.DisplayPreferences
                .OrderBy(displayPreferences => displayPreferences.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<DisplayPreferences?> SelectDisplayPreferencesByIdAsync(int displayPreferencesId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.DisplayPreferences
                .FindAsync(displayPreferencesId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        // ponytail: (UserId, ItemId, Client) is a unique index in DisplayPreferencesConfiguration
        public async ValueTask<DisplayPreferences?> SelectDisplayPreferencesByUserItemClientAsync(
            Guid userId, Guid itemId, string client)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.DisplayPreferences
                .Where(displayPreferences =>
                    displayPreferences.UserId.Equals(userId)
                        && displayPreferences.ItemId.Equals(itemId)
                        && displayPreferences.Client == client)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<DisplayPreferences> UpdateDisplayPreferencesAsync(DisplayPreferences displayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.DisplayPreferences.Update(displayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return displayPreferences;
        }

        /// <inheritdoc/>
        public async ValueTask<DisplayPreferences> DeleteDisplayPreferencesAsync(DisplayPreferences displayPreferences)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.DisplayPreferences.Remove(displayPreferences);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return displayPreferences;
        }
    }
}
