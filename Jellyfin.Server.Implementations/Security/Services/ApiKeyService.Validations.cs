using System;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Security.Services
{
    public partial class ApiKeyService
    {
        private static void ValidateApiKeyOnAdd(ApiKey apiKey)
        {
            ValidateApiKeyIsNotNull(apiKey);

            Validate(
                (Rule: IsInvalid(apiKey.Name), Parameter: nameof(ApiKey.Name)),
                (Rule: IsInvalid(apiKey.AccessToken), Parameter: nameof(ApiKey.AccessToken)));
        }

        private static void ValidateApiKeyOnRemove(ApiKey apiKey)
        {
            ValidateApiKeyIsNotNull(apiKey);
        }

        private static void ValidateApiKeyIsNotNull(ApiKey apiKey)
        {
            if (apiKey is null)
            {
                throw new InvalidApiKeyException();
            }
        }

        private static void ValidateApiKeyById(int apiKeyId)
        {
            if (apiKeyId <= 0)
            {
                throw new InvalidApiKeyException();
            }
        }

        private static void ValidateAccessToken(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidApiKeyException();
            }
        }

        // ponytail: external/dependency existence validation per The-Standard (4.1.3.1.4/1.5)
        private static void ValidateApiKeyExists(ApiKey? apiKey, string identifier)
        {
            if (apiKey is null)
            {
                throw new ApiKeyNotFoundException(identifier);
            }
        }

        private static dynamic IsInvalid(string value) => new
        {
            Condition = string.IsNullOrWhiteSpace(value),
            Message = "Value is required."
        };

        private static dynamic IsInvalid(int value) => new
        {
            Condition = value <= 0,
            Message = "Value is required."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidApiKeyException = new InvalidApiKeyException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidApiKeyException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidApiKeyException.ThrowIfContainsErrors();
        }
    }
}
