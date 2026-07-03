using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Tests.Preferences
{
    public partial class ItemDisplayPreferencesServiceTests
    {
        private static readonly Random random = new Random();

        private static ItemDisplayPreferences CreateRandomItemDisplayPreferences()
        {
            return new ItemDisplayPreferences(
                userId: Guid.NewGuid(),
                itemId: Guid.NewGuid(),
                client: CreateRandomString());
        }

        private static IReadOnlyList<ItemDisplayPreferences> CreateRandomItemDisplayPreferencesList()
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(_ => CreateRandomItemDisplayPreferences())
                .ToList();
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
