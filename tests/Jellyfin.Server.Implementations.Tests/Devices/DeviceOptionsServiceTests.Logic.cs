using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Devices.Brokers;
using Jellyfin.Server.Implementations.Devices.Exceptions;
using Jellyfin.Server.Implementations.Devices.Services;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Devices
{
    public partial class DeviceOptionsServiceTests
    {
        [Fact]
        public async Task AddDeviceOptionsAsync_ValidOptions_CallsBrokerInsertAndReturnsSameOptions()
        {
            // given
            DeviceOptions inputOptions = CreateRandomDeviceOptions();
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.InsertDeviceOptionsAsync(It.IsAny<DeviceOptions>()))
                .Returns(new ValueTask<DeviceOptions>(inputOptions));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when
            DeviceOptions actualOptions = await service.AddDeviceOptionsAsync(inputOptions);

            // then
            Assert.Same(inputOptions, actualOptions);
            brokerMock.Verify(b => b.InsertDeviceOptionsAsync(inputOptions), Times.Once);
        }

        [Fact]
        public async Task AddDeviceOptionsAsync_NullOptions_ThrowsInvalidDeviceOptionsException()
        {
            // given
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceOptionsException>(
                () => service.AddDeviceOptionsAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertDeviceOptionsAsync(It.IsAny<DeviceOptions>()), Times.Never);
        }

        [Fact]
        public async Task AddDeviceOptionsAsync_EmptyDeviceId_ThrowsInvalidDeviceOptionsException()
        {
            // given
            DeviceOptions invalidOptions = new DeviceOptions(string.Empty);
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceOptionsException>(
                () => service.AddDeviceOptionsAsync(invalidOptions).AsTask());
            brokerMock.Verify(b => b.InsertDeviceOptionsAsync(It.IsAny<DeviceOptions>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveAllDeviceOptionsAsync_ReturnsAllFromBroker()
        {
            // given
            List<DeviceOptions> expectedOptions = new List<DeviceOptions>
            {
                CreateRandomDeviceOptions(),
                CreateRandomDeviceOptions()
            };
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.SelectAllDeviceOptionsAsync())
                .Returns(new ValueTask<IReadOnlyList<DeviceOptions>>(expectedOptions));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when
            IReadOnlyList<DeviceOptions> actualOptions = await service.RetrieveAllDeviceOptionsAsync();

            // then
            Assert.Equal(expectedOptions, actualOptions);
            brokerMock.Verify(b => b.SelectAllDeviceOptionsAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveDeviceOptionsByIdAsync_InvalidId_ThrowsInvalidDeviceOptionsException()
        {
            // given
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceOptionsException>(
                () => service.RetrieveDeviceOptionsByIdAsync(0).AsTask());
            brokerMock.Verify(b => b.SelectDeviceOptionsByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveDeviceOptionsByIdAsync_NotFound_ThrowsDeviceOptionsNotFoundException()
        {
            // given
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.SelectDeviceOptionsByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<DeviceOptions?>((DeviceOptions?)null));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<DeviceOptionsNotFoundException>(
                () => service.RetrieveDeviceOptionsByIdAsync(GetRandomNumber()).AsTask());
        }

        [Fact]
        public async Task RetrieveDeviceOptionsByIdAsync_Found_ReturnsOptions()
        {
            // given
            DeviceOptions expectedOptions = CreateRandomDeviceOptions();
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.SelectDeviceOptionsByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<DeviceOptions?>(expectedOptions));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when
            DeviceOptions actualOptions = await service.RetrieveDeviceOptionsByIdAsync(GetRandomNumber());

            // then
            Assert.Same(expectedOptions, actualOptions);
        }

        [Fact]
        public async Task RetrieveDeviceOptionsByDeviceIdAsync_EmptyId_ThrowsInvalidDeviceOptionsException()
        {
            // given
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceOptionsException>(
                () => service.RetrieveDeviceOptionsByDeviceIdAsync(string.Empty).AsTask());
            brokerMock.Verify(b => b.SelectDeviceOptionsByDeviceIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveDeviceOptionsByDeviceIdAsync_NotFound_ThrowsDeviceOptionsNotFoundException()
        {
            // given
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.SelectDeviceOptionsByDeviceIdAsync(It.IsAny<string>()))
                .Returns(new ValueTask<DeviceOptions?>((DeviceOptions?)null));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<DeviceOptionsNotFoundException>(
                () => service.RetrieveDeviceOptionsByDeviceIdAsync(CreateRandomString()).AsTask());
        }

        [Fact]
        public async Task RetrieveDeviceOptionsByDeviceIdAsync_Found_ReturnsOptions()
        {
            // given
            DeviceOptions expectedOptions = CreateRandomDeviceOptions();
            string deviceId = CreateRandomString();
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.SelectDeviceOptionsByDeviceIdAsync(deviceId))
                .Returns(new ValueTask<DeviceOptions?>(expectedOptions));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when
            DeviceOptions actualOptions = await service.RetrieveDeviceOptionsByDeviceIdAsync(deviceId);

            // then
            Assert.Same(expectedOptions, actualOptions);
        }

        [Fact]
        public async Task ModifyDeviceOptionsAsync_NullOptions_ThrowsInvalidDeviceOptionsException()
        {
            // given
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceOptionsException>(
                () => service.ModifyDeviceOptionsAsync(null!).AsTask());
            brokerMock.Verify(b => b.UpdateDeviceOptionsAsync(It.IsAny<DeviceOptions>()), Times.Never);
        }

        [Fact]
        public async Task ModifyDeviceOptionsAsync_EmptyDeviceId_ThrowsInvalidDeviceOptionsException()
        {
            // given
            DeviceOptions invalidOptions = new DeviceOptions(string.Empty);
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceOptionsException>(
                () => service.ModifyDeviceOptionsAsync(invalidOptions).AsTask());
            brokerMock.Verify(b => b.UpdateDeviceOptionsAsync(It.IsAny<DeviceOptions>()), Times.Never);
        }

        [Fact]
        public async Task ModifyDeviceOptionsAsync_ValidOptions_CallsBrokerUpdate()
        {
            // given
            DeviceOptions inputOptions = CreateRandomDeviceOptions();
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.UpdateDeviceOptionsAsync(It.IsAny<DeviceOptions>()))
                .Returns(new ValueTask<DeviceOptions>(inputOptions));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when
            DeviceOptions actualOptions = await service.ModifyDeviceOptionsAsync(inputOptions);

            // then
            Assert.Same(inputOptions, actualOptions);
            brokerMock.Verify(b => b.UpdateDeviceOptionsAsync(inputOptions), Times.Once);
        }

        [Fact]
        public async Task RemoveDeviceOptionsAsync_NullOptions_ThrowsInvalidDeviceOptionsException()
        {
            // given
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceOptionsException>(
                () => service.RemoveDeviceOptionsAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteDeviceOptionsAsync(It.IsAny<DeviceOptions>()), Times.Never);
        }

        [Fact]
        public async Task RemoveDeviceOptionsAsync_ValidOptions_CallsBrokerDelete()
        {
            // given
            DeviceOptions inputOptions = CreateRandomDeviceOptions();
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.DeleteDeviceOptionsAsync(It.IsAny<DeviceOptions>()))
                .Returns(new ValueTask<DeviceOptions>(inputOptions));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when
            DeviceOptions actualOptions = await service.RemoveDeviceOptionsAsync(inputOptions);

            // then
            Assert.Same(inputOptions, actualOptions);
            brokerMock.Verify(b => b.DeleteDeviceOptionsAsync(inputOptions), Times.Once);
        }

        [Fact]
        public async Task AddDeviceOptionsAsync_BrokerThrows_IsWrappedAsDeviceOptionsServiceException()
        {
            // given
            DeviceOptions inputOptions = CreateRandomDeviceOptions();
            var brokerMock = new Mock<IDeviceOptionsBroker>();
            brokerMock.Setup(b => b.InsertDeviceOptionsAsync(It.IsAny<DeviceOptions>()))
                .ThrowsAsync(new InvalidOperationException("broker down"));
            var service = new DeviceOptionsService(brokerMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<DeviceOptionsServiceException>(
                () => service.AddDeviceOptionsAsync(inputOptions).AsTask());
            Assert.IsType<FailedDeviceOptionsServiceException>(thrown.InnerException);
            brokerMock.Verify(b => b.InsertDeviceOptionsAsync(inputOptions), Times.Once);
        }
    }
}
