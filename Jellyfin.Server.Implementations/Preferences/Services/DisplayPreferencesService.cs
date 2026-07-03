using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Brokers;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IDisplayPreferencesBroker"/>; validates,
    /// integrates, and cancels exception noise for display preferences operations (The-Standard 2.1).
    /// </summary>
    public partial class DisplayPreferencesService : IDisplayPreferencesService
    {
        private readonly IDisplayPreferencesBroker displayPreferencesBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayPreferencesService"/> class.
        /// </summary>
        /// <param name="displayPreferencesBroker">The neighboring display preferences broker.</param>
        public DisplayPreferencesService(IDisplayPreferencesBroker displayPreferencesBroker) =>
            this.displayPreferencesBroker = displayPreferencesBroker;

        /// <inheritdoc/>
        public ValueTask<DisplayPreferences> AddDisplayPreferencesAsync(DisplayPreferences displayPreferences) =>
            TryCatch(async () =>
            {
                ValidateDisplayPreferencesOnAdd(displayPreferences);

                return await this.displayPreferencesBroker.InsertDisplayPreferencesAsync(displayPreferences)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<DisplayPreferences>> RetrieveAllDisplayPreferencesAsync() =>
            TryCatch(async () =>
            {
                return await this.displayPreferencesBroker.SelectAllDisplayPreferencesAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<DisplayPreferences> RetrieveDisplayPreferencesByIdAsync(int displayPreferencesId) =>
            TryCatch(async () =>
            {
                ValidateDisplayPreferencesById(displayPreferencesId);

                DisplayPreferences? displayPreferences = await this.displayPreferencesBroker
                    .SelectDisplayPreferencesByIdAsync(displayPreferencesId)
                    .ConfigureAwait(false);

                ValidateDisplayPreferencesExists(
                    displayPreferences,
                    displayPreferencesId.ToString(CultureInfo.InvariantCulture));

                return displayPreferences!;
            });

        /// <inheritdoc/>
        public ValueTask<DisplayPreferences> RetrieveDisplayPreferencesByUserItemClientAsync(
            Guid userId, Guid itemId, string client) =>
            TryCatch(async () =>
            {
                ValidateDisplayPreferencesByUserItemClient(userId, client);

                DisplayPreferences? displayPreferences = await this.displayPreferencesBroker
                    .SelectDisplayPreferencesByUserItemClientAsync(userId, itemId, client)
                    .ConfigureAwait(false);

                ValidateDisplayPreferencesExists(
                    displayPreferences,
                    $"{userId}/{itemId}/{client}");

                return displayPreferences!;
            });

        /// <inheritdoc/>
        public ValueTask<DisplayPreferences> ModifyDisplayPreferencesAsync(DisplayPreferences displayPreferences) =>
            TryCatch(async () =>
            {
                ValidateDisplayPreferencesOnModify(displayPreferences);

                return await this.displayPreferencesBroker.UpdateDisplayPreferencesAsync(displayPreferences)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<DisplayPreferences> RemoveDisplayPreferencesAsync(DisplayPreferences displayPreferences) =>
            TryCatch(async () =>
            {
                ValidateDisplayPreferencesOnRemove(displayPreferences);

                return await this.displayPreferencesBroker.DeleteDisplayPreferencesAsync(displayPreferences)
                    .ConfigureAwait(false);
            });
    }
}
