using System;
using System.Globalization;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Devices.Exceptions;

namespace Jellyfin.Server.Implementations.Devices.Services
{
    public partial class DeviceService
    {
        private static void ValidateDeviceOnAdd(Device device)
        {
            ValidateDeviceIsNotNull(device);

            Validate(
                (Rule: IsInvalid(device.UserId), Parameter: nameof(Device.UserId)),
                (Rule: IsInvalid(device.AccessToken), Parameter: nameof(Device.AccessToken)),
                (Rule: IsInvalid(device.AppName), Parameter: nameof(Device.AppName)),
                (Rule: IsInvalid(device.DeviceId), Parameter: nameof(Device.DeviceId)));
        }

        private static void ValidateDeviceOnModify(Device device)
        {
            ValidateDeviceIsNotNull(device);

            // ponytail: persistence Id existence is enforced at the integration layer (Std 2.0.2.0 Do-or-Delegate)
            Validate(
                (Rule: IsInvalid(device.UserId), Parameter: nameof(Device.UserId)),
                (Rule: IsInvalid(device.AccessToken), Parameter: nameof(Device.AccessToken)),
                (Rule: IsInvalid(device.AppName), Parameter: nameof(Device.AppName)),
                (Rule: IsInvalid(device.DeviceId), Parameter: nameof(Device.DeviceId)));
        }

        private static void ValidateDeviceOnRemove(Device device)
        {
            ValidateDeviceIsNotNull(device);
        }

        private static void ValidateDeviceIsNotNull(Device device)
        {
            if (device is null)
            {
                throw new InvalidDeviceException();
            }
        }

        private static void ValidateDeviceById(int deviceId)
        {
            if (deviceId <= 0)
            {
                throw new InvalidDeviceException();
            }
        }

        private static void ValidateDeviceByDeviceId(string deviceId)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
            {
                throw new InvalidDeviceException();
            }
        }

        private static void ValidateDeviceByUserId(Guid userId)
        {
            if (userId.Equals(Guid.Empty))
            {
                throw new InvalidDeviceException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (2.1.3.1.4)
        private static void ValidateDeviceExists(Device? device, string identifier)
        {
            if (device is null)
            {
                throw new DeviceNotFoundException(identifier);
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

        private static dynamic IsInvalid(Guid value) => new
        {
            Condition = value.Equals(Guid.Empty),
            Message = "Value is required."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidDeviceException = new InvalidDeviceException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDeviceException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDeviceException.ThrowIfContainsErrors();
        }
    }
}
