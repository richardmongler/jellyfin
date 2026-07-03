using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Users.Brokers
{
    /// <summary>
    /// Provides integration operations against the user resource (no flow control).
    /// </summary>
    public interface IUserBroker
    {
        /// <summary>
        /// Inserts a user asynchronously.
        /// </summary>
        /// <param name="user">The user to insert.</param>
        /// <returns>The inserted user.</returns>
        ValueTask<User> InsertUserAsync(User user);

        /// <summary>
        /// Selects all users asynchronously.
        /// </summary>
        /// <returns>All users.</returns>
        ValueTask<IReadOnlyList<User>> SelectAllUsersAsync();

        /// <summary>
        /// Selects a user by its persistence identifier asynchronously.
        /// </summary>
        /// <param name="userId">The user persistence identifier.</param>
        /// <returns>The matching user, or <c>null</c> when absent.</returns>
        ValueTask<User?> SelectUserByIdAsync(Guid userId);

        /// <summary>
        /// Updates a user asynchronously.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>The updated user.</returns>
        ValueTask<User> UpdateUserAsync(User user);

        /// <summary>
        /// Deletes a user asynchronously.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <returns>The deleted user.</returns>
        ValueTask<User> DeleteUserAsync(User user);
    }
}
