using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Brokers;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="ICustomItemDisplayPreferencesBroker"/>; provides validated
    /// custom item display preferences operations in business language (The-Standard 2.1).
    /// </summary>
    public interface ICustomItemDisplayPreferencesService
    {
        /// <summary>
        /// Adds custom item display preferences after structural and logical validation.
        /// </summary>
        /// <param name="customItemDisplayPreferences">The custom item display preferences to add.</param>
        /// <returns>The added custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> AddCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences);

        /// <summary>
        /// Retrieves all custom item display preferences.
        /// </summary>
        /// <returns>All custom item display preferences.</returns>
        ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> RetrieveAllCustomItemDisplayPreferencesAsync();

        /// <summary>
        /// Retrieves custom item display preferences by their persistence identifier.
        /// </summary>
        /// <param name="customItemDisplayPreferencesId">The custom item display preferences persistence identifier.</param>
        /// <returns>The matching custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> RetrieveCustomItemDisplayPreferencesByIdAsync(int customItemDisplayPreferencesId);

        /// <summary>
        /// Retrieves every custom item display preferences row for a natural key (user, item, client).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <returns>Every matching custom item display preferences row for the key triple.</returns>
        ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> RetrieveCustomItemDisplayPreferencesByUserItemClientAsync(Guid userId, Guid itemId, string client);

        /// <summary>
        /// Retrieves custom item display preferences by their full natural key (user, item, client, key).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <param name="key">The preference key.</param>
        /// <returns>The matching custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync(Guid userId, Guid itemId, string client, string key);

        /// <summary>
        /// Modifies custom item display preferences after validation.
        /// </summary>
        /// <param name="customItemDisplayPreferences">The custom item display preferences to modify.</param>
        /// <returns>The modified custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> ModifyCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences);

        /// <summary>
        /// Removes custom item display preferences after validation.
        /// </summary>
        /// <param name="customItemDisplayPreferences">The custom item display preferences to remove.</param>
        /// <returns>The removed custom item display preferences.</returns>
        ValueTask<CustomItemDisplayPreferences> RemoveCustomItemDisplayPreferencesAsync(CustomItemDisplayPreferences customItemDisplayPreferences);
    }
}
