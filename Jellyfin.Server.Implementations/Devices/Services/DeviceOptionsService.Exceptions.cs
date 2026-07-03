using System;
using System.Threading.Tasks;
using Jellyfin.Server.Implementations.Devices.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Services
{
    public partial class DeviceOptionsService
    {
        private delegate ValueTask<T> ReturningValueTaskFunction<T>();

        // ponytail: The-Standard TryCatch cancels exception noise at the Foundation layer
        private static async ValueTask<T> TryCatch<T>(ReturningValueTaskFunction<T> returningValueTaskFunction)
        {
            try
            {
                return await returningValueTaskFunction().ConfigureAwait(false);
            }
            catch (InvalidDeviceOptionsException)
            {
                throw;
            }
            catch (DeviceOptionsNotFoundException)
            {
                throw;
            }
            catch (Exception exception)
            {
                var failedDeviceOptionsServiceException =
                    new FailedDeviceOptionsServiceException(exception);

                throw new DeviceOptionsServiceException(failedDeviceOptionsServiceException);
            }
        }
    }
}
