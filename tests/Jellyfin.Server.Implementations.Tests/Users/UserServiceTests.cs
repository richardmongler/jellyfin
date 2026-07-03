using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Tests.Users
{
    public partial class UserServiceTests
    {
        private static readonly Random random = new Random();

        private static User CreateRandomUser()
        {
            return new User(
                username: CreateRandomString(),
                authenticationProviderId: CreateRandomString(),
                passwordResetProviderId: CreateRandomString());
        }

        private static IReadOnlyList<User> CreateRandomUsers()
        {
            return Enumerable.Range(start: 0, count: GetRandomNumber())
                .Select(_ => CreateRandomUser())
                .ToList();
        }

        private static string CreateRandomString() =>
            Guid.NewGuid().ToString();

        private static int GetRandomNumber() =>
            random.Next(2, 9);
    }
}
