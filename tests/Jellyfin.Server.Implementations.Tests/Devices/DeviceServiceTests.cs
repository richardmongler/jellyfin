using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Tests.Devices
{
    public partial class DeviceServiceTests
    {
        private static readonly Random random = new Random();

        private static Device CreateRandomDevice()
        {
            return new Device(
                userId: Guid.NewGuid(),
                appName: CreateRandomString(),
                appVersion: CreateRandomString(),
                deviceName: CreateRandomString(),
                deviceId: CreateRandomString())
            {
                DateCreated = DateTimeOffset.UtcNow.DateTime,
                DateLastActivity = DateTimeOffset.UtcNow.DateTime
            };
        }

        private static IReadOnlyList<Device> CreateRandomDevices()
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(_ => CreateRandomDevice())
                .ToList();
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
