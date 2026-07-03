using Jellyfin.Data.Queries;
using Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Orchestration
{
    public partial class ActivityLogOrchestrationService
    {
        // ponytail: structural validation of the orchestration request before any downstream call.
        private static void ValidateActivityLogQuery(ActivityLogQuery query)
        {
            if (query is null)
            {
                throw new InvalidActivityLogOrchestrationException();
            }
        }
    }
}
