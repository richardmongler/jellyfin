using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Security.Brokers;

namespace Jellyfin.Server.Implementations.Security.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IApiKeyBroker"/>; validates,
    /// integrates, and cancels exception noise for API key operations.
    /// </summary>
    public partial class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyBroker apiKeyBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyService"/> class.
        /// </summary>
        /// <param name="apiKeyBroker">The neighboring API key broker.</param>
        public ApiKeyService(IApiKeyBroker apiKeyBroker) =>
            this.apiKeyBroker = apiKeyBroker;

        /// <inheritdoc/>
        public ValueTask<ApiKey> AddApiKeyAsync(ApiKey apiKey) =>
            TryCatch(async () =>
            {
                ValidateApiKeyOnAdd(apiKey);

                return await this.apiKeyBroker.InsertApiKeyAsync(apiKey)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<ApiKey>> RetrieveAllApiKeysAsync() =>
            TryCatch(async () =>
            {
                return await this.apiKeyBroker.SelectAllApiKeysAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<ApiKey> RetrieveApiKeyByIdAsync(int apiKeyId) =>
            TryCatch(async () =>
            {
                ValidateApiKeyById(apiKeyId);

                ApiKey? apiKey = await this.apiKeyBroker.SelectApiKeyByIdAsync(apiKeyId)
                    .ConfigureAwait(false);

                ValidateApiKeyExists(apiKey, apiKeyId.ToString(CultureInfo.InvariantCulture));

                return apiKey!;
            });

        /// <inheritdoc/>
        public ValueTask<ApiKey> RetrieveApiKeyByAccessTokenAsync(string accessToken) =>
            TryCatch(async () =>
            {
                ValidateAccessToken(accessToken);

                ApiKey? apiKey = await this.apiKeyBroker.SelectApiKeyByAccessTokenAsync(accessToken)
                    .ConfigureAwait(false);

                ValidateApiKeyExists(apiKey, accessToken);

                return apiKey!;
            });

        /// <inheritdoc/>
        public ValueTask<ApiKey> RemoveApiKeyAsync(ApiKey apiKey) =>
            TryCatch(async () =>
            {
                ValidateApiKeyOnRemove(apiKey);

                return await this.apiKeyBroker.DeleteApiKeyAsync(apiKey)
                    .ConfigureAwait(false);
            });
    }
}
