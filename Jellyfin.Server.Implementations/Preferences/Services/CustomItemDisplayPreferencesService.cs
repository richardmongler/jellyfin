using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Brokers;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="ICustomItemDisplayPreferencesBroker"/>; validates,
    /// integrates, and cancels exception noise for custom item display preferences operations (The-Standard 2.1).
    /// </summary>
    public partial class CustomItemDisplayPreferencesService : ICustomItemDisplayPreferencesService
    {
        private readonly ICustomItemDisplayPreferencesBroker customItemDisplayPreferencesBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemDisplayPreferencesService"/> class.
        /// </summary>
        /// <param name="customItemDisplayPreferencesBroker">The neighboring custom item display preferences broker.</param>
        public CustomItemDisplayPreferencesService(ICustomItemDisplayPreferencesBroker customItemDisplayPreferencesBroker) =>
            this.customItemDisplayPreferencesBroker = customItemDisplayPreferencesBroker;

        /// <inheritdoc/>
        public ValueTask<CustomItemDisplayPreferences> AddCustomItemDisplayPreferencesAsync(
            CustomItemDisplayPreferences customItemDisplayPreferences) =>
            TryCatch(async () =>
            {
                ValidateCustomItemDisplayPreferencesOnAdd(customItemDisplayPreferences);

                return await this.customItemDisplayPreferencesBroker
                    .InsertCustomItemDisplayPreferencesAsync(customItemDisplayPreferences)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> RetrieveAllCustomItemDisplayPreferencesAsync() =>
            TryCatch(async () =>
            {
                return await this.customItemDisplayPreferencesBroker.SelectAllCustomItemDisplayPreferencesAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<CustomItemDisplayPreferences> RetrieveCustomItemDisplayPreferencesByIdAsync(
            int customItemDisplayPreferencesId) =>
            TryCatch(async () =>
            {
                ValidateCustomItemDisplayPreferencesById(customItemDisplayPreferencesId);

                CustomItemDisplayPreferences? customItemDisplayPreferences = await this.customItemDisplayPreferencesBroker
                    .SelectCustomItemDisplayPreferencesByIdAsync(customItemDisplayPreferencesId)
                    .ConfigureAwait(false);

                ValidateCustomItemDisplayPreferencesExists(
                    customItemDisplayPreferences,
                    customItemDisplayPreferencesId.ToString(CultureInfo.InvariantCulture));

                return customItemDisplayPreferences!;
            });

        /// <inheritdoc/>
        // ponytail: a natural-key list retrieve is multi-row; an empty result is a valid "no custom prefs set"
        // state (the legacy manager projects it to an empty dictionary), so no external existence validation
        // fires here — only the singular key retrieve below enforces NotFound (Std 2.1.3.1.4).
        public ValueTask<IReadOnlyList<CustomItemDisplayPreferences>> RetrieveCustomItemDisplayPreferencesByUserItemClientAsync(
            Guid userId, Guid itemId, string client) =>
            TryCatch(async () =>
            {
                ValidateCustomItemDisplayPreferencesByUserItemClient(userId, client);

                return await this.customItemDisplayPreferencesBroker
                    .SelectCustomItemDisplayPreferencesByUserItemClientAsync(userId, itemId, client)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<CustomItemDisplayPreferences> RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync(
            Guid userId, Guid itemId, string client, string key) =>
            TryCatch(async () =>
            {
                ValidateCustomItemDisplayPreferencesByUserItemClientKey(userId, client, key);

                CustomItemDisplayPreferences? customItemDisplayPreferences = await this.customItemDisplayPreferencesBroker
                    .SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(userId, itemId, client, key)
                    .ConfigureAwait(false);

                ValidateCustomItemDisplayPreferencesExists(
                    customItemDisplayPreferences,
                    $"{userId}/{itemId}/{client}/{key}");

                return customItemDisplayPreferences!;
            });

        /// <inheritdoc/>
        public ValueTask<CustomItemDisplayPreferences> ModifyCustomItemDisplayPreferencesAsync(
            CustomItemDisplayPreferences customItemDisplayPreferences) =>
            TryCatch(async () =>
            {
                ValidateCustomItemDisplayPreferencesOnModify(customItemDisplayPreferences);

                return await this.customItemDisplayPreferencesBroker
                    .UpdateCustomItemDisplayPreferencesAsync(customItemDisplayPreferences)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<CustomItemDisplayPreferences> RemoveCustomItemDisplayPreferencesAsync(
            CustomItemDisplayPreferences customItemDisplayPreferences) =>
            TryCatch(async () =>
            {
                ValidateCustomItemDisplayPreferencesOnRemove(customItemDisplayPreferences);

                return await this.customItemDisplayPreferencesBroker
                    .DeleteCustomItemDisplayPreferencesAsync(customItemDisplayPreferences)
                    .ConfigureAwait(false);
            });
    }
}
