using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Exceptions
{
    /// <summary>
    /// Validation exception thrown when an activity-log entry fails structural or logical validation.
    /// </summary>
    public class InvalidActivityLogException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidActivityLogException"/> class.
        /// </summary>
        public InvalidActivityLogException()
            : base(message: "Invalid activity-log error(s) occurred, fix the errors and try again.")
        {
        }
    }
}
