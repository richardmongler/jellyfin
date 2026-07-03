using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Activity.Brokers;

namespace Jellyfin.Server.Implementations.Activity.Services
{
    /// <summary>
    /// Foundation service neighboring the <see cref="IActivityLogBroker"/>; validates,
    /// integrates, and cancels exception noise for activity-log operations (The-Standard 2.1).
    /// </summary>
    public partial class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogBroker activityLogBroker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogService"/> class.
        /// </summary>
        /// <param name="activityLogBroker">The neighboring activity-log broker.</param>
        public ActivityLogService(IActivityLogBroker activityLogBroker) =>
            this.activityLogBroker = activityLogBroker;

        /// <inheritdoc/>
        public ValueTask<ActivityLog> AddActivityLogAsync(ActivityLog activityLog) =>
            TryCatch(async () =>
            {
                ValidateActivityLogOnAdd(activityLog);

                return await this.activityLogBroker.InsertActivityLogAsync(activityLog)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<IReadOnlyList<ActivityLog>> RetrieveAllActivityLogsAsync() =>
            TryCatch(async () =>
            {
                return await this.activityLogBroker.SelectAllActivityLogsAsync()
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<ActivityLog> RetrieveActivityLogByIdAsync(int activityLogId) =>
            TryCatch(async () =>
            {
                ValidateActivityLogById(activityLogId);

                ActivityLog? activityLog = await this.activityLogBroker
                    .SelectActivityLogByIdAsync(activityLogId)
                    .ConfigureAwait(false);

                ValidateActivityLogExists(
                    activityLog,
                    activityLogId.ToString(System.Globalization.CultureInfo.InvariantCulture));

                return activityLog!;
            });

        /// <inheritdoc/>
        public ValueTask<ActivityLog> ModifyActivityLogAsync(ActivityLog activityLog) =>
            TryCatch(async () =>
            {
                ValidateActivityLogOnModify(activityLog);

                return await this.activityLogBroker.UpdateActivityLogAsync(activityLog)
                    .ConfigureAwait(false);
            });

        /// <inheritdoc/>
        public ValueTask<ActivityLog> RemoveActivityLogAsync(ActivityLog activityLog) =>
            TryCatch(async () =>
            {
                ValidateActivityLogOnRemove(activityLog);

                return await this.activityLogBroker.DeleteActivityLogAsync(activityLog)
                    .ConfigureAwait(false);
            });
    }
}
