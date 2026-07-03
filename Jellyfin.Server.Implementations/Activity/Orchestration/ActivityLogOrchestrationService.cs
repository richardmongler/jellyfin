using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Data.Enums;
using Jellyfin.Data.Queries;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Server.Implementations.Activity.Services;
using Jellyfin.Server.Implementations.Users.Services;
using MediaBrowser.Model.Activity;
using MediaBrowser.Model.Querying;

namespace Jellyfin.Server.Implementations.Activity.Orchestration
{
    /// <summary>
    /// Combines the ActivityLog and User Foundation services to resolve the username of an
    /// activity-log paged query. Primary entity ActivityLog; supporting entity User resolves
    /// the Username column (The-Standard 2.3, Two-Three Florance over two Foundations).
    /// </summary>
    public partial class ActivityLogOrchestrationService : IActivityLogOrchestrationService
    {
        private readonly IActivityLogService activityLogService;
        private readonly IUserService userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogOrchestrationService"/> class.
        /// </summary>
        /// <param name="activityLogService">The ActivityLog Foundation service.</param>
        /// <param name="userService">The User Foundation service.</param>
        public ActivityLogOrchestrationService(
            IActivityLogService activityLogService,
            IUserService userService)
        {
            this.activityLogService = activityLogService;
            this.userService = userService;
        }

        /// <inheritdoc/>
        public ValueTask<QueryResult<ActivityLogEntry>> RetrieveActivityLogEntriesAsync(ActivityLogQuery query) =>
            TryCatch(async () =>
            {
                ValidateActivityLogQuery(query);

                IReadOnlyList<ActivityLog> activityLogs =
                    await this.activityLogService.RetrieveAllActivityLogsAsync()
                        .ConfigureAwait(false);

                IReadOnlyList<User> users =
                    await this.userService.RetrieveAllUsersAsync()
                        .ConfigureAwait(false);

                // ponytail: in-memory filter/sort/page over a bounded admin-scoped log table.
                // The-Standard 2.3.3.0 Flow Combination joins ActivityLogs with users here so each
                // Foundation broker stays single-resource (Std 1.2.8). Upgrade path: a paged broker
                // query when throughput demands it; the username pre-filter/sort is the only consumer.
                var usernameByUserId = users.ToDictionary(user => user.Id, user => user.Username);

                IEnumerable<ExpandedEntry> entries = activityLogs
                    .Select(activityLog => new ExpandedEntry
                    {
                        ActivityLog = activityLog,
                        Username = usernameByUserId.TryGetValue(activityLog.UserId, out var username) ? username : null
                    });

                entries = ApplyFilters(entries, query);
                IOrderedEnumerable<ExpandedEntry> ordered = ApplyOrdering(entries, query.OrderBy);
                IReadOnlyList<ExpandedEntry> materialized = ordered.ToList();

                int totalCount = materialized.Count;
                int skip = query.Skip ?? 0;
                int limit = query.Limit ?? 100;

                IReadOnlyList<ActivityLogEntry> page = materialized
                    .Skip(skip)
                    .Take(limit)
                    .Select(ToActivityLogEntry)
                    .ToList();

                return new QueryResult<ActivityLogEntry>(query.Skip, totalCount, page);
            });

        private static IEnumerable<ExpandedEntry> ApplyFilters(
            IEnumerable<ExpandedEntry> entries,
            ActivityLogQuery query)
        {
            if (query.HasUserId is not null)
            {
                entries = entries.Where(e => e.ActivityLog.UserId.Equals(default) != query.HasUserId.Value);
            }

            if (query.MinDate is not null)
            {
                entries = entries.Where(e => e.ActivityLog.DateCreated >= query.MinDate.Value);
            }

            if (query.MaxDate is not null)
            {
                entries = entries.Where(e => e.ActivityLog.DateCreated <= query.MaxDate.Value);
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                entries = entries.Where(e => e.ActivityLog.Name != null && Contains(e.ActivityLog.Name, query.Name));
            }

            if (!string.IsNullOrEmpty(query.Overview))
            {
                entries = entries.Where(e => e.ActivityLog.Overview != null && Contains(e.ActivityLog.Overview, query.Overview));
            }

            if (!string.IsNullOrEmpty(query.ShortOverview))
            {
                entries = entries.Where(e => e.ActivityLog.ShortOverview != null && Contains(e.ActivityLog.ShortOverview, query.ShortOverview));
            }

            if (!string.IsNullOrEmpty(query.Type))
            {
                entries = entries.Where(e => Contains(e.ActivityLog.Type, query.Type));
            }

            if (query.ItemId is not null && !query.ItemId.Value.Equals(Guid.Empty))
            {
                var itemId = query.ItemId.Value.ToString("N");
                entries = entries.Where(e => e.ActivityLog.ItemId == itemId);
            }

            if (!string.IsNullOrEmpty(query.Username))
            {
                entries = entries.Where(e => e.Username != null && Contains(e.Username, query.Username));
            }

            if (query.Severity is not null)
            {
                entries = entries.Where(e => e.ActivityLog.LogSeverity == query.Severity);
            }

            return entries;
        }

