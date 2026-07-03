using System.Threading.Tasks;
using Jellyfin.Data.Queries;
using Jellyfin.Server.Implementations.Activity.Services;
using Jellyfin.Server.Implementations.Users.Services;
using MediaBrowser.Model.Activity;
using MediaBrowser.Model.Querying;

namespace Jellyfin.Server.Implementations.Activity.Orchestration
{
    /// <summary>
    /// Combines the ActivityLog and User Foundation services to resolve the username of an
    /// activity-log paged query (The-Standard 2.3, Two-Three Florance over two Foundations).
    /// </summary>
    public interface IActivityLogOrchestrationService
    {
        /// <summary>
        /// Retrieves a paged list of activity-log entries, resolving the optional username
        /// filter/sort against the User Foundation before paging (The-Standard 2.3.3.0 Flow Combination).
        /// </summary>
        /// <param name="query">The activity-log query.</param>
        /// <returns>The paged activity-log entries.</returns>
        ValueTask<QueryResult<ActivityLogEntry>> RetrieveActivityLogEntriesAsync(ActivityLogQuery query);
    }
}
