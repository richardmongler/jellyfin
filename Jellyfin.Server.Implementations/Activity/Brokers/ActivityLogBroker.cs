using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations;
using Jellyfin.Database.Implementations.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jellyfin.Server.Implementations.Activity.Brokers
{
    /// <summary>
    /// Entity broker integrating the activity-log resource with EF Core. Owns no flow control.
    /// </summary>
    public partial class ActivityLogBroker : IActivityLogBroker
    {
        private readonly IDbContextFactory<JellyfinDbContext> dbContextFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogBroker"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The EF Core context factory.</param>
        public ActivityLogBroker(IDbContextFactory<JellyfinDbContext> dbContextFactory) =>
            this.dbContextFactory = dbContextFactory;

        /// <inheritdoc/>
        public async ValueTask<ActivityLog> InsertActivityLogAsync(ActivityLog activityLog)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ActivityLogs.Add(activityLog);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return activityLog;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<ActivityLog>> SelectAllActivityLogsAsync()
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ActivityLogs
                .OrderBy(activityLog => activityLog.Id)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<ActivityLog?> SelectActivityLogByIdAsync(int activityLogId)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            return await dbContext.ActivityLogs
                .FindAsync(activityLogId)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async ValueTask<ActivityLog> UpdateActivityLogAsync(ActivityLog activityLog)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ActivityLogs.Update(activityLog);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return activityLog;
        }

        /// <inheritdoc/>
        public async ValueTask<ActivityLog> DeleteActivityLogAsync(ActivityLog activityLog)
        {
            using var dbContext = await this.dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);

            dbContext.ActivityLogs.Remove(activityLog);
            await dbContext.SaveChangesAsync().ConfigureAwait(false);

            return activityLog;
        }
    }
}
