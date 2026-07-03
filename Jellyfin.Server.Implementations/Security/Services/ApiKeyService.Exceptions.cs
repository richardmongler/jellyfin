using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Security.Services
{
    public partial class ApiKeyService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        private static T TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return returningValueTaskFunction().AsTask().GetAwaiter().GetResult();
            }
            catch (InvalidApiKeyException invalidApiKeyException)
            {
                throw invalidApiKeyException;
            }
            catch (ApiKeyNotFoundException apiKeyNotFoundException)
            {
                throw apiKeyNotFoundException;
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
