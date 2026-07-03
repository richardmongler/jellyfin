using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Security.Services
{
    public partial class ApiKeyService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        // ponytail: The-Standard TryCatch cancels exception noise at the Foundation layer
        private static async ValueTask<T> TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidApiKeyException)
            {
                throw;
            }
            catch (ApiKeyNotFoundException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedApiKeyServiceException =
                    new FailedApiKeyServiceException(exception);

                throw new ApiKeyServiceException(failedApiKeyServiceException);
            }
        }
    }
}
