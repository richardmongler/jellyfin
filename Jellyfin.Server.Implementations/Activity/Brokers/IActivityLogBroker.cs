using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;

namespace Jellyfin.Server.Implementations.Activity.Brokers
{
    /// <summary>
    /// Provides integration operations against the activity-log resource (no flow control).
    /// </summary>
    public interface IActivityLogBroker
    {
        /// <summary>
        /// Inserts an activity-log entry asynchronously.
        /// </summary>
        /// <param name="activityLog">The entry to insert.</param>
        /// <returns>The inserted entry.</returns>
        ValueTask<ActivityLog> InsertActivityLogAsync(ActivityLog activityLog);

        /// <summary>
        /// Selects all activity-log entries asynchronously.
        /// </summary>
        /// <returns>Every persisted entry.</returns>
        ValueTask<IReadOnlyList<ActivityLog>> SelectAllActivityLogsAsync();

        /// <summary>
        /// Selects an activity-log entry by its persistence identifier asynchronously.
        /// </summary>
        /// <param name="activityLogId">The entry persistence identifier.</param>
        /// <returns>The matching entry, or <c>null</c> when absent.</returns>
        ValueTask<ActivityLog?> SelectActivityLogByIdAsync(int activityLogId);

        /// <summary>
        /// Updates an activity-log entry asynchronously.
        /// </summary>
        /// <param name="activityLog">The entry to update.</param>
        /// <returns>The updated entry.</returns>
        ValueTask<ActivityLog> UpdateActivityLogAsync(ActivityLog activityLog);

        /// <summary>
        /// Deletes an activity-log entry asynchronously.
        /// </summary>
        /// <param name="activityLog">The entry to delete.</param>
        /// <returns>The deleted entry.</returns>
        ValueTask<ActivityLog> DeleteActivityLogAsync(ActivityLog activityLog);
    }
}
