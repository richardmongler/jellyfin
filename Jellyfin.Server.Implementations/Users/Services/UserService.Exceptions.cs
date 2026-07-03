using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Users.Exceptions;

namespace Jellyfin.Server.Implementations.Users.Services
{
    public partial class UserService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        // ponytail: The-Standard TryCatch cancels exception noise at the Foundation layer
        private static async ValueTask<T> TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidUserException)
            {
                throw;
            }
            catch (UserNotFoundException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedUserServiceException =
                    new FailedUserServiceException(exception);

                throw new UserServiceException(failedUserServiceException);
            }
        }
    }
}
