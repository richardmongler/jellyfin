using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Preferences.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    public partial class CustomItemDisplayPreferencesService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        // ponytail: The-Standard TryCatch cancels exception noise at the Foundation layer
        private static async ValueTask<T> TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidCustomItemDisplayPreferencesException)
            {
                throw;
            }
            catch (CustomItemDisplayPreferencesNotFoundException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedCustomItemDisplayPreferencesServiceException =
                    new FailedCustomItemDisplayPreferencesServiceException(exception);

                throw new CustomItemDisplayPreferencesServiceException(failedCustomItemDisplayPreferencesServiceException);
            }
        }
    }
}
