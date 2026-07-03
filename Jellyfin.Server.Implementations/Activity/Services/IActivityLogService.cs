using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Activity.Brokers;

namespace Jellyfin.Server.Implementations.Activity.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IActivityLogBroker"/>; provides validated
    /// activity-log operations in business language (The-Standard 2.1).
    /// </summary>
    public interface IActivityLogService
    {
        /// <summary>
        /// Adds an activity-log entry after structural and logical validation.
        /// </summary>
        /// <param name="activityLog">The entry to add.</param>
        /// <returns>The added entry.</returns>
        ValueTask<ActivityLog> AddActivityLogAsync(ActivityLog activityLog);

        /// <summary>
        /// Retrieves all activity-log entries.
        /// </summary>
        /// <returns>All entries.</returns>
        ValueTask<IReadOnlyList<ActivityLog>> RetrieveAllActivityLogsAsync();

        /// <summary>
        /// Retrieves an activity-log entry by its persistence identifier.
        /// </summary>
        /// <param name="activityLogId">The entry persistence identifier.</param>
        /// <returns>The matching entry.</returns>
        ValueTask<ActivityLog> RetrieveActivityLogByIdAsync(int activityLogId);

        /// <summary>
        /// Modifies an activity-log entry after validation.
        /// </summary>
        /// <param name="activityLog">The entry to modify.</param>
        /// <returns>The modified entry.</returns>
        ValueTask<ActivityLog> ModifyActivityLogAsync(ActivityLog activityLog);

        /// <summary>
        /// Removes an activity-log entry after validation.
        /// </summary>
        /// <param name="activityLog">The entry to remove.</param>
        /// <returns>The removed entry.</returns>
        ValueTask<ActivityLog> RemoveActivityLogAsync(ActivityLog activityLog);
    }
}
