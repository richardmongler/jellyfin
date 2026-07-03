using System;
using Jellyfin.Database.Implementations.Entities.Security;

namespace Jellyfin.Server.Implementations.Tests.Devices
{
    public partial class DeviceOptionsServiceTests
    {
        private static readonly Random random = new Random();

        private static DeviceOptions CreateRandomDeviceOptions()
        {
            return new DeviceOptions(CreateRandomString())
            {
                CustomName = CreateRandomString()
            };
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
