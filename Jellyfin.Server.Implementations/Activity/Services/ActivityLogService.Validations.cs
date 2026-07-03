using System;
using System.Globalization;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Activity.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Services
{
    public partial class ActivityLogService
    {
        private static void ValidateActivityLogOnAdd(ActivityLog activityLog)
        {
            ValidateActivityLogIsNotNull(activityLog);

            Validate(
                (Rule: IsInvalid(activityLog.Name), Parameter: nameof(ActivityLog.Name)),
                (Rule: IsInvalid(activityLog.Type), Parameter: nameof(ActivityLog.Type)),
                (Rule: IsInvalid(activityLog.UserId), Parameter: nameof(ActivityLog.UserId)));
        }

        private static void ValidateActivityLogOnModify(ActivityLog activityLog)
        {
            ValidateActivityLogIsNotNull(activityLog);

            // ponytail: persistence Id existence is enforced at the integration layer (Std 2.0.2.0 Do-or-Delegate)
            Validate(
                (Rule: IsInvalid(activityLog.Name), Parameter: nameof(ActivityLog.Name)),
                (Rule: IsInvalid(activityLog.Type), Parameter: nameof(ActivityLog.Type)),
                (Rule: IsInvalid(activityLog.UserId), Parameter: nameof(ActivityLog.UserId)));
        }

        private static void ValidateActivityLogOnRemove(ActivityLog activityLog)
        {
            ValidateActivityLogIsNotNull(activityLog);
        }

        private static void ValidateActivityLogIsNotNull(ActivityLog activityLog)
        {
            if (activityLog is null)
            {
                throw new InvalidActivityLogException();
            }
        }

        private static void ValidateActivityLogById(int activityLogId)
        {
            if (activityLogId <= 0)
            {
                throw new InvalidActivityLogException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (2.1.3.1.4)
        private static void ValidateActivityLogExists(ActivityLog? activityLog, string identifier)
        {
            if (activityLog is null)
            {
                throw new ActivityLogNotFoundException(identifier);
            }
        }

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
            var invalidActivityLogException = new InvalidActivityLogException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidActivityLogException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidActivityLogException.ThrowIfContainsErrors();
        }
    }
}
