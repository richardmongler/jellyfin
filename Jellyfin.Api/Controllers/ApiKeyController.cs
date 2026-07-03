using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Jellyfin.Api.Constants;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Server.Implementations.Security.Services;
using MediaBrowser.Common.Api;
using MediaBrowser.Controller.Security;
using MediaBrowser.Model.Querying;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Api.Controllers;

/// <summary>
/// Authentication controller.
/// </summary>
[Route("Auth")]
[Tags("Authentication")]
public class ApiKeyController : BaseJellyfinApiController
{
    private readonly IApiKeyService _apiKeyService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyController"/> class.
    /// </summary>
    /// <param name="apiKeyService">Instance of <see cref="IApiKeyService"/> interface.</param>
    public ApiKeyController(IApiKeyService apiKeyService)
    {
        _apiKeyService = apiKeyService;
    }

    /// <summary>
    /// Get all keys.
    /// </summary>
    /// <response code="200">Api keys retrieved.</response>
    /// <returns>A <see cref="QueryResult{AuthenticationInfo}"/> with all keys.</returns>
    [HttpGet("Keys")]
    [Authorize(Policy = Policies.RequiresElevation)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<QueryResult<AuthenticationInfo>>> GetKeys()
    {
        var apiKeys = await _apiKeyService.RetrieveAllApiKeysAsync().ConfigureAwait(false);

        var keys = apiKeys.Select(key => new AuthenticationInfo
        {
            AppName = key.Name,
            AccessToken = key.AccessToken,
            DateCreated = key.DateCreated,
            DeviceId = string.Empty,
            DeviceName = string.Empty,
            AppVersion = string.Empty
        }).ToList();

        return new QueryResult<AuthenticationInfo>(keys);
    }

    /// <summary>
    /// Create a new api key.
    /// </summary>
    /// <param name="app">Name of the app using the authentication key.</param>
    /// <response code="204">Api key created.</response>
    /// <returns>A <see cref="NoContentResult"/>.</returns>
    [HttpPost("Keys")]
    [Authorize(Policy = Policies.RequiresElevation)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> CreateKey([FromQuery, Required] string app)
    {
        var apiKey = new ApiKey(name: app);
        await _apiKeyService.AddApiKeyAsync(apiKey).ConfigureAwait(false);

        return NoContent();
    }

    /// <summary>
    /// Remove an api key.
    /// </summary>
    /// <param name="key">The access token to delete.</param>
    /// <response code="204">Api key deleted.</response>
    /// <returns>A <see cref="NoContentResult"/>.</returns>
    [HttpDelete("Keys/{key}")]
    [Authorize(Policy = Policies.RequiresElevation)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> RevokeKey([FromRoute, Required] string key)
    {
        var apiKey = await _apiKeyService.RetrieveApiKeyByAccessTokenAsync(key).ConfigureAwait(false);
        await _apiKeyService.RemoveApiKeyAsync(apiKey).ConfigureAwait(false);

        return NoContent();
    }
}
