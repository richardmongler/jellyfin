using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Preferences.Brokers
{
    /// <summary>
    /// Provides integration operations against the item display preferences resource (no flow control).
    /// </summary>
    public interface IItemDisplayPreferencesBroker
    {
        /// <summary>
        /// Inserts item display preferences asynchronously.
        /// </summary>
        /// <param name="itemDisplayPreferences">The item display preferences to insert.</param>
        /// <returns>The inserted item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> InsertItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences);

        /// <summary>
        /// Selects all item display preferences asynchronously.
        /// </summary>
        /// <returns>All item display preferences.</returns>
        ValueTask<IReadOnlyList<ItemDisplayPreferences>> SelectAllItemDisplayPreferencesAsync();

        /// <summary>
        /// Selects item display preferences by their persistence identifier asynchronously.
        /// </summary>
        /// <param name="itemDisplayPreferencesId">The item display preferences persistence identifier.</param>
        /// <returns>The matching item display preferences, or <c>null</c> when absent.</returns>
        ValueTask<ItemDisplayPreferences?> SelectItemDisplayPreferencesByIdAsync(int itemDisplayPreferencesId);

        /// <summary>
        /// Selects item display preferences by their natural key (user, item, client) asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <returns>The matching item display preferences, or <c>null</c> when absent.</returns>
        ValueTask<ItemDisplayPreferences?> SelectItemDisplayPreferencesByUserItemClientAsync(Guid userId, Guid itemId, string client);

        /// <summary>
        /// Updates item display preferences asynchronously.
        /// </summary>
        /// <param name="itemDisplayPreferences">The item display preferences to update.</param>
        /// <returns>The updated item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> UpdateItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences);

        /// <summary>
        /// Deletes item display preferences asynchronously.
        /// </summary>
        /// <param name="itemDisplayPreferences">The item display preferences to delete.</param>
        /// <returns>The deleted item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> DeleteItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences);
    }
}
