using System;
using System.Globalization;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Users.Exceptions;

namespace Jellyfin.Server.Implementations.Users.Services
{
    public partial class UserService
    {
        private static void ValidateUserOnAdd(User user)
        {
            ValidateUserIsNotNull(user);

            Validate(
                (Rule: IsInvalid(user.Username), Parameter: nameof(User.Username)),
                (Rule: IsInvalid(user.AuthenticationProviderId), Parameter: nameof(User.AuthenticationProviderId)),
                (Rule: IsInvalid(user.PasswordResetProviderId), Parameter: nameof(User.PasswordResetProviderId)),
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)));
        }

        private static void ValidateUserOnModify(User user)
        {
            ValidateUserIsNotNull(user);

            // ponytail: persistence Id existence is enforced at the integration layer (Std 2.0.2.0 Do-or-Delegate)
            Validate(
                (Rule: IsInvalid(user.Username), Parameter: nameof(User.Username)),
                (Rule: IsInvalid(user.AuthenticationProviderId), Parameter: nameof(User.AuthenticationProviderId)),
                (Rule: IsInvalid(user.PasswordResetProviderId), Parameter: nameof(User.PasswordResetProviderId)),
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)));
        }

        private static void ValidateUserOnRemove(User user)
        {
            ValidateUserIsNotNull(user);
        }

        private static void ValidateUserIsNotNull(User user)
        {
            if (user is null)
            {
                throw new InvalidUserException();
            }
        }

        private static void ValidateUserById(Guid userId)
        {
            if (userId.Equals(Guid.Empty))
            {
                throw new InvalidUserException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (2.1.3.1.4)
        private static void ValidateUserExists(User? user, string identifier)
        {
            if (user is null)
            {
                throw new UserNotFoundException(identifier);
            }
        }

        // ponytail: NormalizedUsername is derived from Username in the User constructor, so it is
        // intentionally NOT validated separately — a valid Username guarantees a valid NormalizedUsername.
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
            var invalidUserException = new InvalidUserException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserException.ThrowIfContainsErrors();
        }
    }
}
