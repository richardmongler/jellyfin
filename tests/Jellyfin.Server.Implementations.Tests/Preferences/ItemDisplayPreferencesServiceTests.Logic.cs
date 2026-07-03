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
    public partial class ItemDisplayPreferencesServiceTests
    {
        [Fact]
        public async Task AddItemDisplayPreferencesAsync_ValidPreferences_CallsBrokerInsertAndReturnsSame()
        {
            // given
            ItemDisplayPreferences inputPreferences = CreateRandomItemDisplayPreferences();
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()))
                .Returns(new ValueTask<ItemDisplayPreferences>(inputPreferences));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when
            ItemDisplayPreferences actualPreferences = await service.AddItemDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.InsertItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task AddItemDisplayPreferencesAsync_NullPreferences_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.AddItemDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task AddItemDisplayPreferencesAsync_EmptyUserId_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            ItemDisplayPreferences invalidPreferences = new ItemDisplayPreferences(
                userId: Guid.Empty,
                itemId: Guid.NewGuid(),
                client: CreateRandomString());
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.AddItemDisplayPreferencesAsync(invalidPreferences).AsTask());
            brokerMock.Verify(b => b.InsertItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task AddItemDisplayPreferencesAsync_EmptyClient_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            ItemDisplayPreferences invalidPreferences = CreateRandomItemDisplayPreferences();
            invalidPreferences.Client = string.Empty;
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.AddItemDisplayPreferencesAsync(invalidPreferences).AsTask());
            brokerMock.Verify(b => b.InsertItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()), Times.Never);
        }

        // ItemId is a free Guid coordinate (Guid.Empty is the legacy "no item" sentinel) and is intentionally
        // NOT validated — a Guid.Empty ItemId must pass the Foundation the same as any other value.
        [Fact]
        public async Task AddItemDisplayPreferencesAsync_EmptyItemIdSentinel_PassesValidationAndReachesBroker()
        {
            // given
            ItemDisplayPreferences sentinelPreferences = new ItemDisplayPreferences(
                userId: Guid.NewGuid(),
                itemId: Guid.Empty,
                client: CreateRandomString());
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()))
                .Returns(new ValueTask<ItemDisplayPreferences>(sentinelPreferences));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when
            ItemDisplayPreferences actualPreferences = await service.AddItemDisplayPreferencesAsync(sentinelPreferences);

            // then
            Assert.Same(sentinelPreferences, actualPreferences);
            brokerMock.Verify(b => b.InsertItemDisplayPreferencesAsync(sentinelPreferences), Times.Once);
        }

        [Fact]
        public async Task RetrieveAllItemDisplayPreferencesAsync_ReturnsAllFromBroker()
        {
            // given
            IReadOnlyList<ItemDisplayPreferences> expectedPreferences = CreateRandomItemDisplayPreferencesList();
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectAllItemDisplayPreferencesAsync())
                .Returns(new ValueTask<IReadOnlyList<ItemDisplayPreferences>>(expectedPreferences));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when
            IReadOnlyList<ItemDisplayPreferences> actualPreferences = await service.RetrieveAllItemDisplayPreferencesAsync();

            // then
            Assert.Equal(expectedPreferences, actualPreferences);
            brokerMock.Verify(b => b.SelectAllItemDisplayPreferencesAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveItemDisplayPreferencesByIdAsync_InvalidId_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.RetrieveItemDisplayPreferencesByIdAsync(0).AsTask());
            brokerMock.Verify(b => b.SelectItemDisplayPreferencesByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveItemDisplayPreferencesByIdAsync_NotFound_ThrowsItemDisplayPreferencesNotFoundException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectItemDisplayPreferencesByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<ItemDisplayPreferences?>((ItemDisplayPreferences?)null));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<ItemDisplayPreferencesNotFoundException>(
                () => service.RetrieveItemDisplayPreferencesByIdAsync(GetRandomNumber()).AsTask());
        }

        [Fact]
        public async Task RetrieveItemDisplayPreferencesByIdAsync_Found_ReturnsPreferences()
        {
            // given
            ItemDisplayPreferences expectedPreferences = CreateRandomItemDisplayPreferences();
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectItemDisplayPreferencesByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<ItemDisplayPreferences?>(expectedPreferences));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when
            ItemDisplayPreferences actualPreferences = await service.RetrieveItemDisplayPreferencesByIdAsync(GetRandomNumber());

            // then
            Assert.Same(expectedPreferences, actualPreferences);
        }

        [Fact]
        public async Task RetrieveItemDisplayPreferencesByUserItemClientAsync_EmptyUserId_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.RetrieveItemDisplayPreferencesByUserItemClientAsync(Guid.Empty, Guid.NewGuid(), CreateRandomString()).AsTask());
            brokerMock.Verify(
                b => b.SelectItemDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveItemDisplayPreferencesByUserItemClientAsync_EmptyClient_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.RetrieveItemDisplayPreferencesByUserItemClientAsync(Guid.NewGuid(), Guid.NewGuid(), string.Empty).AsTask());
            brokerMock.Verify(
                b => b.SelectItemDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task RetrieveItemDisplayPreferencesByUserItemClientAsync_NotFound_ThrowsItemDisplayPreferencesNotFoundException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectItemDisplayPreferencesByUserItemClientAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new ValueTask<ItemDisplayPreferences?>((ItemDisplayPreferences?)null));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<ItemDisplayPreferencesNotFoundException>(
                () => service.RetrieveItemDisplayPreferencesByUserItemClientAsync(Guid.NewGuid(), Guid.NewGuid(), CreateRandomString()).AsTask());
        }

        [Fact]
        public async Task RetrieveItemDisplayPreferencesByUserItemClientAsync_Found_ReturnsPreferences()
        {
            // given
            ItemDisplayPreferences expectedPreferences = CreateRandomItemDisplayPreferences();
            var userId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            string client = CreateRandomString();
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.SelectItemDisplayPreferencesByUserItemClientAsync(userId, itemId, client))
                .Returns(new ValueTask<ItemDisplayPreferences?>(expectedPreferences));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when
            ItemDisplayPreferences actualPreferences = await service.RetrieveItemDisplayPreferencesByUserItemClientAsync(userId, itemId, client);

            // then
            Assert.Same(expectedPreferences, actualPreferences);
        }

        [Fact]
        public async Task ModifyItemDisplayPreferencesAsync_NullPreferences_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.ModifyItemDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.UpdateItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task ModifyItemDisplayPreferencesAsync_ValidPreferences_CallsBrokerUpdate()
        {
            // given
            ItemDisplayPreferences inputPreferences = CreateRandomItemDisplayPreferences();
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.UpdateItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()))
                .Returns(new ValueTask<ItemDisplayPreferences>(inputPreferences));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when
            ItemDisplayPreferences actualPreferences = await service.ModifyItemDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.UpdateItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task RemoveItemDisplayPreferencesAsync_NullPreferences_ThrowsInvalidItemDisplayPreferencesException()
        {
            // given
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidItemDisplayPreferencesException>(
                () => service.RemoveItemDisplayPreferencesAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()), Times.Never);
        }

        [Fact]
        public async Task RemoveItemDisplayPreferencesAsync_ValidPreferences_CallsBrokerDelete()
        {
            // given
            ItemDisplayPreferences inputPreferences = CreateRandomItemDisplayPreferences();
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.DeleteItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()))
                .Returns(new ValueTask<ItemDisplayPreferences>(inputPreferences));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when
            ItemDisplayPreferences actualPreferences = await service.RemoveItemDisplayPreferencesAsync(inputPreferences);

            // then
            Assert.Same(inputPreferences, actualPreferences);
            brokerMock.Verify(b => b.DeleteItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }

        [Fact]
        public async Task AddItemDisplayPreferencesAsync_BrokerThrows_IsWrappedAsItemDisplayPreferencesServiceException()
        {
            // given
            ItemDisplayPreferences inputPreferences = CreateRandomItemDisplayPreferences();
            var brokerMock = new Mock<IItemDisplayPreferencesBroker>();
            brokerMock.Setup(b => b.InsertItemDisplayPreferencesAsync(It.IsAny<ItemDisplayPreferences>()))
                .ThrowsAsync(new InvalidOperationException("broker down"));
            var service = new ItemDisplayPreferencesService(brokerMock.Object);

            // when . then
            var thrown = await Assert.ThrowsAsync<ItemDisplayPreferencesServiceException>(
                () => service.AddItemDisplayPreferencesAsync(inputPreferences).AsTask());
            Assert.IsType<FailedItemDisplayPreferencesServiceException>(thrown.InnerException);
            brokerMock.Verify(b => b.InsertItemDisplayPreferencesAsync(inputPreferences), Times.Once);
        }
    }
}