        private static IOrderedEnumerable<ExpandedEntry> ApplyOrdering(
            IEnumerable<ExpandedEntry> entries,
            IReadOnlyCollection<(ActivityLogSortBy, SortOrder)>? sorting)
        {
            if (sorting is null || sorting.Count == 0)
            {
                return entries.OrderByDescending(e => e.ActivityLog.DateCreated);
            }

            IOrderedEnumerable<ExpandedEntry>? ordered = null;

            foreach (var (sortBy, sortOrder) in sorting)
            {
                bool ascending = sortOrder == SortOrder.Ascending;

                if (ordered is null)
                {
                    ordered = sortBy switch
                    {
                        ActivityLogSortBy.Name => ascending ? entries.OrderBy(e => e.ActivityLog.Name) : entries.OrderByDescending(e => e.ActivityLog.Name),
                        ActivityLogSortBy.Overiew => ascending ? entries.OrderBy(e => e.ActivityLog.Overview) : entries.OrderByDescending(e => e.ActivityLog.Overview),
                        ActivityLogSortBy.ShortOverview => ascending ? entries.OrderBy(e => e.ActivityLog.ShortOverview) : entries.OrderByDescending(e => e.ActivityLog.ShortOverview),
                        ActivityLogSortBy.Type => ascending ? entries.OrderBy(e => e.ActivityLog.Type) : entries.OrderByDescending(e => e.ActivityLog.Type),
                        ActivityLogSortBy.DateCreated => ascending ? entries.OrderBy(e => e.ActivityLog.DateCreated) : entries.OrderByDescending(e => e.ActivityLog.DateCreated),
                        ActivityLogSortBy.Username => ascending ? entries.OrderBy(e => e.Username) : entries.OrderByDescending(e => e.Username),
                        ActivityLogSortBy.LogSeverity => ascending ? entries.OrderBy(e => e.ActivityLog.LogSeverity) : entries.OrderByDescending(e => e.ActivityLog.LogSeverity),
                        _ => throw new ArgumentOutOfRangeException(nameof(sorting), sortBy, "Unhandled ActivityLogSortBy")
                    };
                }
                else
                {
                    ordered = sortBy switch
                    {
                        ActivityLogSortBy.Name => ascending ? ordered.ThenBy(e => e.ActivityLog.Name) : ordered.ThenByDescending(e => e.ActivityLog.Name),
                        ActivityLogSortBy.Overiew => ascending ? ordered.ThenBy(e => e.ActivityLog.Overview) : ordered.ThenByDescending(e => e.ActivityLog.Overview),
                        ActivityLogSortBy.ShortOverview => ascending ? ordered.ThenBy(e => e.ActivityLog.ShortOverview) : ordered.ThenByDescending(e => e.ActivityLog.ShortOverview),
                        ActivityLogSortBy.Type => ascending ? ordered.ThenBy(e => e.ActivityLog.Type) : ordered.ThenByDescending(e => e.ActivityLog.Type),
                        ActivityLogSortBy.DateCreated => ascending ? ordered.ThenBy(e => e.ActivityLog.DateCreated) : ordered.ThenByDescending(e => e.ActivityLog.DateCreated),
                        ActivityLogSortBy.Username => ascending ? ordered.ThenBy(e => e.Username) : ordered.ThenByDescending(e => e.Username),
                        ActivityLogSortBy.LogSeverity => ascending ? ordered.ThenBy(e => e.ActivityLog.LogSeverity) : ordered.ThenByDescending(e => e.ActivityLog.LogSeverity),
                        _ => throw new ArgumentOutOfRangeException(nameof(sorting), sortBy, "Unhandled ActivityLogSortBy")
                    };
                }
            }

            return ordered!;
        }

        // ponytail: in-memory LIKE (%X%) mirror of the original EF.Functions.Like; SQLite default
        // LIKE is case-insensitive for ASCII, so OrdinalIgnoreCase preserves the original semantics.
        private static bool Contains(string? value, string fragment) =>
            value is not null && value.Contains(fragment, StringComparison.OrdinalIgnoreCase);

        private static ActivityLogEntry ToActivityLogEntry(ExpandedEntry entry)
        {
            var activityLog = entry.ActivityLog;

            return new ActivityLogEntry(activityLog.Name, activityLog.Type, activityLog.UserId)
            {
                Id = activityLog.Id,
                Overview = activityLog.Overview,
                ShortOverview = activityLog.ShortOverview,
                ItemId = activityLog.ItemId,
                Date = activityLog.DateCreated,
                Severity = activityLog.LogSeverity
            };
        }

        private sealed class ExpandedEntry
        {
            public ActivityLog ActivityLog { get; set; } = null!;

            public string? Username { get; set; }
        }
    }
}
