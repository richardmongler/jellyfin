using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Preferences.Brokers
{
    /// <summary>
    /// Provides integration operations against the custom item display preferences resource (no flow control).
    /// </summary>
    public interface ICustomItemDisplayPreferencesBroker
    {
        /// <summary>
        /// Inserts custom item display preferences asynchronously.
        /// </summary>
        /// <param name="customItemDisplayPreferences">The custom item display preferences to insert.</param>
        /// <returns>The inserted custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> InsertCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences);

        /// <summary>
        /// Selects all custom item display preferences asynchronously.
        /// </summary>
        /// <returns>All custom item display preferences.</returns>
        ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> SelectAllCustomItemDisplayPreferencesAsync();

        /// <summary>
        /// Selects custom item display preferences by their persistence identifier asynchronously.
        /// </summary>
        /// <param name="customItemDisplayPreferencesId">The custom item display preferences persistence identifier.</param>
        /// <returns>The matching custom item display preferences, or <c>null</c> when absent.</returns>
        ValueTask<CustomItemDisplayPreferences?> SelectCustomItemDisplayPreferencesByIdAsync(int customItemDisplayPreferencesId);

        /// <summary>
        /// Selects every custom item display preferences row for a natural key (user, item, client) asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <returns>Every matching custom item display preferences row for the key triple.</returns>
        ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> SelectCustomItemDisplayPreferencesByUserItemClientAsync(Guid userId, Guid itemId, string client);

        /// <summary>
        /// Selects custom item display preferences by their full natural key (user, item, client, key) asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <param name="key">The preference key.</param>
        /// <returns>The matching custom item display preferences, or <c>null</c> when absent.</returns>
        ValueTask<CustomItemDisplayPreferences?> SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(Guid userId, Guid itemId, string client, string key);

        /// <summary>
        /// Updates custom item display preferences asynchronously.
        /// </summary>
        /// <param name="customItemDisplayPreferences">The custom item display preferences to update.</param>
        /// <returns>The updated custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> UpdateCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences);

        /// <summary>
        /// Deletes custom item display preferences asynchronously.
        /// </summary>
        /// <param name="customItemDisplayPreferences">The custom item display preferences to delete.</param>
        /// <returns>The deleted custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> DeleteCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences);
    }
}
