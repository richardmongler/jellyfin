using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Security.Brokers
{
    public interface IApiKeyBroker
    {
        ValueTask<ApiKey> InsertApiKeyAsync(ApiKey apiKey);
        ValueTask<IReadOnlyList<ApiKey>> SelectAllApiKeysAsync();
        ValueTask<ApiKey> SelectApiKeyByIdAsync(int apiKeyId);
        ValueTask<ApiKey> SelectApiKeyByAccessTokenAsync(string accessToken);
        ValueTask<ApiKey> DeleteApiKeyAsync(ApiKey apiKey);
    }
}
