using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Activity.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Services
{
    public partial class ActivityLogService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        // ponytail: The-Standard TryCatch cancels exception noise at the Foundation layer
        private static async ValueTask<T> TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidActivityLogException)
            {
                throw;
            }
            catch (ActivityLogNotFoundException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedActivityLogServiceException =
                    new FailedActivityLogServiceException(exception);

                throw new ActivityLogServiceException(failedActivityLogServiceException);
            }
        }
    }
}
