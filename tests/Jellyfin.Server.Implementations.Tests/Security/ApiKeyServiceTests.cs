using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Tests.Security
{
    public partial class ApiKeyServiceTests
    {
        private static readonly Random random = new Random();

        private static ApiKey CreateRandomApiKey()
        {
            return new ApiKey(name: CreateRandomString())
            {
                DateCreated = DateTimeOffset.UtcNow.DateTime,
                DateLastActivity = DateTimeOffset.UtcNow.DateTime
            };
        }

        private static IReadOnlyList<ApiKey> CreateRandomApiKeys()
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(_ => CreateRandomApiKey())
                .ToList();
        }

        private static string CreateRandomAccessToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
