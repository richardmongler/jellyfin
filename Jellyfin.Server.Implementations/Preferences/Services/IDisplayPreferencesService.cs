using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Brokers;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IDisplayPreferencesBroker"/>; provides validated
    /// display preferences operations in business language (The-Standard 2.1).
    /// </summary>
    public interface IDisplayPreferencesService
    {
        /// <summary>
        /// Adds display preferences after structural and logical validation.
        /// </summary>
        /// <param name="displayPreferences">The display preferences to add.</param>
        /// <returns>The added display preferences.</returns>
        ValueTask<DisplayPreferences> AddDisplayPreferencesAsync(DisplayPreferences displayPreferences);

        /// <summary>
        /// Retrieves all display preferences.
        /// </summary>
        /// <returns>All display preferences.</returns>
        ValueTask<IReadOnlyList<DisplayPreferences>> RetrieveAllDisplayPreferencesAsync();

        /// <summary>
        /// Retrieves display preferences by their persistence identifier.
        /// </summary>
        /// <param name="displayPreferencesId">The display preferences persistence identifier.</param>
        /// <returns>The matching display preferences.</returns>
        ValueTask<DisplayPreferences> RetrieveDisplayPreferencesByIdAsync(int displayPreferencesId);

        /// <summary>
        /// Retrieves display preferences by their natural key (user, item, client).
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="client">The client string.</param>
        /// <returns>The matching display preferences.</returns>
        ValueTask<DisplayPreferences> RetrieveDisplayPreferencesByUserItemClientAsync(Guid userId, Guid itemId, string client);

        /// <summary>
        /// Modifies display preferences after validation.
        /// </summary>
        /// <param name="displayPreferences">The display preferences to modify.</param>
        /// <returns>The modified display preferences.</returns>
        ValueTask<DisplayPreferences> ModifyDisplayPreferencesAsync(DisplayPreferences displayPreferences);

        /// <summary>
        /// Removes display preferences after validation.
        /// </summary>
        /// <param name="displayPreferences">The display preferences to remove.</param>
        /// <returns>The removed display preferences.</returns>
        ValueTask<DisplayPreferences> RemoveDisplayPreferencesAsync(DisplayPreferences displayPreferences);
    }
}
