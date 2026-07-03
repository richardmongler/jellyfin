using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Api.Controllers;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Security.Exceptions;
using Jellyfin.Server.Implementations.Security.Services;
using MediaBrowser.Controller.Security;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Jellyfin.Api.Tests.Controllers;

public class ApiKeyControllerTests
{
    private readonly Mock<IApiKeyService> _apiKeyServiceMock;
    private readonly ApiKeyController _controller;

    public ApiKeyControllerTests()
    {
        _apiKeyServiceMock = new Mock<IApiKeyService>();
        _controller = new ApiKeyController(_apiKeyServiceMock.Object);
    }

    [Fact]
    public async Task GetKeysAsync_DelegatesToRetrieveAllAndMapsToQueryResult()
    {
        // given
        var apiKeys = new List<ApiKey>
        {
            new("firstApp"),
            new("secondApp")
        };

        _apiKeyServiceMock.Setup(s => s.RetrieveAllApiKeysAsync())
            .Returns(new ValueTask<IReadOnlyList<ApiKey>>(apiKeys));

        // when
        ActionResult<QueryResult<AuthenticationInfo>> actionResult =
            await _controller.GetKeys();

        // then
        _apiKeyServiceMock.Verify(s => s.RetrieveAllApiKeysAsync(), Times.Once);

        var queryResult = Assert.IsType<QueryResult<AuthenticationInfo>>(actionResult.Value);

        Assert.Equal(apiKeys.Count, queryResult.Items.Count);

        foreach ((ApiKey expected, AuthenticationInfo actual) in
            apiKeys.Zip(queryResult.Items, (expected, actual) => (expected, actual)))
        {
            Assert.Equal(expected.Name, actual.AppName);
            Assert.Equal(expected.AccessToken, actual.AccessToken);
            Assert.Equal(expected.DateCreated, actual.DateCreated);
            Assert.Empty(actual.DeviceId);
            Assert.Empty(actual.DeviceName);
            Assert.Empty(actual.AppVersion);
        }
    }

    [Fact]
    public async Task CreateKeyAsync_DelegatesToAddApiKeyWithGivenAppName()
    {
        // given
        const string AppName = "testApp";
        ApiKey? capturedApiKey = null;

        _apiKeyServiceMock.Setup(s => s.AddApiKeyAsync(It.IsAny<ApiKey>()))
            .Callback<ApiKey>(apiKey => capturedApiKey = apiKey)
            .Returns(new ValueTask<ApiKey>(capturedApiKey!));

        // when
        ActionResult result = await _controller.CreateKey(AppName);

        // then
        Assert.IsType<NoContentResult>(result);
        Assert.NotNull(capturedApiKey);
        Assert.Equal(AppName, capturedApiKey!.Name);
        Assert.False(string.IsNullOrEmpty(capturedApiKey.AccessToken));
        _apiKeyServiceMock.Verify(s => s.AddApiKeyAsync(It.IsAny<ApiKey>()), Times.Once);
    }

    [Fact]
    public async Task RevokeKeyAsync_DelegatesToRetrieveThenRemoveApiKey()
    {
        // given
        const string Token = "a1b2c3";
        var existingApiKey = new ApiKey("testApp") { AccessToken = Token };

        _apiKeyServiceMock.Setup(s => s.RetrieveApiKeyByAccessTokenAsync(Token))
            .Returns(new ValueTask<ApiKey>(existingApiKey));
        _apiKeyServiceMock.Setup(s => s.RemoveApiKeyAsync(existingApiKey))
            .Returns(new ValueTask<ApiKey>(existingApiKey));

        // when
        ActionResult result = await _controller.RevokeKey(Token);

        // then
        Assert.IsType<NoContentResult>(result);
        _apiKeyServiceMock.Verify(s => s.RetrieveApiKeyByAccessTokenAsync(Token), Times.Once);
        _apiKeyServiceMock.Verify(s => s.RemoveApiKeyAsync(existingApiKey), Times.Once);
    }

    [Fact]
    public async Task RevokeKeyAsync_WhenServiceThrowsNotFound_PropagatesException()
    {
        // given
        const string Token = "missing";
        _apiKeyServiceMock.Setup(s => s.RetrieveApiKeyByAccessTokenAsync(Token))
            .ThrowsAsync(new ApiKeyNotFoundException(Token));

        // when . then
        await Assert.ThrowsAsync<ApiKeyNotFoundException>(() => _controller.RevokeKey(Token));
        _apiKeyServiceMock.Verify(s => s.RemoveApiKeyAsync(It.IsAny<ApiKey>()), Times.Never);
    }
}
