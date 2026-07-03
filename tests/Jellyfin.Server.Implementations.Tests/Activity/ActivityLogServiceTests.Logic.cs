using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Activity.Brokers;
using Jellyfin.Server.Implementations.Activity.Exceptions;
using Jellyfin.Server.Implementations.Activity.Services;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Activity
{
    public partial class ActivityLogServiceTests
    {
        [Fact]
        public async Task AddActivityLogAsync_ValidEntry_CallsBrokerInsertAndReturnsSameEntry()
        {
            // given
            ActivityLog inputActivityLog = CreateRandomActivityLog();
            var brokerMock = new Mock<IActivityLogBroker>();
            brokerMock.Setup(b => b.InsertActivityLogAsync(It.IsAny<ActivityLog>()))
                .Returns(new ValueTask<ActivityLog>(inputActivityLog));
            var service = new ActivityLogService(brokerMock.Object);

            // when
            ActivityLog actualActivityLog = await service.AddActivityLogAsync(inputActivityLog);

            // then
            Assert.Same(inputActivityLog, actualActivityLog);
            brokerMock.Verify(b => b.InsertActivityLogAsync(inputActivityLog), Times.Once);
        }

        [Fact]
        public async Task AddActivityLogAsync_NullEntry_ThrowsInvalidActivityLogException()
        {
            // given
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.AddActivityLogAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task AddActivityLogAsync_EmptyName_ThrowsInvalidActivityLogException()
        {
            // given
            ActivityLog invalidActivityLog = CreateRandomActivityLog();
            invalidActivityLog.Name = string.Empty;
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.AddActivityLogAsync(invalidActivityLog).AsTask());
            brokerMock.Verify(b => b.InsertActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task AddActivityLogAsync_EmptyType_ThrowsInvalidActivityLogException()
        {
            // given
            ActivityLog invalidActivityLog = CreateRandomActivityLog();
            invalidActivityLog.Type = string.Empty;
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.AddActivityLogAsync(invalidActivityLog).AsTask());
            brokerMock.Verify(b => b.InsertActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task AddActivityLogAsync_EmptyUserId_ThrowsInvalidActivityLogException()
        {
            // given
            ActivityLog invalidActivityLog = CreateRandomActivityLog();
            invalidActivityLog.UserId = Guid.Empty;
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.AddActivityLogAsync(invalidActivityLog).AsTask());
            brokerMock.Verify(b => b.InsertActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveAllActivityLogsAsync_ReturnsAllFromBroker()
        {
            // given
            IReadOnlyList<ActivityLog> expectedActivityLogs = CreateRandomActivityLogs();
            var brokerMock = new Mock<IActivityLogBroker>();
            brokerMock.Setup(b => b.SelectAllActivityLogsAsync())
                .Returns(new ValueTask<IReadOnlyList<ActivityLog>>(expectedActivityLogs));
            var service = new ActivityLogService(brokerMock.Object);

            // when
            IReadOnlyList<ActivityLog> actualActivityLogs = await service.RetrieveAllActivityLogsAsync();

            // then
            Assert.Equal(expectedActivityLogs, actualActivityLogs);
            brokerMock.Verify(b => b.SelectAllActivityLogsAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveActivityLogByIdAsync_InvalidId_ThrowsInvalidActivityLogException()
        {
            // given
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.RetrieveActivityLogByIdAsync(0).AsTask());
            brokerMock.Verify(b => b.SelectActivityLogByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveActivityLogByIdAsync_NotFound_ThrowsActivityLogNotFoundException()
        {
            // given
            var brokerMock = new Mock<IActivityLogBroker>();
            brokerMock.Setup(b => b.SelectActivityLogByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<ActivityLog?>((ActivityLog?)null));
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<ActivityLogNotFoundException>(
                () => service.RetrieveActivityLogByIdAsync(GetRandomNumber()).AsTask());
        }

        [Fact]
        public async Task RetrieveActivityLogByIdAsync_Found_ReturnsEntry()
        {
            // given
            ActivityLog expectedActivityLog = CreateRandomActivityLog();
            var brokerMock = new Mock<IActivityLogBroker>();
            brokerMock.Setup(b => b.SelectActivityLogByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<ActivityLog?>(expectedActivityLog));
            var service = new ActivityLogService(brokerMock.Object);

            // when
            ActivityLog actualActivityLog = await service.RetrieveActivityLogByIdAsync(GetRandomNumber());

            // then
            Assert.Same(expectedActivityLog, actualActivityLog);
        }

        [Fact]
        public async Task ModifyActivityLogAsync_NullEntry_ThrowsInvalidActivityLogException()
        {
            // given
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.ModifyActivityLogAsync(null!).AsTask());
            brokerMock.Verify(b => b.UpdateActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task ModifyActivityLogAsync_EmptyName_ThrowsInvalidActivityLogException()
        {
            // given
            ActivityLog invalidActivityLog = CreateRandomActivityLog();
            invalidActivityLog.Name = string.Empty;
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.ModifyActivityLogAsync(invalidActivityLog).AsTask());
            brokerMock.Verify(b => b.UpdateActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task ModifyActivityLogAsync_EmptyType_ThrowsInvalidActivityLogException()
        {
            // given
            ActivityLog invalidActivityLog = CreateRandomActivityLog();
            invalidActivityLog.Type = string.Empty;
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.ModifyActivityLogAsync(invalidActivityLog).AsTask());
            brokerMock.Verify(b => b.UpdateActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task ModifyActivityLogAsync_EmptyUserId_ThrowsInvalidActivityLogException()
        {
            // given
            ActivityLog invalidActivityLog = CreateRandomActivityLog();
            invalidActivityLog.UserId = Guid.Empty;
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.ModifyActivityLogAsync(invalidActivityLog).AsTask());
            brokerMock.Verify(b => b.UpdateActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task ModifyActivityLogAsync_ValidEntry_CallsBrokerUpdate()
        {
            // given
            ActivityLog inputActivityLog = CreateRandomActivityLog();
            var brokerMock = new Mock<IActivityLogBroker>();
            brokerMock.Setup(b => b.UpdateActivityLogAsync(It.IsAny<ActivityLog>()))
                .Returns(new ValueTask<ActivityLog>(inputActivityLog));
            var service = new ActivityLogService(brokerMock.Object);

            // when
            ActivityLog actualActivityLog = await service.ModifyActivityLogAsync(inputActivityLog);

            // then
            Assert.Same(inputActivityLog, actualActivityLog);
            brokerMock.Verify(b => b.UpdateActivityLogAsync(inputActivityLog), Times.Once);
        }

        [Fact]
        public async Task RemoveActivityLogAsync_NullEntry_ThrowsInvalidActivityLogException()
        {
            // given
            var brokerMock = new Mock<IActivityLogBroker>();
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidActivityLogException>(
                () => service.RemoveActivityLogAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteActivityLogAsync(It.IsAny<ActivityLog>()), Times.Never);
        }

        [Fact]
        public async Task RemoveActivityLogAsync_ValidEntry_CallsBrokerDelete()
        {
            // given
            ActivityLog inputActivityLog = CreateRandomActivityLog();
            var brokerMock = new Mock<IActivityLogBroker>();
            brokerMock.Setup(b => b.DeleteActivityLogAsync(It.IsAny<ActivityLog>()))
                .Returns(new ValueTask<ActivityLog>(inputActivityLog));
            var service = new ActivityLogService(brokerMock.Object);

            // when
            ActivityLog actualActivityLog = await service.RemoveActivityLogAsync(inputActivityLog);

            // then
            Assert.Same(inputActivityLog, actualActivityLog);
            brokerMock.Verify(b => b.DeleteActivityLogAsync(inputActivityLog), Times.Once);
        }

        [Fact]
        public async Task AddActivityLogAsync_BrokerThrows_IsWrappedAsActivityLogServiceException()
        {
            // given
            ActivityLog inputActivityLog = CreateRandomActivityLog();
            var brokerMock = new Mock<IActivityLogBroker>();
            brokerMock.Setup(b => b.InsertActivityLogAsync(It.IsAny<ActivityLog>()))
                .ThrowsAsync(new InvalidOperationException("broker down"));
            var service = new ActivityLogService(brokerMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<ActivityLogServiceException>(
                () => service.AddActivityLogAsync(inputActivityLog).AsTask());
            Assert.IsType<FailedActivityLogServiceException>(thrown.InnerException);
            brokerMock.Verify(b => b.InsertActivityLogAsync(inputActivityLog), Times.Once);
        }
    }
}
