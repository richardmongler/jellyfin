using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Brokers;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IItemDisplayPreferencesBroker"/>; provides validated
    /// item display preferences operations in business language (The-Standard 2.1).
    /// </summary>
    public interface IItemDisplayPreferencesService
    {
        /// <summary>
        /// Adds item display preferences after structural and logical validation.
        /// </summary>
        /// <param name="itemDisplayPreferences">The item display preferences to add.</param>
        /// <returns>The added item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> AddItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences);

        /// <summary>
        /// Retrieves all item display preferences.
        /// </summary>
        /// <returns>All item display preferences.</returns>
        ValueTask<IReadOnlyList<ItemDisplayPreferences>> RetrieveAllItemDisplayPreferencesAsync();

        /// <summary>
        /// Retrieves item display preferences by their persistence identifier.
        /// </summary>
        /// <param name="itemDisplayPreferencesId">The item display preferences persistence identifier.</param>
        /// <returns>The matching item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> RetrieveItemDisplayPreferencesByIdAsync(int itemDisplayPreferencesId);

        /// <summary>
        /// Retrieves item display preferences by their natural key (user, item, client).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <returns>The matching item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> RetrieveItemDisplayPreferencesByUserItemClientAsync(Guid userId, Guid itemId, string client);

        /// <summary>
        /// Modifies item display preferences after validation.
        /// </summary>
        /// <param name="itemDisplayPreferences">The item display preferences to modify.</param>
        /// <returns>The modified item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> ModifyItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences);

        /// <summary>
        /// Removes item display preferences after validation.
        /// </summary>
        /// <param name="itemDisplayPreferences">The item display preferences to remove.</param>
        /// <returns>The removed item display preferences.</returns>
        ValueTask<ItemDisplayPreferences> RemoveItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences);
    }
}
