using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Preferences.Brokers
{
    /// <summary>
    /// Provides integration operations against the display preferences resource (no flow control).
    /// </summary>
    public interface IDisplayPreferencesBroker
    {
        /// <summary>
        /// Inserts display preferences asynchronously.
        /// </summary>
        /// <param name="displayPreferences">The display preferences to insert.</param>
        /// <returns>The inserted display preferences.</returns>
        ValueTask<DisplayPreferences> InsertDisplayPreferencesAsync(DisplayPreferences displayPreferences);

        /// <summary>
        /// Selects all display preferences asynchronously.
        /// </summary>
        /// <returns>All display preferences.</returns>
        ValueTask<IReadOnlyList<DisplayPreferences>> SelectAllDisplayPreferencesAsync();

        /// <summary>
        /// Selects display preferences by their persistence identifier asynchronously.
        /// </summary>
        /// <param name="displayPreferencesId">The display preferences persistence identifier.</param>
        /// <returns>The matching display preferences, or <c>null</c> when absent.</returns>
        ValueTask<DisplayPreferences?> SelectDisplayPreferencesByIdAsync(int displayPreferencesId);

        /// <summary>
        /// Selects display preferences by their natural key (user, item, client) asynchronously.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <returns>The matching display preferences, or <c>null</c> when absent.</returns>
        ValueTask<DisplayPreferences?> SelectDisplayPreferencesByUserItemClientAsync(Guid userId, Guid itemId, string client);

        /// <summary>
        /// Updates display preferences asynchronously.
        /// </summary>
        /// <param name="displayPreferences">The display preferences to update.</param>
        /// <returns>The updated display preferences.</returns>
        ValueTask<DisplayPreferences> UpdateDisplayPreferencesAsync(DisplayPreferences displayPreferences);

        /// <summary>
        /// Deletes display preferences asynchronously.
        /// </summary>
        /// <param name="displayPreferences">The display preferences to delete.</param>
        /// <returns>The deleted display preferences.</returns>
        ValueTask<DisplayPreferences> DeleteDisplayPreferencesAsync(DisplayPreferences displayPreferences);
    }
}
