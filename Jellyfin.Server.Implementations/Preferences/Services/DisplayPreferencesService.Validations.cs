using System;
using System.Globalization;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    public partial class DisplayPreferencesService
    {
        // ponytail: logical rules intentionally limited to UserId (!= Empty) and Client (non-whitespace).
        // ItemId is a free Guid coordinate and may be Guid.Empty for client-default preferences, so it is
        // NOT validated (mirrors the legacy manager which constructs DisplayPreferences with any ItemId).
        private static void ValidateDisplayPreferencesOnAdd(DisplayPreferences displayPreferences)
        {
            ValidateDisplayPreferencesIsNotNull(displayPreferences);

            Validate(
                (Rule: IsInvalid(displayPreferences.UserId), Parameter: nameof(DisplayPreferences.UserId)),
                (Rule: IsInvalid(displayPreferences.Client), Parameter: nameof(DisplayPreferences.Client)));
        }

        private static void ValidateDisplayPreferencesOnModify(DisplayPreferences displayPreferences)
        {
            ValidateDisplayPreferencesIsNotNull(displayPreferences);

            // ponytail: persistence Id existence is enforced at the integration layer (Std 2.0.2.0 Do-or-Delegate)
            Validate(
                (Rule: IsInvalid(displayPreferences.UserId), Parameter: nameof(DisplayPreferences.UserId)),
                (Rule: IsInvalid(displayPreferences.Client), Parameter: nameof(DisplayPreferences.Client)));
        }

        private static void ValidateDisplayPreferencesOnRemove(DisplayPreferences displayPreferences)
        {
            ValidateDisplayPreferencesIsNotNull(displayPreferences);
        }

        private static void ValidateDisplayPreferencesIsNotNull(DisplayPreferences displayPreferences)
        {
            if (displayPreferences is null)
            {
                throw new InvalidDisplayPreferencesException();
            }
        }

        private static void ValidateDisplayPreferencesById(int displayPreferencesId)
        {
            if (displayPreferencesId <= 0)
            {
                throw new InvalidDisplayPreferencesException();
            }
        }

        private static void ValidateDisplayPreferencesByUserItemClient(Guid userId, string client)
        {
            if (userId.Equals(Guid.Empty) || string.IsNullOrWhiteSpace(client))
            {
                throw new InvalidDisplayPreferencesException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (2.1.3.1.4)
        private static void ValidateDisplayPreferencesExists(DisplayPreferences? displayPreferences, string identifier)
        {
            if (displayPreferences is null)
            {
                throw new DisplayPreferencesNotFoundException(identifier);
            }
        }

        private static dynamic IsInvalid(Guid value) => new
        {
            Condition = value.Equals(Guid.Empty),
            Message = "Value is required."
        };

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Value is required."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidDisplayPreferencesException = new InvalidDisplayPreferencesException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDisplayPreferencesException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDisplayPreferencesException.ThrowIfContainsErrors();
        }
    }
}
