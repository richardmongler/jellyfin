using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Security.Brokers
{
    /// <summary>
    /// Provides integration operations against the API key resource (no flow control).
    /// </summary>
    public interface IApiKeyBroker
    {
        /// <summary>
        /// Inserts an API key asynchronously.
        /// </summary>
        /// <param name="apiKey">The API key to insert.</param>
        /// <returns>The inserted API key.</returns>
        ValueTask<ApiKey> InsertApiKeyAsync(ApiKey apiKey);

        /// <summary>
        /// Selects all API keys asynchronously.
        /// </summary>
        /// <returns>All API keys.</returns>
        ValueTask<IReadOnlyList<ApiKey>> SelectAllApiKeysAsync();

        /// <summary>
        /// Selects an API key by its identifier asynchronously.
        /// </summary>
        /// <param name="apiKeyId">The API key identifier.</param>
        /// <returns>The matching API key, or <c>null</c> when absent.</returns>
        ValueTask<ApiKey?> SelectApiKeyByIdAsync(int apiKeyId);

        /// <summary>
        /// Selects an API key by its access token asynchronously.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns>The matching API key, or <c>null</c> when absent.</returns>
        ValueTask<ApiKey?> SelectApiKeyByAccessTokenAsync(string accessToken);

        /// <summary>
        /// Deletes an API key asynchronously.
        /// </summary>
        /// <param name="apiKey">The API key to delete.</param>
        /// <returns>The deleted API key.</returns>
        ValueTask<ApiKey> DeleteApiKeyAsync(ApiKey apiKey);
    }
}
