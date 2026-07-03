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
    public partial class DeviceServiceTests
    {
        [Fact]
        public async Task AddDeviceAsync_ValidDevice_CallsBrokerInsertAndReturnsSameDevice()
        {
            // given
            Device inputDevice = CreateRandomDevice();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.InsertDeviceAsync(It.IsAny<Device>()))
                .Returns(new ValueTask<Device>(inputDevice));
            var service = new DeviceService(brokerMock.Object);

            // when
            Device actualDevice = await service.AddDeviceAsync(inputDevice);

            // then
            Assert.Same(inputDevice, actualDevice);
            brokerMock.Verify(b => b.InsertDeviceAsync(inputDevice), Times.Once);
        }

        [Fact]
        public async Task AddDeviceAsync_NullDevice_ThrowsInvalidDeviceException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.AddDeviceAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertDeviceAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task AddDeviceAsync_EmptyUserId_ThrowsInvalidDeviceException()
        {
            // given
            Device invalidDevice = new Device(
                userId: Guid.Empty,
                appName: CreateRandomString(),
                appVersion: CreateRandomString(),
                deviceName: CreateRandomString(),
                deviceId: CreateRandomString());
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.AddDeviceAsync(invalidDevice).AsTask());
            brokerMock.Verify(b => b.InsertDeviceAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task AddDeviceAsync_EmptyAccessToken_ThrowsInvalidDeviceException()
        {
            // given
            Device invalidDevice = CreateRandomDevice();
            invalidDevice.AccessToken = string.Empty;
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.AddDeviceAsync(invalidDevice).AsTask());
            brokerMock.Verify(b => b.InsertDeviceAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task AddDeviceAsync_EmptyAppName_ThrowsInvalidDeviceException()
        {
            // given
            Device invalidDevice = CreateRandomDevice();
            invalidDevice.AppName = string.Empty;
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.AddDeviceAsync(invalidDevice).AsTask());
            brokerMock.Verify(b => b.InsertDeviceAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task AddDeviceAsync_EmptyDeviceId_ThrowsInvalidDeviceException()
        {
            // given
            Device invalidDevice = CreateRandomDevice();
            invalidDevice.DeviceId = string.Empty;
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.AddDeviceAsync(invalidDevice).AsTask());
            brokerMock.Verify(b => b.InsertDeviceAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveAllDevicesAsync_ReturnsAllFromBroker()
        {
            // given
            IReadOnlyList<Device> expectedDevices = CreateRandomDevices();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.SelectAllDevicesAsync())
                .Returns(new ValueTask<IReadOnlyList<Device>>(expectedDevices));
            var service = new DeviceService(brokerMock.Object);

            // when
            IReadOnlyList<Device> actualDevices = await service.RetrieveAllDevicesAsync();

            // then
            Assert.Equal(expectedDevices, actualDevices);
            brokerMock.Verify(b => b.SelectAllDevicesAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveDeviceByIdAsync_InvalidId_ThrowsInvalidDeviceException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.RetrieveDeviceByIdAsync(0).AsTask());
            brokerMock.Verify(b => b.SelectDeviceByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveDeviceByIdAsync_NotFound_ThrowsDeviceNotFoundException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.SelectDeviceByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<Device?>((Device?)null));
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<DeviceNotFoundException>(
                () => service.RetrieveDeviceByIdAsync(GetRandomNumber()).AsTask());
        }

        [Fact]
        public async Task RetrieveDeviceByIdAsync_Found_ReturnsDevice()
        {
            // given
            Device expectedDevice = CreateRandomDevice();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.SelectDeviceByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<Device?>(expectedDevice));
            var service = new DeviceService(brokerMock.Object);

            // when
            Device actualDevice = await service.RetrieveDeviceByIdAsync(GetRandomNumber());

            // then
            Assert.Same(expectedDevice, actualDevice);
        }

        [Fact]
        public async Task RetrieveDeviceByDeviceIdAsync_EmptyId_ThrowsInvalidDeviceException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.RetrieveDeviceByDeviceIdAsync(string.Empty).AsTask());
            brokerMock.Verify(b => b.SelectDeviceByDeviceIdAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveDeviceByDeviceIdAsync_NotFound_ThrowsDeviceNotFoundException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.SelectDeviceByDeviceIdAsync(It.IsAny<string>()))
                .Returns(new ValueTask<Device?>((Device?)null));
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<DeviceNotFoundException>(
                () => service.RetrieveDeviceByDeviceIdAsync(CreateRandomString()).AsTask());
        }

        [Fact]
        public async Task RetrieveDeviceByDeviceIdAsync_Found_ReturnsDevice()
        {
            // given
            Device expectedDevice = CreateRandomDevice();
            string deviceId = CreateRandomString();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.SelectDeviceByDeviceIdAsync(deviceId))
                .Returns(new ValueTask<Device?>(expectedDevice));
            var service = new DeviceService(brokerMock.Object);

            // when
            Device actualDevice = await service.RetrieveDeviceByDeviceIdAsync(deviceId);

            // then
            Assert.Same(expectedDevice, actualDevice);
        }

        [Fact]
        public async Task RetrieveDevicesByUserIdAsync_EmptyUserId_ThrowsInvalidDeviceException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.RetrieveDevicesByUserIdAsync(Guid.Empty).AsTask());
            brokerMock.Verify(b => b.SelectDevicesByUserIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveDevicesByUserIdAsync_ValidUserId_ReturnsFromBroker()
        {
            // given
            IReadOnlyList<Device> expectedDevices = CreateRandomDevices();
            var userId = Guid.NewGuid();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.SelectDevicesByUserIdAsync(userId))
                .Returns(new ValueTask<IReadOnlyList<Device>>(expectedDevices));
            var service = new DeviceService(brokerMock.Object);

            // when
            IReadOnlyList<Device> actualDevices = await service.RetrieveDevicesByUserIdAsync(userId);

            // then
            Assert.Same(expectedDevices, actualDevices);
            brokerMock.Verify(b => b.SelectDevicesByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task ModifyDeviceAsync_NullDevice_ThrowsInvalidDeviceException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.ModifyDeviceAsync(null!).AsTask());
            brokerMock.Verify(b => b.UpdateDeviceAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task ModifyDeviceAsync_ValidDevice_CallsBrokerUpdate()
        {
            // given
            Device inputDevice = CreateRandomDevice();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.UpdateDeviceAsync(It.IsAny<Device>()))
                .Returns(new ValueTask<Device>(inputDevice));
            var service = new DeviceService(brokerMock.Object);

            // when
            Device actualDevice = await service.ModifyDeviceAsync(inputDevice);

            // then
            Assert.Same(inputDevice, actualDevice);
            brokerMock.Verify(b => b.UpdateDeviceAsync(inputDevice), Times.Once);
        }

        [Fact]
        public async Task RemoveDeviceAsync_NullDevice_ThrowsInvalidDeviceException()
        {
            // given
            var brokerMock = new Mock<IDeviceBroker>();
            var service = new DeviceService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDeviceException>(
                () => service.RemoveDeviceAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteDeviceAsync(It.IsAny<Device>()), Times.Never);
        }

        [Fact]
        public async Task RemoveDeviceAsync_ValidDevice_CallsBrokerDelete()
        {
            // given
            Device inputDevice = CreateRandomDevice();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.DeleteDeviceAsync(It.IsAny<Device>()))
                .Returns(new ValueTask<Device>(inputDevice));
            var service = new DeviceService(brokerMock.Object);

            // when
            Device actualDevice = await service.RemoveDeviceAsync(inputDevice);

            // then
            Assert.Same(inputDevice, actualDevice);
            brokerMock.Verify(b => b.DeleteDeviceAsync(inputDevice), Times.Once);
        }

        [Fact]
        public async Task AddDeviceAsync_BrokerThrows_IsWrappedAsDeviceServiceException()
        {
            // given
            Device inputDevice = CreateRandomDevice();
            var brokerMock = new Mock<IDeviceBroker>();
            brokerMock.Setup(b => b.InsertDeviceAsync(It.IsAny<Device>()))
                .ThrowsAsync(new InvalidOperationException("broker down"));
            var service = new DeviceService(brokerMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<DeviceServiceException>(
                () => service.AddDeviceAsync(inputDevice).AsTask());
            Assert.IsType<FailedDeviceServiceException>(thrown.InnerException);
            brokerMock.Verify(b => b.InsertDeviceAsync(inputDevice), Times.Once);
        }
    }
}
