using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Jellyfin.Server.Implementations.Security.Exceptions
{
    // ponytail: Xeption package won't resolve on net10.0, inline minimal equivalent

    /// <summary>
    /// Base exception supporting aggregated validation data, modeled on the Xeption contract.
    /// Name intentionally omits the "Exception" suffix to match The-Standard's Xeption convention.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "The-Standard reserves the Xeption name for its base exception contract.")]
    public class Xeption : Exception
    {
        private readonly Dictionary<string, List<string>> errorData = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Xeption"/> class.
        /// </summary>
        public Xeption()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Xeption"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public Xeption(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Xeption"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The originating exception.</param>
        public Xeption(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Appends a validation message to the list associated with the given parameter key.
        /// </summary>
        /// <param name="key">The parameter name.</param>
        /// <param name="value">The validation message.</param>
        public void UpsertDataList(string key, string value)
        {
            if (!this.errorData.TryGetValue(key, out var list))
            {
                list = new List<string>();
                this.errorData[key] = list;
            }

            list.Add(value);
        }

        /// <summary>
        /// Determines whether any aggregated validation messages exist.
        /// </summary>
        /// <returns><c>true</c> if validation messages are present; otherwise <c>false</c>.</returns>
        public bool ContainsErrors() => this.errorData.Count > 0;

        /// <summary>
        /// Throws this instance when aggregated validation messages are present.
        /// </summary>
        public void ThrowIfContainsErrors()
        {
            if (ContainsErrors())
            {
                throw this;
            }
        }
    }
}
