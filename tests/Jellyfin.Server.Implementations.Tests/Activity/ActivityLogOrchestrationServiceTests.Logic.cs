using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Enums;
using Jellyfin.Server.Implementations.Activity.Exceptions;
using Jellyfin.Server.Implementations.Activity.Orchestration;
using Jellyfin.Server.Implementations.Activity.Orchestration.Exceptions;
using Jellyfin.Server.Implementations.Activity.Services;
using Jellyfin.Server.Implementations.Users.Exceptions;
using Jellyfin.Server.Implementations.Users.Services;
using MediaBrowser.Model.Activity;
using MediaBrowser.Model.Querying;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Activity
{
    public partial class ActivityLogOrchestrationServiceTests
    {
        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_NullQuery_ThrowsInvalidActivityLogOrchestrationException()
        {
            // given
            var activityLogServiceMock = new Mock<IActivityLogService>();
            var userServiceMock = new Mock<IUserService>();
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogOrchestrationException>(
                () => orchestrationService.RetrieveActivityLogEntriesAsync(null!).AsTask());

            activityLogServiceMock.Verify(
                service => service.RetrieveAllActivityLogsAsync(),
                Times.Never);
            userServiceMock.Verify(service => service.RetrieveAllUsersAsync(), Times.Never);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_NoLogsNoUsers_ReturnsEmptyPage()
        {
            // given
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(CreateNoActivityLogs()));
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(Array.Empty<User>()));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(CreateQuery());

            // then
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalRecordCount);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_LogWithoutMatchingUser_MapsEntryWithUnknownUsername()
        {
            // given
            ActivityLog orphan = CreateActivityLog("orphan", Guid.NewGuid());
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(new[] { orphan }));
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(Array.Empty<User>()));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(CreateQuery());

            // then
            ActivityLogEntry entry = Assert.Single(result.Items);
            Assert.Equal("orphan", entry.Name);
            Assert.Equal(orphan.UserId, entry.UserId);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_UsernameFilter_ReturnsOnlyMatchingUserLogs()
        {
            // given
            ActivityLog aliceLog = CreateActivityLog("alice-log", AliceId);
            ActivityLog bobLog = CreateActivityLog("bob-log", BobId);
            ActivityLog orphanLog = CreateActivityLog("orphan-log", Guid.NewGuid());
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(new[] { aliceLog, bobLog, orphanLog }));
            IReadOnlyList<User> users = new[]
            {
                CreateUser("alice", AliceId),
                CreateUser("bob", BobId)
            };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(users));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(
                    CreateQuery(username: "ALI"));

            // then
            ActivityLogEntry entry = Assert.Single(result.Items);
            Assert.Equal("alice-log", entry.Name);
            Assert.Equal(AliceId, entry.UserId);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_HasUserIdTrue_ReturnsOnlyLogsWithUserId()
        {
            // given
            ActivityLog anonymous = CreateActivityLog("anon", Guid.Empty);
            ActivityLog owned = CreateActivityLog("owned", AliceId);
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(new[] { anonymous, owned }));
            IReadOnlyList<User> users = new[] { CreateUser("alice", AliceId) };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(users));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(
                    CreateQuery(hasUserId: true));

            // then
            ActivityLogEntry entry = Assert.Single(result.Items);
            Assert.Equal("owned", entry.Name);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_HasUserIdFalse_ReturnsOnlyAnonymousLogs()
        {
            // given
            ActivityLog anonymous = CreateActivityLog("anon", Guid.Empty);
            ActivityLog owned = CreateActivityLog("owned", AliceId);
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(new[] { anonymous, owned }));
            IReadOnlyList<User> users = new[] { CreateUser("alice", AliceId) };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(users));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(
                    CreateQuery(hasUserId: false));

            // then
            ActivityLogEntry entry = Assert.Single(result.Items);
            Assert.Equal("anon", entry.Name);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_ColumnFilters_NameOverviewTypeSeverity_DateAndItemId_Apply()
        {
            // given
            Guid itemId = Guid.NewGuid();
            var kept = CreateActivityLog(
                name: "kept-name",
                userId: AliceId,
                overview: "the overview text",
                shortOverview: "shorter",
                itemId: itemId.ToString("N"),
                severity: LogLevel.Warning);
            var droppedByName = CreateActivityLog("other-name", AliceId, overview: "the overview text", severity: LogLevel.Warning);
            var droppedByOverview = CreateActivityLog("kept-name", AliceId, overview: "different", severity: LogLevel.Warning);
            var droppedByType = CreateActivityLog("kept-name", AliceId, "wrong-type", overview: "the overview text", severity: LogLevel.Warning);
            var droppedBySeverity = CreateActivityLog("kept-name", AliceId, overview: "the overview text", severity: LogLevel.Error);
            var droppedByItemId = CreateActivityLog("kept-name", AliceId, overview: "the overview text", severity: LogLevel.Warning, itemId: Guid.NewGuid().ToString("N"));
            var droppedByDate = CreateActivityLog("kept-name", AliceId, dateCreated: new DateTime(2000, 1, 1), overview: "the overview text", severity: LogLevel.Warning);

            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(new[] { kept, droppedByName, droppedByOverview, droppedByType, droppedBySeverity, droppedByItemId, droppedByDate }));
            IReadOnlyList<User> users = new[] { CreateUser("alice", AliceId) };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(users));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(
                    CreateQuery(
                        name: "kept",
                        type: "type-kept",
                        overview: "overview",
                        shortOverview: "short",
                        itemId: itemId,
                        severity: LogLevel.Warning,
                        minDate: new DateTime(2020, 1, 1)));

            // then
            ActivityLogEntry entry = Assert.Single(result.Items);
            Assert.Equal("kept-name", entry.Name);
            Assert.Equal(LogLevel.Warning, entry.Severity);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_DefaultSort_IsByDateCreatedDescending()
        {
            // given
            ActivityLog oldest = CreateActivityLog("old", AliceId, dateCreated: new DateTime(2020, 1, 1));
            ActivityLog newest = CreateActivityLog("new", AliceId, dateCreated: new DateTime(2024, 1, 1));
            ActivityLog middle = CreateActivityLog("mid", AliceId, dateCreated: new DateTime(2022, 1, 1));
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(new[] { oldest, newest, middle }));
            IReadOnlyList<User> users = new[] { CreateUser("alice", AliceId) };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(users));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(CreateQuery());

            // then
            Assert.Equal(new[] { "new", "mid", "old" }, result.Items.Select(e => e.Name).ToArray());
            Assert.Equal(3, result.TotalRecordCount);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_SortByUsernameAscending_UsesUserFoundation()
        {
            // given
            ActivityLog bobLog = CreateActivityLog("zlog", BobId, dateCreated: new DateTime(2024, 1, 1));
            ActivityLog aliceLog = CreateActivityLog("alog", AliceId, dateCreated: new DateTime(2020, 1, 1));
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(new[] { bobLog, aliceLog }));
            IReadOnlyList<User> users = new[]
            {
                CreateUser("bob", BobId),
                CreateUser("alice", AliceId)
            };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(users));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(
                    CreateQuery(orderBy: new[] { (ActivityLogSortBy.Username, SortOrder.Ascending) }));

            // then
            Assert.Equal(new[] { "alog", "zlog" }, result.Items.Select(e => e.Name).ToArray());
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_Pagination_AppliesSkipAndLimitAndTotalCount()
        {
            // given
            ActivityLog[] logs = Enumerable.Range(0, 5)
                .Select(i => CreateActivityLog("log-" + i, AliceId, dateCreated: new DateTime(2020 + i, 1, 1)))
                .ToArray();
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(logs));
            IReadOnlyList<User> users = new[] { CreateUser("alice", AliceId) };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .Returns(new ValueTask<IReadOnlyList<User>>(users));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when (default desc sort: log-4..log-0 ; skip 1 take 2 -> log-3, log-2)
            QueryResult<ActivityLogEntry> result =
                await orchestrationService.RetrieveActivityLogEntriesAsync(
                    CreateQuery(skip: 1, limit: 2));

            // then
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(5, result.TotalRecordCount);
            Assert.Equal(new[] { "log-3", "log-2" }, result.Items.Select(e => e.Name).ToArray());
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_ActivityLogServiceThrows_IsWrappedAsOrchestrationDependencyException()
        {
            // given
            var activityLogServiceMock = new Mock<IActivityLogService>();
            var failing = new FailedActivityLogServiceException(new InvalidOperationException());
            ActivityLogServiceException activityLogServiceException = new ActivityLogServiceException(failing);
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .ThrowsAsync(activityLogServiceException);
            var userServiceMock = new Mock<IUserService>();
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<ActivityLogOrchestrationDependencyException>(
                () => orchestrationService.RetrieveActivityLogEntriesAsync(CreateQuery()).AsTask());

            Assert.Same(activityLogServiceException, thrown.InnerException);
            userServiceMock.Verify(service => service.RetrieveAllUsersAsync(), Times.Never);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_UserServiceThrowsInvalidUser_IsWrappedAsOrchestrationValidationException()
        {
            // given
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(CreateNoActivityLogs()));
            var invalidUserException = new InvalidUserException();
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .ThrowsAsync(invalidUserException);
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<ActivityLogOrchestrationValidationException>(
                () => orchestrationService.RetrieveActivityLogEntriesAsync(CreateQuery()).AsTask());

            Assert.Same(invalidUserException, thrown.InnerException);
        }

        [Fact]
        public async Task RetrieveActivityLogEntriesAsync_UserServiceThrowsUnexpected_IsWrappedAsOrchestrationServiceException()
        {
            // given
            var activityLogServiceMock = new Mock<IActivityLogService>();
            activityLogServiceMock.Setup(service => service.RetrieveAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(CreateNoActivityLogs()));
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(service => service.RetrieveAllUsersAsync())
                .ThrowsAsync(new InvalidOperationException("user foundation down"));
            var orchestrationService = new ActivityLogOrchestrationService(
                activityLogServiceMock.Object,
                userServiceMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<ActivityLogOrchestrationServiceException>(
                () => orchestrationService.RetrieveActivityLogEntriesAsync(CreateQuery()).AsTask());

            Assert.IsType<FailedActivityLogOrchestrationServiceException>(thrown.InnerException);
        }
    }
}
