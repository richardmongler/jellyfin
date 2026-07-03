using System;
using System.Collections.Generic;
using Jellyfin.Data.Enums;
using Jellyfin.Data.Queries;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Server.Implementations.Tests.Activity
{
    public partial class ActivityLogOrchestrationServiceTests
    {
        private static readonly Guid AliceId = Guid.NewGuid();
        private static readonly Guid BobId = Guid.NewGuid();

        private static ActivityLog CreateActivityLog(
            string name,
            Guid userId,
            string? type = null,
            DateTime? dateCreated = null,
            string? overview = null,
            string? shortOverview = null,
            string? itemId = null,
            LogLevel? severity = null) =>
            new ActivityLog(name, type ?? ("type-" + name), userId)
            {
                DateCreated = dateCreated ?? DateTime.UtcNow,
                Overview = overview,
                ShortOverview = shortOverview,
                ItemId = itemId,
                LogSeverity = severity ?? LogLevel.Information
            };

        private static User CreateUser(string username, Guid? id = null, string providerId = "prov")
        {
            var user = new User(username, providerId, providerId)
            {
                Id = id ?? Guid.NewGuid()
            };

            return user;
        }

        private static ActivityLogQuery CreateQuery(
            int? skip = null,
            int? limit = null,
            string? username = null,
            bool? hasUserId = null,
            DateTime? minDate = null,
            DateTime? maxDate = null,
            string? name = null,
            string? type = null,
            string? overview = null,
            string? shortOverview = null,
            Guid? itemId = null,
            LogLevel? severity = null,
            IReadOnlyCollection<(ActivityLogSortBy, SortOrder)>? orderBy = null) =>
            new ActivityLogQuery
            {
                Skip = skip,
                Limit = limit,
                Username = username,
                HasUserId = hasUserId,
                MinDate = minDate,
                MaxDate = maxDate,
                Name = name,
                Type = type,
                Overview = overview,
                ShortOverview = shortOverview,
                ItemId = itemId,
                Severity = severity,
                OrderBy = orderBy
            };

        private static IReadOnlyList<ActivityLog> CreateNoActivityLogs() =>
            Array.Empty<ActivityLog>();
    }
}
