using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Security.Brokers;
using Jellyfin.Server.Implementations.Security.Exceptions;
using Jellyfin.Server.Implementations.Security.Services;
using Moq;
using Xunit;

namespace Jellyfin.Server.Implementations.Tests.Security
{
    public partial class ApiKeyServiceTests
    {
        [Fact]
        public async Task AddApiKeyAsync_ValidApiKey_CallsBrokerInsertAndReturnsSameKey()
        {
            // given
            ApiKey inputApiKey = CreateRandomApiKey();
            var brokerMock = new Mock<IApiKeyBroker>();
            brokerMock.Setup(b => b.InsertApiKeyAsync(It.IsAny<ApiKey>()))
                .Returns(new ValueTask<ApiKey>(inputApiKey));
            var service = new ApiKeyService(brokerMock.Object);

            // when
            ApiKey actualApiKey = await service.AddApiKeyAsync(inputApiKey);

            // then
            Assert.Same(inputApiKey, actualApiKey);
            brokerMock.Verify(b => b.InsertApiKeyAsync(inputApiKey), Times.Once);
        }

        [Fact]
        public async Task AddApiKeyAsync_NullApiKey_ThrowsInvalidApiKeyException()
        {
            // given
            var brokerMock = new Mock<IApiKeyBroker>();
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidApiKeyException>(
                () => service.AddApiKeyAsync(null!).AsTask());
            brokerMock.Verify(b => b.InsertApiKeyAsync(It.IsAny<ApiKey>()), Times.Never);
        }

        [Fact]
        public async Task AddApiKeyAsync_EmptyName_ThrowsInvalidApiKeyException()
        {
            // given
            ApiKey invalidApiKey = CreateRandomApiKey();
            invalidApiKey.Name = string.Empty;
            var brokerMock = new Mock<IApiKeyBroker>();
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidApiKeyException>(
                () => service.AddApiKeyAsync(invalidApiKey).AsTask());
            brokerMock.Verify(b => b.InsertApiKeyAsync(It.IsAny<ApiKey>()), Times.Never);
        }

