using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Security.Services
{
    /// <summary>
    /// Foundation service providing validated API key operations (business language).
    /// </summary>
    public interface IApiKeyService
    {
        /// <summary>
        /// Adds an API key after structural and logical validation.
        /// </summary>
        /// <param name="apiKey">The API key to add.</param>
        /// <returns>The added API key.</returns>
        ValueTask<ApiKey> AddApiKeyAsync(ApiKey apiKey);

        /// <summary>
        /// Retrieves all API keys.
        /// </summary>
        /// <returns>All API keys.</returns>
        ValueTask<IReadOnlyList<ApiKey>> RetrieveAllApiKeysAsync();

        /// <summary>
        /// Retrieves an API key by its identifier.
        /// </summary>
        /// <param name="apiKeyId">The API key identifier.</param>
        /// <returns>The matching API key.</returns>
        ValueTask<ApiKey> RetrieveApiKeyByIdAsync(int apiKeyId);

        /// <summary>
        /// Retrieves an API key by its access token.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The matching API key.</returns>
        ValueTask<ApiKey> RetrieveApiKeyByAccessTokenAsync(string accessToken);

        /// <summary>
        /// Removes an API key after validation.
        /// </summary>
        /// <param name="apiKey">The API key to remove.</param>
        /// <returns>The removed API key.</returns>
        ValueTask<ApiKey> RemoveApiKeyAsync(ApiKey apiKey);
    }
}
