using System;
using System.Globalization;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Exceptions;

namespace Jellyfin.Server.Implementations.Preferences.Services
{
    public partial class ItemDisplayPreferencesService
    {
        // ponytail: logical rules intentionally limited to UserId (!= Empty) and Client (non-whitespace).
        // ItemId is a free Guid coordinate and may be Guid.Empty — the legacy manager treats Guid.Empty
        // as the "no item" sentinel (GetItemDisplayPreferences creates with ItemId=Guid.Empty;
        // ListItemDisplayPreferences excludes default ItemId), so it is NOT validated, mirroring
        // the DisplayPreferences Foundation in this domain.
        private static void ValidateItemDisplayPreferencesOnAdd(ItemDisplayPreferences itemDisplayPreferences)
        {
            ValidateItemDisplayPreferencesIsNotNull(itemDisplayPreferences);

            Validate(
                (Rule: IsInvalid(itemDisplayPreferences.UserId), Parameter: nameof(ItemDisplayPreferences.UserId)),
                (Rule: IsInvalid(itemDisplayPreferences.Client), Parameter: nameof(ItemDisplayPreferences.Client)));
        }

        private static void ValidateItemDisplayPreferencesOnModify(ItemDisplayPreferences itemDisplayPreferences)
        {
            ValidateItemDisplayPreferencesIsNotNull(itemDisplayPreferences);

            // ponytail: persistence Id existence is enforced at the integration layer (Std 2.0.2.0 Do-or-Delegate)
            Validate(
                (Rule: IsInvalid(itemDisplayPreferences.UserId), Parameter: nameof(ItemDisplayPreferences.UserId)),
                (Rule: IsInvalid(itemDisplayPreferences.Client), Parameter: nameof(ItemDisplayPreferences.Client)));
        }

        private static void ValidateItemDisplayPreferencesOnRemove(ItemDisplayPreferences itemDisplayPreferences)
        {
            ValidateItemDisplayPreferencesIsNotNull(itemDisplayPreferences);
        }

        private static void ValidateItemDisplayPreferencesIsNotNull(ItemDisplayPreferences itemDisplayPreferences)
        {
            if (itemDisplayPreferences is null)
            {
                throw new InvalidItemDisplayPreferencesException();
            }
        }

        private static void ValidateItemDisplayPreferencesById(int itemDisplayPreferencesId)
        {
            if (itemDisplayPreferencesId <= 0)
            {
                throw new InvalidItemDisplayPreferencesException();
            }
        }

        private static void ValidateItemDisplayPreferencesByUserItemClient(Guid userId, string client)
        {
            if (userId.Equals(Guid.Empty) || string.IsNullOrWhiteSpace(client))
            {
                throw new InvalidItemDisplayPreferencesException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (2.1.3.1.4)
        private static void ValidateItemDisplayPreferencesExists(ItemDisplayPreferences? itemDisplayPreferences, string identifier)
        {
            if (itemDisplayPreferences is null)
            {
                throw new ItemDisplayPreferencesNotFoundException(identifier);
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
            var invalidItemDisplayPreferencesException = new InvalidItemDisplayPreferencesException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidItemDisplayPreferencesException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidItemDisplayPreferencesException.ThrowIfContainsErrors();
        }
    }
}
