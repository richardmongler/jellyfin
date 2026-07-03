using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Activity.Exceptions;
using Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions;
using Jellyfin.Server.Implementations.Standard.Exceptions;
using Jellyfin.Server.Implementations.Users.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Orchestration
{
    public partial class ActivityLogOrchestrationService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        // ponytail: The-Standard TryCatch unwraps downstream categorical exceptions (validation and
        // dependency) and re-wraps them into orchestration-layer categorical exceptions (Std 2.3.3.0.2).
        private static async ValueTask<T> TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidActivityLogOrchestrationException)
            {
                throw;
            }
            catch (InvalidActivityLogException invalidActivityLogException)
            {
                throw new ActivityLogOrchestrationValidationException(invalidActivityLogException);
            }
            catch (ActivityLogNotFoundException activityLogNotFoundException)
            {
                throw new ActivityLogOrchestrationValidationException(activityLogNotFoundException);
            }
            catch (InvalidUserException invalidUserException)
            {
                throw new ActivityLogOrchestrationValidationException(invalidUserException);
            }
            catch (UserNotFoundException userNotFoundException)
            {
                throw new ActivityLogOrchestrationValidationException(userNotFoundException);
            }
            catch (ActivityLogServiceException activityLogServiceException)
            {
                throw new ActivityLogOrchestrationDependencyException(activityLogServiceException);
            }
            catch (UserServiceException userServiceException)
            {
                throw new ActivityLogOrchestrationDependencyException(userServiceException);
            }
            catch (Exception exception)
            {
                var failedActivityLogOrchestrationServiceException =
                    new FailedActivityLogOrchestrationServiceException(exception);

                throw new ActivityLogOrchestrationServiceException(failedActivityLogOrchestrationServiceException);
            }
        }
    }
}
