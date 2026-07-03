using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Security.Brokers;

namespace Jellyfin.Server.Implementations.Security.Services
{
    public partial class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyBroker apiKeyBroker;

        public ApiKeyService(IApiKeyBroker apiKeyBroker) =>
            this.apiKeyBroker = apiKeyBroker;

        public ValueTask<ApiKey> AddApiKeyAsync(ApiKey apiKey) =>
        TryCatch(async () =>
        {
            ValidateApiKeyOnAdd(apiKey);

            return await this.apiKeyBroker.InsertApiKeyAsync(apiKey)
                .ConfigureAwait(false);
        });

        public ValueTask<IReadOnlyList<ApiKey>> RetrieveAllApiKeysAsync() =>
        TryCatch(async () =>
        {
            return await this.apiKeyBroker.SelectAllApiKeysAsync()
                .ConfigureAwait(false);
        });

        public ValueTask<ApiKey> RetrieveApiKeyByIdAsync(int apiKeyId) =>
        TryCatch(async () =>
        {
            ValidateApiKeyById(apiKeyId);

            return await this.apiKeyBroker.SelectApiKeyByIdAsync(apiKeyId)
                .ConfigureAwait(false);
        });

        public ValueTask<ApiKey> RetrieveApiKeyByAccessTokenAsync(string accessToken) =>
        TryCatch(async () =>
        {
            ValidateAccessToken(accessToken);

            return await this.apiKeyBroker.SelectApiKeyByAccessTokenAsync(accessToken)
                .ConfigureAwait(false);
        });

        public ValueTask<ApiKey> RemoveApiKeyAsync(ApiKey apiKey) =>
        TryCatch(async () =>
        {
            ValidateApiKeyOnRemove(apiKey);

            return await this.apiKeyBroker.DeleteApiKeyAsync(apiKey)
                .ConfigureAwait(false);
        });
    }
}
