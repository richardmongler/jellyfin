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
    public partial class CustomItemDisplayPreferencesServiceTests
    {
        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_ValidPreferences_CallsBrokerInsertAndReturnsSame()
        {
            // given
            CustomItemDisplayPreferences inputPreferences = CreateRandomCustomItemDisplayPreferences();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences>(inputPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            CustomItemDisplayPreferences actualPreferences = await service.AddCustomItemDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_NullPreferences_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.AddCustomItemDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_EmptyUserId_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            CustomItemDisplayPreferences invalidPreferences = new CustomItemDisplayPreferences(
                userId: Guid.Empty,
                itemId: Guid.NewGuid(),
                client: CreateRandomString(),
                key: CreateRandomString(),
                value: CreateRandomString());
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.AddCustomItemDisplayPreferencesAsync(invalidPreferences).AsTask());
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_EmptyClient_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            CustomItemDisplayPreferences invalidPreferences = CreateRandomCustomItemDisplayPreferences();
            invalidPreferences.Client = string.Empty;
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.AddCustomItemDisplayPreferencesAsync(invalidPreferences).AsTask());
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_EmptyKey_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            CustomItemDisplayPreferences invalidPreferences = CreateRandomCustomItemDisplayPreferences();
            invalidPreferences.Key = string.Empty;
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.AddCustomItemDisplayPreferencesAsync(invalidPreferences).AsTask());
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()), Times.Never);
        }

        // ItemId is a free Guid coordinate (Guid.Empty is the legacy "no item" sentinel) and is intentionally
        // NOT validated — a Guid.Empty ItemId must pass the Foundation the same as any other value.
        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_EmptyItemIdSentinel_PassesValidationAndReachesBroker()
        {
            // given
            CustomItemDisplayPreferences sentinelPreferences = new CustomItemDisplayPreferences(
                userId: Guid.NewGuid(),
                itemId: Guid.Empty,
                client: CreateRandomString(),
                key: CreateRandomString(),
                value: CreateRandomString());
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences>(sentinelPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            CustomItemDisplayPreferences actualPreferences = await service.AddCustomItemDisplayPreferencesAsync(sentinelPreferences);

            // then
            Assert.Same(sentinelPreferences, actualPreferences);
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(sentinelPreferences), Times.Once);
        }

        // Value is a nullable string (`string?`) and is intentionally NOT validated — null is a valid preference value
        // (the legacy manager persists arbitrary key/value custom prefs); it must pass the Foundation and reach the broker.
        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_NullValue_PassesValidationAndReachesBroker()
        {
            // given
            CustomItemDisplayPreferences nullValuePreferences = new CustomItemDisplayPreferences(
                userId: Guid.NewGuid(),
                itemId: Guid.NewGuid(),
                client: CreateRandomString(),
                key: CreateRandomString(),
                value: null);
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences>(nullValuePreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            CustomItemDisplayPreferences actualPreferences = await service.AddCustomItemDisplayPreferencesAsync(nullValuePreferences);

            // then
            Assert.Same(nullValuePreferences, actualPreferences);
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(nullValuePreferences), Times.Once);
        }

        [Fact]
        public async Task AddCustomItemDisplayPreferencesAsync_BrokerThrows_IsWrappedAsCustomItemDisplayPreferencesServiceException()
        {
            // given
            CustomItemDisplayPreferences inputPreferences = CreateRandomCustomItemDisplayPreferences();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()))
                .ThrowsAsync(new InvalidOperationException("broker down"));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<CustomItemDisplayPreferencesServiceException>(
                () => service.AddCustomItemDisplayPreferencesAsync(inputPreferences).AsTask());
            Assert.IsType<FailedCustomItemDisplayPreferencesServiceException>(thrown.InnerException);
            brokerMock.Verify(b => b.InsertCustomItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task RetrieveAllCustomItemDisplayPreferencesAsync_ReturnsAllFromBroker()
        {
            // given
            IReadOnlyList<CustomItemDisplayPreferences> expectedPreferences = CreateRandomCustomItemDisplayPreferencesList();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectAllCustomItemDisplayPreferencesAsync())
                .Returns(new ValueTask<IReadOnlyList<CustomItemDisplayPreferences>>(expectedPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            IReadOnlyList<CustomItemDisplayPreferences> actualPreferences = await service.RetrieveAllCustomItemDisplayPreferencesAsync();

            // then
            Assert.Equal(expectedPreferences, actualPreferences);
            brokerMock.Verify(b => b.SelectAllCustomItemDisplayPreferencesAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByIdAsync_InvalidId_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.RetrieveCustomItemDisplayPreferencesByIdAsync(0).AsTask());
            brokerMock.Verify(b => b.SelectCustomItemDisplayPreferencesByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByIdAsync_NotFound_ThrowsCustomItemDisplayPreferencesNotFoundException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectCustomItemDisplayPreferencesByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences?>((CustomItemDisplayPreferences?)null));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<CustomItemDisplayPreferencesNotFoundException>(
                () => service.RetrieveCustomItemDisplayPreferencesByIdAsync(GetRandomNumber()).AsTask());
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByIdAsync_Found_ReturnsPreferences()
        {
            // given
            CustomItemDisplayPreferences expectedPreferences = CreateRandomCustomItemDisplayPreferences();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectCustomItemDisplayPreferencesByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences?>(expectedPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            CustomItemDisplayPreferences actualPreferences = await service.RetrieveCustomItemDisplayPreferencesByIdAsync(GetRandomNumber());

            // then
            Assert.Same(expectedPreferences, actualPreferences);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientAsync_EmptyUserId_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.RetrieveCustomItemDisplayPreferencesByUserItemClientAsync(Guid.Empty, Guid.NewGuid(), CreateRandomString()).AsTask());
            brokerMock.Verify(
                b => b.SelectCustomItemDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientAsync_EmptyClient_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.RetrieveCustomItemDisplayPreferencesByUserItemClientAsync(Guid.NewGuid(), Guid.NewGuid(), string.Empty).AsTask());
            brokerMock.Verify(
                b => b.SelectCustomItemDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        // A multi-row list retrieve: an empty result is a valid "no custom prefs set" state, so NO NotFound fires.
        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientAsync_EmptyListPassesThroughAndReachesBroker()
        {
            // given
            IReadOnlyList<CustomItemDisplayPreferences> emptyList = Array.Empty<CustomItemDisplayPreferences>();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectCustomItemDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new ValueTask<IReadOnlyList<CustomItemDisplayPreferences>>(emptyList));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            IReadOnlyList<CustomItemDisplayPreferences> actualPreferences = await service
                .RetrieveCustomItemDisplayPreferencesByUserItemClientAsync(Guid.NewGuid(), Guid.NewGuid(), CreateRandomString());

            // then
            Assert.Empty(actualPreferences);
            brokerMock.Verify(
                b => b.SelectCustomItemDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientAsync_FoundList_ReturnsList()
        {
            // given
            IReadOnlyList<CustomItemDisplayPreferences> expectedPreferences = CreateRandomCustomItemDisplayPreferencesList();
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            string client = CreateRandomString();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectCustomItemDisplayPreferencesByUserItemClientAsync(userId, itemId, client))
                .Returns(new ValueTask<IReadOnlyList<CustomItemDisplayPreferences>>(expectedPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            IReadOnlyList<CustomItemDisplayPreferences> actualPreferences = await service
                .RetrieveCustomItemDisplayPreferencesByUserItemClientAsync(userId, itemId, client);

            // then
            Assert.Equal(expectedPreferences, actualPreferences);
            brokerMock.Verify(b => b.SelectCustomItemDisplayPreferencesByUserItemClientAsync(userId, itemId, client), Times.Once);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync_EmptyUserId_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync(Guid.Empty, Guid.NewGuid(), CreateRandomString(), CreateRandomString()).AsTask());
            brokerMock.Verify(
                b => b.SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync_EmptyClient_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync(Guid.NewGuid(), Guid.NewGuid(), string.Empty, CreateRandomString()).AsTask());
            brokerMock.Verify(
                b => b.SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync_EmptyKey_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync(Guid.NewGuid(), Guid.NewGuid(), CreateRandomString(), string.Empty).AsTask());
            brokerMock.Verify(
                b => b.SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync_NotFound_ThrowsCustomItemDisplayPreferencesNotFoundException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences?>((CustomItemDisplayPreferences?)null));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<CustomItemDisplayPreferencesNotFoundException>(
                () => service.RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync(Guid.NewGuid(), Guid.NewGuid(), CreateRandomString(), CreateRandomString()).AsTask());
        }

        [Fact]
        public async Task RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync_Found_ReturnsPreferences()
        {
            // given
            CustomItemDisplayPreferences expectedPreferences = CreateRandomCustomItemDisplayPreferences();
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            string client = CreateRandomString();
            string key = CreateRandomString();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectCustomItemDisplayPreferencesByUserItemClientKeyAsync(userId, itemId, client, key))
                .Returns(new ValueTask<CustomItemDisplayPreferences?>(expectedPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            CustomItemDisplayPreferences actualPreferences = await service
                .RetrieveCustomItemDisplayPreferencesByUserItemClientKeyAsync(userId, itemId, client, key);

            // then
            Assert.Same(expectedPreferences, actualPreferences);
        }

        [Fact]
        public async Task ModifyCustomItemDisplayPreferencesAsync_NullPreferences_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.ModifyCustomItemDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.UpdateCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task ModifyCustomItemDisplayPreferencesAsync_ValidPreferences_CallsBrokerUpdate()
        {
            // given
            CustomItemDisplayPreferences inputPreferences = CreateRandomCustomItemDisplayPreferences();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.UpdateCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences>(inputPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            CustomItemDisplayPreferences actualPreferences = await service.ModifyCustomItemDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.UpdateCustomItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task RemoveCustomItemDisplayPreferencesAsync_NullPreferences_ThrowsInvalidCustomItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidCustomItemDisplayPreferencesException>(
                () => service.RemoveCustomItemDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task RemoveCustomItemDisplayPreferencesAsync_ValidPreferences_CallsBrokerDelete()
        {
            // given
            CustomItemDisplayPreferences inputPreferences = CreateRandomCustomItemDisplayPreferences();
            var brokerMock = new Mock<ICustomItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.DeleteCustomItemDisplayPreferencesAsync(It.IsAny<CustomItemDisplayPreferences>()))
                .Returns(new ValueTask<CustomItemDisplayPreferences>(inputPreferences));
            var service = new CustomItemDisplayPreferencesService(brokerMock.Object);

            // when
            CustomItemDisplayPreferences actualPreferences = await service.RemoveCustomItemDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.DeleteCustomItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }
    }
}
