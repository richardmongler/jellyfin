using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Preferences.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    public partial class ItemDisplayPreferencesService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        // ponytail: The-Standard TryCatch cancels exception noise at the Foundation layer
        private static async ValueTask<T> TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidItemDisplayPreferencesException)
            {
                throw;
            }
            catch (ItemDisplayPreferencesNotFoundException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedItemDisplayPreferencesServiceException =
                    new FailedItemDisplayPreferencesServiceException(exception);

                throw new ItemDisplayPreferencesServiceException(failedItemDisplayPreferencesServiceException);
            }
        }
    }
}
