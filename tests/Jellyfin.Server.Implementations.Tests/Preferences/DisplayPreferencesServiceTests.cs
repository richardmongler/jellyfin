using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Tests.Preferences
{
    public partial class DisplayPreferencesServiceTests
    {
        private static readonly Random random = new Random();

        private static DisplayPreferences CreateRandomDisplayPreferences()
        {
            return new DisplayPreferences(
                userId: Guid.NewGuid(),
                itemId: Guid.NewGuid(),
                client: CreateRandomString());
        }

        private static IReadOnlyList<DisplayPreferences> CreateRandomDisplayPreferencesList()
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(_ => CreateRandomDisplayPreferences())
                .ToList();
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
