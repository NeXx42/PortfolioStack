using AuthEngineShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Api.Services;
using Portfolio.Api.Types;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/Releases")]
[Authorize(Roles = nameof(UserRoles.Admin))]
public class ReleaseController(ReleaseService _service, CacheService _cache, IOptions<GeneralSettings> _settings) : ControllerBase
{
    [HttpPost("{projectId}/{releaseId}")]
    public async Task<IResult> CreateReleaseEngineEntry(Guid projectId, int releaseId, [FromQuery] string platform)
    {
        try
        {
            // auth here

            UploadSession session = await _service.CreateReleaseEngineEntry(projectId, releaseId, platform);
            Guid sessionId = Guid.NewGuid();

            _cache.SetIfNotExists(sessionId.ToString(), session, new TimeSpan(1, 0, 0, 0));
            return Results.Ok(sessionId);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPut("{sessionId}/Upload")]
    [RequestSizeLimit(10L * (1024 * 1024 * 1024))]
    public async Task<IResult> UploadFileToReleaseEngine(Guid sessionId, [FromQuery] string relativePath)
    {
        if (!_cache.TryGetValue(sessionId.ToString(), out UploadSession session))
            return Results.Unauthorized();

        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = Path.Combine(_settings.Value.releaseEngineUrl, "api", "Releases", session.projectId.ToString(), $"{session.releaseId}?platform={Uri.EscapeDataString(session.platform)}&relativePath={Uri.EscapeDataString(relativePath)}");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StreamContent(Request.Body)
                };

                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Request.ContentType ?? "application/octet-stream");
                HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();

                    return Results.Problem(
                        detail: error,
                        statusCode: (int)response.StatusCode
                    );
                }

                return Results.StatusCode((int)response.StatusCode);
            }
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    [HttpPost("{sessionId}/Complete")]
    public async Task<IResult> CompleteReleaseUpload(Guid sessionId)
    {
        if (!_cache.TryGetValue(sessionId.ToString(), out UploadSession session))
            return Results.Unauthorized();

        try
        {
            await _service.CompleteReleaseUpload(session);
            _cache.Remove(sessionId.ToString());

            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
}
