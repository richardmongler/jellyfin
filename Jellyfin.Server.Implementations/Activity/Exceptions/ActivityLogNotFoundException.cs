using Jellyfin.Server.Implementations.Security.Exceptions;

namespace Jellyfin.Server.Implementations.Activity.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested activity-log entry cannot be located.
    /// </summary>
    public class ActivityLogNotFoundException : Xeption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogNotFoundException"/> class.
        /// </summary>
        /// <param name="identifier">The identifier that failed to match an activity-log entry.</param>
        public ActivityLogNotFoundException(string identifier)
            : base(message: $"ActivityLog with identifier '{identifier}' was not found.")
        {
        }
    }
}
