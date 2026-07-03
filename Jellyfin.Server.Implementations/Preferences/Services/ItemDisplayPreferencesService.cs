using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Brokers;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IItemDisplayPreferencesBroker"/>; validates,
    /// integrates, and cancels exception noise for item display preferences operations (The-Standard 2.1).
    /// </summary>
    public partial class ItemDisplayPreferencesService : IItemDisplayPreferencesService
    {
        private readonly IItemDisplayPreferencesBroker itemDisplayPreferencesBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDisplayPreferencesService"/> class.
        /// </summary>
        /// <param name="itemDisplayPreferencesBroker">The neighboring item display preferences broker.</param>
        public ItemDisplayPreferencesService(IItemDisplayPreferencesBroker itemDisplayPreferencesBroker) =>
            this.itemDisplayPreferencesBroker = itemDisplayPreferencesBroker;

        /// <inheritdoc/>
        public ValueTask<ItemDisplayPreferences> AddItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences) =>
            TryCatch(async () =>
            {
                ValidateItemDisplayPreferencesOnAdd(itemDisplayPreferences);

                return await this.itemDisplayPreferencesBroker.InsertItemDisplayPreferencesAsync(itemDisplayPreferences)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<ItemDisplayPreferences>> RetrieveAllItemDisplayPreferencesAsync() =>
            TryCatch(async () =>
            {
                return await this.itemDisplayPreferencesBroker.SelectAllItemDisplayPreferencesAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<ItemDisplayPreferences> RetrieveItemDisplayPreferencesByIdAsync(int itemDisplayPreferencesId) =>
            TryCatch(async () =>
            {
                ValidateItemDisplayPreferencesById(itemDisplayPreferencesId);

                ItemDisplayPreferences? itemDisplayPreferences = await this.itemDisplayPreferencesBroker
                    .SelectItemDisplayPreferencesByIdAsync(itemDisplayPreferencesId)
                    .ConfigureAwait(false);

                ValidateItemDisplayPreferencesExists(
                    itemDisplayPreferences,
                    itemDisplayPreferencesId.ToString(CultureInfo.InvariantCulture));

                return itemDisplayPreferences!;
            });

        /// <inheritdoc/>
        public ValueTask<ItemDisplayPreferences> RetrieveItemDisplayPreferencesByUserItemClientAsync(
            Guid userId, Guid itemId, string client) =>
            TryCatch(async () =>
            {
                ValidateItemDisplayPreferencesByUserItemClient(userId, client);

                ItemDisplayPreferences? itemDisplayPreferences = await this.itemDisplayPreferencesBroker
                    .SelectItemDisplayPreferencesByUserItemClientAsync(userId, itemId, client)
                    .ConfigureAwait(false);

                ValidateItemDisplayPreferencesExists(
                    itemDisplayPreferences,
                    $"{userId}/{itemId}/{client}");

                return itemDisplayPreferences!;
            });

        /// <inheritdoc/>
        public ValueTask<ItemDisplayPreferences> ModifyItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences) =>
            TryCatch(async () =>
            {
                ValidateItemDisplayPreferencesOnModify(itemDisplayPreferences);

                return await this.itemDisplayPreferencesBroker.UpdateItemDisplayPreferencesAsync(itemDisplayPreferences)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<ItemDisplayPreferences> RemoveItemDisplayPreferencesAsync(ItemDisplayPreferences itemDisplayPreferences) =>
            TryCatch(async () =>
            {
                ValidateItemDisplayPreferencesOnRemove(itemDisplayPreferences);

                return await this.itemDisplayPreferencesBroker.DeleteItemDisplayPreferencesAsync(itemDisplayPreferences)
                    .ConfigureAwait(false);
            });
    }
}
