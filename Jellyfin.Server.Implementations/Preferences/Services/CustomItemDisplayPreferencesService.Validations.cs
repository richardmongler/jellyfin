using System;
using System.Globalization;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    public partial class CustomItemDisplayPreferencesService
    {
        // ponytail: logical rules intentionally limited to UserId (!= Empty), Client (non-whitespace), and
        // Key (non-whitespace). ItemId is a free Guid coordinate and may be Guid.Empty (the legacy manager
        // treats Guid.Empty as the "no item" sentinel), so it is NOT validated, mirroring the
        // DisplayPreferences and ItemDisplayPreferences Foundations in this domain. Value is a nullable
        // string (`string?`) and is intentionally NOT validated — no business rule for it.
        private static void ValidateCustomItemDisplayPreferencesOnAdd(CustomItemDisplayPreferences customItemDisplayPreferences)
        {
            ValidateCustomItemDisplayPreferencesIsNotNull(customItemDisplayPreferences);

            Validate(
                (Rule: IsInvalid(customItemDisplayPreferences.UserId), Parameter: nameof(CustomItemDisplayPreferences.UserId)),
                (Rule: IsInvalid(customItemDisplayPreferences.Client), Parameter: nameof(CustomItemDisplayPreferences.Client)),
                (Rule: IsInvalid(customItemDisplayPreferences.Key), Parameter: nameof(CustomItemDisplayPreferences.Key)));
        }

        private static void ValidateCustomItemDisplayPreferencesOnModify(CustomItemDisplayPreferences customItemDisplayPreferences)
        {
            ValidateCustomItemDisplayPreferencesIsNotNull(customItemDisplayPreferences);

            // ponytail: persistence Id existence is enforced at the integration layer (Std 2.0.2.0 Do-or-Delegate)
            Validate(
                (Rule: IsInvalid(customItemDisplayPreferences.UserId), Parameter: nameof(CustomItemDisplayPreferences.UserId)),
                (Rule: IsInvalid(customItemDisplayPreferences.Client), Parameter: nameof(CustomItemDisplayPreferences.Client)),
                (Rule: IsInvalid(customItemDisplayPreferences.Key), Parameter: nameof(CustomItemDisplayPreferences.Key)));
        }

        private static void ValidateCustomItemDisplayPreferencesOnRemove(CustomItemDisplayPreferences customItemDisplayPreferences)
        {
            ValidateCustomItemDisplayPreferencesIsNotNull(customItemDisplayPreferences);
        }

        private static void ValidateCustomItemDisplayPreferencesIsNotNull(CustomItemDisplayPreferences customItemDisplayPreferences)
        {
            if (customItemDisplayPreferences is null)
            {
                throw new InvalidCustomItemDisplayPreferencesException();
            }
        }

        private static void ValidateCustomItemDisplayPreferencesById(int customItemDisplayPreferencesId)
        {
            if (customItemDisplayPreferencesId <= 0)
            {
                throw new InvalidCustomItemDisplayPreferencesException();
            }
        }

        private static void ValidateCustomItemDisplayPreferencesByUserItemClient(Guid userId, string client)
        {
            if (userId.Equals(Guid.Empty) || string.IsNullOrWhiteSpace(client))
            {
                throw new InvalidCustomItemDisplayPreferencesException();
            }
        }

        private static void ValidateCustomItemDisplayPreferencesByUserItemClientKey(Guid userId, string client, string key)
        {
            if (userId.Equals(Guid.Empty) || string.IsNullOrWhiteSpace(client) || string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidCustomItemDisplayPreferencesException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (2.1.3.1.4)
        private static void ValidateCustomItemDisplayPreferencesExists(
            CustomItemDisplayPreferences? customItemDisplayPreferences, string identifier)
        {
            if (customItemDisplayPreferences is null)
            {
                throw new CustomItemDisplayPreferencesNotFoundException(identifier);
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
            var invalidCustomItemDisplayPreferencesException = new InvalidCustomItemDisplayPreferencesException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCustomItemDisplayPreferencesException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCustomItemDisplayPreferencesException.ThrowIfContainsErrors();
        }
    }
}
