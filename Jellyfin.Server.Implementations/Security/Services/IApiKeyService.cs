using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Security.Services
{
    public interface IApiKeyService
    {
        ValueTask<ApiKey> AddApiKeyAsync(ApiKey apiKey);
        ValueTask<IReadOnlyList<ApiKey>> RetrieveAllApiKeysAsync();
        ValueTask<ApiKey> RetrieveApiKeyByIdAsync(int apiKeyId);
        ValueTask<ApiKey> RetrieveApiKeyByAccessTokenAsync(string accessToken);
        ValueTask<ApiKey> RemoveApiKeyAsync(ApiKey apiKey);
    }
}
