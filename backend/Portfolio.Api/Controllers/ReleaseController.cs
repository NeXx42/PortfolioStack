using System.IO.Compression;
using System.Security.Cryptography;
using AuthEngineShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Api.Helpers;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Core.DTOs;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/Releases")]
public class ReleaseController(ReleaseService _service, CacheService _cache, IOptions<GeneralSettings> _settings) : ControllerBase
{
    [HttpPost("{projectId}/{releaseId}")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IResult> CreateReleaseEngineEntry(Guid projectId, int releaseId, [FromQuery] string platform)
    {
        try
        {
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
    [Authorize(Roles = nameof(UserRoles.Admin))]
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
    [Authorize(Roles = nameof(UserRoles.Admin))]
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

    [HttpGet("{projectId}/{releaseId}")]
    public async Task<IResult> GetDownloadFiles(Guid projectId, int releaseId, [FromQuery] string platform)
    {
        try
        {
            var files = await _service.GetDownloadFiles(projectId, releaseId, platform);
            return Results.Json(files);
        }
        catch (ArgumentException e)
        {
            return Results.BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    [HttpPut("Upload")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    [RequestSizeLimit(10L * (1024 * 1024 * 1024))]
    public async Task<IResult> UploadGenericFile([FromQuery] string fileName, CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[64 * 1024];
        string tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            string sha256;
            long totalBytes = 0;

            using (IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256))
            await using (FileStream output = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize: buffer.Length, useAsync: true))
            await using (GZipStream compression = new GZipStream(output, CompressionLevel.Optimal))
            {
                int bytesRead;

                while ((bytesRead = await Request.Body.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    hash.AppendData(buffer, 0, bytesRead);
                    await compression.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);

                    totalBytes += bytesRead;
                }

                sha256 = Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
            }

            long compressedSize = new FileInfo(tempPath).Length;
            await _service.UploadGenericFile(sha256, totalBytes, compressedSize, "gzip", tempPath, fileName);

            return Results.Ok();
        }
        finally
        {
            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);
        }
    }

    [HttpGet("Uploads")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IResult> GetGenericFiles()
    {
        try
        {
            var res = await _service.GetGenericFiles();
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpGet("{FileId}/Download")]
    public async Task<IResult> DownloadGenericFile(Guid fileId)
    {
        UserObject? usr = await SessionHelper.GetSessionUser(User);

        FileDto? file = await _service.GetGenericFileInfo(usr, fileId);
        string filePath = _service.GetGenericFilePath(fileId);

        if (file == null || !System.IO.File.Exists(filePath))
            return Results.NotFound();

        Func<Task<IResult>> decompressionTask = Uncompressed;

        switch (file.compression ?? "")
        {
            case "gzip":
                decompressionTask = Gzip;
                break;
        }

        return await decompressionTask();

        async Task<IResult> Gzip()
        {
            await using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            await using GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);

            return Results.File(gzip);
        }

        async Task<IResult> Uncompressed()
        {
            await using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            await using GZipStream gzip = new GZipStream(stream, CompressionMode.Decompress);

            return Results.File(gzip);
        }
    }
}
