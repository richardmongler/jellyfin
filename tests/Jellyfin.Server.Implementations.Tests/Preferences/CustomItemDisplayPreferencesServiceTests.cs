using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Tests.Preferences
{
    public partial class CustomItemDisplayPreferencesServiceTests
    {
        private static readonly Random random = new Random();

        private static CustomItemDisplayPreferences CreateRandomCustomItemDisplayPreferences()
        {
            return new CustomItemDisplayPreferences(
                userId: Guid.NewGuid(),
                itemId: Guid.NewGuid(),
                client: CreateRandomString(),
                key: CreateRandomString(),
                value: CreateRandomString());
        }

        private static IReadOnlyList<CustomItemDisplayPreferences> CreateRandomCustomItemDisplayPreferencesList()
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(_ => CreateRandomCustomItemDisplayPreferences())
                .ToList();
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