        [Fact]
        public async Task AddApiKeyAsync_EmptyAccessToken_ThrowsInvalidApiKeyException()
        {
            // given
            ApiKey invalidApiKey = CreateRandomApiKey();
            invalidApiKey.AccessToken = string.Empty;
            var brokerMock = new Mock<IApiKeyBroker>();
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidApiKeyException>(
                () => service.AddApiKeyAsync(invalidApiKey).AsTask());
            brokerMock.Verify(b => b.InsertApiKeyAsync(It.IsAny<ApiKey>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveAllApiKeysAsync_ReturnsAllFromBroker()
        {
            // given
            IReadOnlyList<ApiKey> expectedApiKeys = CreateRandomApiKeys();
            var brokerMock = new Mock<IApiKeyBroker>();
            brokerMock.Setup(b => b.SelectAllApiKeysAsync())
                .Returns(new ValueTask<IReadOnlyList<ApiKey>>(expectedApiKeys));
            var service = new ApiKeyService(brokerMock.Object);

            // when
            IReadOnlyList<ApiKey> actualApiKeys = await service.RetrieveAllApiKeysAsync();

            // then
            Assert.Equal(expectedApiKeys, actualApiKeys);
            brokerMock.Verify(b => b.SelectAllApiKeysAsync(), Times.Once);
        }

        [Fact]
        public async Task RetrieveApiKeyByIdAsync_InvalidId_ThrowsInvalidApiKeyException()
        {
            // given
            var brokerMock = new Mock<IApiKeyBroker>();
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidApiKeyException>(
                () => service.RetrieveApiKeyByIdAsync(0).AsTask());
            brokerMock.Verify(b => b.SelectApiKeyByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveApiKeyByIdAsync_NotFound_ThrowsApiKeyNotFoundException()
        {
            // given
            var brokerMock = new Mock<IApiKeyBroker>();
            brokerMock.Setup(b => b.SelectApiKeyByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<ApiKey?>((ApiKey?)null));
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<ApiKeyNotFoundException>(
                () => service.RetrieveApiKeyByIdAsync(GetRandomNumber()).AsTask());
        }

        [Fact]
        public async Task RetrieveApiKeyByIdAsync_Found_ReturnsApiKey()
        {
            // given
            ApiKey expectedApiKey = CreateRandomApiKey();
            var brokerMock = new Mock<IApiKeyBroker>();
            brokerMock.Setup(b => b.SelectApiKeyByIdAsync(It.IsAny<int>()))
                .Returns(new ValueTask<ApiKey?>(expectedApiKey));
            var service = new ApiKeyService(brokerMock.Object);

            // when
            ApiKey actualApiKey = await service.RetrieveApiKeyByIdAsync(GetRandomNumber());

            // then
            Assert.Same(expectedApiKey, actualApiKey);
        }

        [Fact]
        public async Task RetrieveApiKeyByAccessTokenAsync_EmptyToken_ThrowsInvalidApiKeyException()
        {
            // given
            var brokerMock = new Mock<IApiKeyBroker>();
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidApiKeyException>(
                () => service.RetrieveApiKeyByAccessTokenAsync(string.Empty).AsTask());
            brokerMock.Verify(b => b.SelectApiKeyByAccessTokenAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveApiKeyByAccessTokenAsync_NotFound_ThrowsApiKeyNotFoundException()
        {
            // given
            var brokerMock = new Mock<IApiKeyBroker>();
            brokerMock.Setup(b => b.SelectApiKeyByAccessTokenAsync(It.IsAny<string>()))
                .Returns(new ValueTask<ApiKey?>((ApiKey?)null));
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<ApiKeyNotFoundException>(
                () => service.RetrieveApiKeyByAccessTokenAsync(CreateRandomAccessToken()).AsTask());
        }

        [Fact]
        public async Task RetrieveApiKeyByAccessTokenAsync_Found_ReturnsApiKey()
        {
            // given
            ApiKey expectedApiKey = CreateRandomApiKey();
            string token = CreateRandomAccessToken();
            var brokerMock = new Mock<IApiKeyBroker>();
            brokerMock.Setup(b => b.SelectApiKeyByAccessTokenAsync(token))
                .Returns(new ValueTask<ApiKey?>(expectedApiKey));
            var service = new ApiKeyService(brokerMock.Object);

            // when
            ApiKey actualApiKey = await service.RetrieveApiKeyByAccessTokenAsync(token);

            // then
            Assert.Same(expectedApiKey, actualApiKey);
        }

        [Fact]
        public async Task RemoveApiKeyAsync_NullApiKey_ThrowsInvalidApiKeyException()
        {
            // given
            var brokerMock = new Mock<IApiKeyBroker>();
            var service = new ApiKeyService(brokerMock.Object);

            // when . then
            await Assert.ThrowsAsync<InvalidApiKeyException>(
                () => service.RemoveApiKeyAsync(null!).AsTask());
            brokerMock.Verify(b => b.DeleteApiKeyAsync(It.IsAny<ApiKey>()), Times.Never);
        }

        [Fact]
        public async Task RemoveApiKeyAsync_ValidApiKey_CallsBrokerDelete()
        {
            // given
            ApiKey inputApiKey = CreateRandomApiKey();
            var brokerMock = new Mock<IApiKeyBroker>();
            brokerMock.Setup(b => b.DeleteApiKeyAsync(It.IsAny<ApiKey>()))
                .Returns(new ValueTask<ApiKey>(inputApiKey));
            var service = new ApiKeyService(brokerMock.Object);

            // when
            ApiKey actualApiKey = await service.RemoveApiKeyAsync(inputApiKey);

            // then
            Assert.Same(inputApiKey, actualApiKey);
            brokerMock.Verify(b => b.DeleteApiKeyAsync(inputApiKey), Times.Once);
        }
    }
}
