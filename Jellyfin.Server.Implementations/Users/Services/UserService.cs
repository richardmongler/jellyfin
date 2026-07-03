using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Users.Brokers;

namespace Jellyfin.Server.Implementations.Users.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IUserBroker"/>; validates,
    /// integrates, and cancels exception noise for user operations (The-Standard 2.1).
    /// </summary>
    public partial class UserService : IUserService
    {
        private readonly IUserBroker userBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userBroker">The neighboring user broker.</param>
        public UserService(IUserBroker userBroker) =>
            this.userBroker = userBroker;

        /// <inheritdoc/>
        public ValueTask<User> AddUserAsync(User user) =>
            TryCatch(async () =>
            {
                ValidateUserOnAdd(user);

                return await this.userBroker.InsertUserAsync(user)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<User>> RetrieveAllUsersAsync() =>
            TryCatch(async () =>
            {
                return await this.userBroker.SelectAllUsersAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
            TryCatch(async () =>
            {
                ValidateUserById(userId);

                User? user = await this.userBroker.SelectUserByIdAsync(userId)
                    .ConfigureAwait(false);

                ValidateUserExists(user, userId.ToString());

                return user!;
            });

        /// <inheritdoc/>
        public ValueTask<User> ModifyUserAsync(User user) =>
            TryCatch(async () =>
            {
                ValidateUserOnModify(user);

                return await this.userBroker.UpdateUserAsync(user)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<User> RemoveUserAsync(User user) =>
            TryCatch(async () =>
            {
                ValidateUserOnRemove(user);

                return await this.userBroker.DeleteUserAsync(user)
                    .ConfigureAwait(false);
            });
    }
}
