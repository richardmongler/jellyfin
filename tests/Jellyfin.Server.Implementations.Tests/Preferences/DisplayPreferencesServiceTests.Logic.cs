using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Server.Implementations.Preferences.Brokers;
using Jellyfin.Server.Implementations.Preferences.Exceptions;
using Jellyfin.Server.Implementations.Preferences.Services;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Preferences
{
    public partial class DisplayPreferencesServiceTests
    {
        [Fact]
        public async Task AddDisplayPreferencesAsync_ValidPreferences_CallsBrokerInsertAndReturnsSame()
        {
            // given
            DisplayPreferences inputPreferences = CreateRandomDisplayPreferences();
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()))
                .Returns(new ValueTask<DisplayPreferences>(inputPreferences));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when
            DisplayPreferences actualPreferences = await service.AddDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.InsertDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task AddDisplayPreferencesAsync_NullPreferences_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.AddDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task AddDisplayPreferencesAsync_EmptyUserId_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            DisplayPreferences invalidPreferences = new DisplayPreferences(
                userId: Guid.Empty,
                itemId: Guid.NewGuid(),
                client: CreateRandomString());
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.AddDisplayPreferencesAsync(invalidPreferences).AsTask());
            brokerMock.Verify(b => b.InsertDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task AddDisplayPreferencesAsync_EmptyClient_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            DisplayPreferences invalidPreferences = CreateRandomDisplayPreferences();
            invalidPreferences.Client = string.Empty;
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.AddDisplayPreferencesAsync(invalidPreferences).AsTask());
            brokerMock.Verify(b => b.InsertDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveAllDisplayPreferencesAsync_ReturnsAllFromBroker()
        {
            // given
            IReadOnlyList<DisplayPreferences> expectedPreferences = CreateRandomDisplayPreferencesList();
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectAllDisplayPreferencesAsync())
                .Returns(new ValueTask<IReadOnlyList<DisplayPreferences>>(expectedPreferences));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when
            IReadOnlyList<DisplayPreferences> actualPreferences = await service.RetrieveAllDisplayPreferencesAsync();

            // then
            Assert.Equal(expectedPreferences, actualPreferences);
            brokerMock.Verify(b => b.SelectAllDisplayPreferencesAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveDisplayPreferencesByIdAsync_InvalidId_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.RetrieveDisplayPreferencesByIdAsync(0).AsTask());
            brokerMock.Verify(b => b.SelectDisplayPreferencesByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveDisplayPreferencesByIdAsync_NotFound_ThrowsDisplayPreferencesNotFoundException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectDisplayPreferencesByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<DisplayPreferences?>((DisplayPreferences?)null));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<DisplayPreferencesNotFoundException>(
                () => service.RetrieveDisplayPreferencesByIdAsync(GetRandomNumber()).AsTask());
        }

        [Fact]
        public async Task RetrieveDisplayPreferencesByIdAsync_Found_ReturnsPreferences()
        {
            // given
            DisplayPreferences expectedPreferences = CreateRandomDisplayPreferences();
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectDisplayPreferencesByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<DisplayPreferences?>(expectedPreferences));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when
            DisplayPreferences actualPreferences = await service.RetrieveDisplayPreferencesByIdAsync(GetRandomNumber());

            // then
            Assert.Same(expectedPreferences, actualPreferences);
        }

        [Fact]
        public async Task RetrieveDisplayPreferencesByUserItemClientAsync_EmptyUserId_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.RetrieveDisplayPreferencesByUserItemClientAsync(Guid.Empty, Guid.NewGuid(), CreateRandomString()).AsTask());
            brokerMock.Verify(
                b => b.SelectDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveDisplayPreferencesByUserItemClientAsync_EmptyClient_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.RetrieveDisplayPreferencesByUserItemClientAsync(Guid.NewGuid(), Guid.NewGuid(), string.Empty).AsTask());
            brokerMock.Verify(
                b => b.SelectDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveDisplayPreferencesByUserItemClientAsync_NotFound_ThrowsDisplayPreferencesNotFoundException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new ValueTask<DisplayPreferences?>((DisplayPreferences?)null));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<DisplayPreferencesNotFoundException>(
                () => service.RetrieveDisplayPreferencesByUserItemClientAsync(Guid.NewGuid(), Guid.NewGuid(), CreateRandomString()).AsTask());
        }

        [Fact]
        public async Task RetrieveDisplayPreferencesByUserItemClientAsync_Found_ReturnsPreferences()
        {
            // given
            DisplayPreferences expectedPreferences = CreateRandomDisplayPreferences();
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            string client = CreateRandomString();
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectDisplayPreferencesByUserItemClientAsync(userId, itemId, client))
                .Returns(new ValueTask<DisplayPreferences?>(expectedPreferences));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when
            DisplayPreferences actualPreferences = await service.RetrieveDisplayPreferencesByUserItemClientAsync(userId, itemId, client);

            // then
            Assert.Same(expectedPreferences, actualPreferences);
        }

        [Fact]
        public async Task ModifyDisplayPreferencesAsync_NullPreferences_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.ModifyDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.UpdateDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task ModifyDisplayPreferencesAsync_ValidPreferences_CallsBrokerUpdate()
        {
            // given
            DisplayPreferences inputPreferences = CreateRandomDisplayPreferences();
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.UpdateDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()))
                .Returns(new ValueTask<DisplayPreferences>(inputPreferences));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when
            DisplayPreferences actualPreferences = await service.ModifyDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.UpdateDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task RemoveDisplayPreferencesAsync_NullPreferences_ThrowsInvalidDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidDisplayPreferencesException>(
                () => service.RemoveDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task RemoveDisplayPreferencesAsync_ValidPreferences_CallsBrokerDelete()
        {
            // given
            DisplayPreferences inputPreferences = CreateRandomDisplayPreferences();
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.DeleteDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()))
                .Returns(new ValueTask<DisplayPreferences>(inputPreferences));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when
            DisplayPreferences actualPreferences = await service.RemoveDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.DeleteDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task AddDisplayPreferencesAsync_BrokerThrows_IsWrappedAsDisplayPreferencesServiceException()
        {
            // given
            DisplayPreferences inputPreferences = CreateRandomDisplayPreferences();
            var brokerMock = new Mock<IDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertDisplayPreferencesAsync(It.IsAny<DisplayPreferences>()))
                .ThrowsAsync(new InvalidOperationException("broker down"));
            var service = new DisplayPreferencesService(brokerMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<DisplayPreferencesServiceException>(
                () => service.AddDisplayPreferencesAsync(inputPreferences).AsTask());
            Assert.IsType<FailedDisplayPreferencesServiceException>(thrown.InnerException);
            brokerMock.Verify(b => b.InsertDisplayPreferencesAsync(inputPreferences), Times.Once);
        }
    }
}
