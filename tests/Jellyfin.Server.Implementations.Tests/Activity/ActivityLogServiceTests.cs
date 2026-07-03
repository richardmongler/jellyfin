using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Tests.Activity
{
    public partial class ActivityLogServiceTests
    {
        private static readonly Random random = new Random();

        private static ActivityLog CreateRandomActivityLog()
        {
            return new ActivityLog(
                name: CreateRandomString(),
                type: CreateRandomString(),
                userId: Guid.NewGuid());
        }

        private static IReadOnlyList<ActivityLog> CreateRandomActivityLogs()
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(_ => CreateRandomActivityLog())
                .ToList();
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
