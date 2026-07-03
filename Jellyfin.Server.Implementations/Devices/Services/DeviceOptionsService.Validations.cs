using System;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Devices.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Services
{
    public partial class DeviceOptionsService
    {
        private static void ValidateDeviceOptionsOnAdd(DeviceOptions deviceOptions)
        {
            ValidateDeviceOptionsIsNotNull(deviceOptions);

            Validate(
                (Rule: IsInvalid(deviceOptions.DeviceId), Parameter: nameof(DeviceOptions.DeviceId)));
        }

        private static void ValidateDeviceOptionsOnModify(DeviceOptions deviceOptions)
        {
            ValidateDeviceOptionsIsNotNull(deviceOptions);

            // ponytail: persistence Id existence is enforced at the integration layer (Std 2.0.2.0 Do-or-Delegate)
            Validate(
                (Rule: IsInvalid(deviceOptions.DeviceId), Parameter: nameof(DeviceOptions.DeviceId)));
        }

        private static void ValidateDeviceOptionsOnRemove(DeviceOptions deviceOptions)
        {
            ValidateDeviceOptionsIsNotNull(deviceOptions);
        }

        private static void ValidateDeviceOptionsIsNotNull(DeviceOptions deviceOptions)
        {
            if (deviceOptions is null)
            {
                throw new InvalidDeviceOptionsException();
            }
        }

        private static void ValidateDeviceOptionsById(int deviceOptionsId)
        {
            if (deviceOptionsId <= 0)
            {
                throw new InvalidDeviceOptionsException();
            }
        }

        private static void ValidateDeviceOptionsByDeviceId(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new InvalidDeviceOptionsException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (2.1.3.1.4)
        private static void ValidateDeviceOptionsExists(DeviceOptions? deviceOptions, string identifier)
        {
            if (deviceOptions is null)
            {
                throw new DeviceOptionsNotFoundException(identifier);
            }
        }

        private static dynamic IsInvalid(int value) => new
        {
            Condition = value <= 0,
            Message = "Value is required."
        };

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Value is required."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidDeviceOptionsException = new InvalidDeviceOptionsException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDeviceOptionsException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDeviceOptionsException.ThrowIfContainsErrors();
        }
    }
}
