using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Users.Brokers;

namespace Jellyfin.Server.Implementations.Users.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IUserBroker"/>; provides validated
    /// user operations in business language (The-Standard 2.1).
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adds a user after structural and logical validation.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>The added user.</returns>
        ValueTask<User> AddUserAsync(User user);

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>All users.</returns>
        ValueTask<IReadOnlyList<User>> RetrieveAllUsersAsync();

        /// <summary>
        /// Retrieves a user by its persistence identifier.
        /// </summary>
        /// <param name="userId">The user persistence identifier.</param>
        /// <returns>The matching user.</returns>
        ValueTask<User> RetrieveUserByIdAsync(Guid userId);

        /// <summary>
        /// Modifies a user after validation.
        /// </summary>
        /// <param name="user">The user to modify.</param>
        /// <returns>The modified user.</returns>
        ValueTask<User> ModifyUserAsync(User user);

        /// <summary>
        /// Removes a user after validation.
        /// </summary>
        /// <param name="user">The user to remove.</param>
        /// <returns>The removed user.</returns>
        ValueTask<User> RemoveUserAsync(User user);
    }
}
