using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities.Security;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Server.Implementations.Security.Brokers
{
    /// <summary>
    /// Entity broker integrating the API key resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class ApiKeyBroker : IApiKeyBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiKeyBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public ApiKeyBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<ApiKey> InsertApiKeyAsync(ApiKey apiKey)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ApiKeys.Add(apiKey);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return apiKey;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<ApiKey>> SelectAllApiKeysAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ApiKeys
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<ApiKey?> SelectApiKeyByIdAsync(int apiKeyId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ApiKeys
                .FindAsync(apiKeyId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<ApiKey?> SelectApiKeyByAccessTokenAsync(string accessToken)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ApiKeys
                .FirstOrDefaultAsync(apiKey => apiKey.AccessToken == accessToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<ApiKey> DeleteApiKeyAsync(ApiKey apiKey)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ApiKeys.Remove(apiKey);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return apiKey;
        }
    }
}
