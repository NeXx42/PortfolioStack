using System.Net.NetworkInformation;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;

namespace Portfolio.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRoles.Admin))]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ContentService _content;
    private readonly AdminService _admin;
    private readonly CacheService _cache;

    private readonly string _releaseEngineUrl;

    public AdminController(ContentService content, AdminService admin, CacheService cache, IOptions<GeneralSettings> settings)
    {
        _content = content;
        _admin = admin;
        _cache = cache;

        _releaseEngineUrl = settings.Value.releaseEngineUrl;
    }

    [HttpGet("clearCache")]
    public IResult ClearCache()
    {
        _cache.Clear();
        return Results.Ok();
    }

    [HttpGet("tags")]
    public async Task<IResult> GetTags()
    {
        try
        {
            var res = await _admin.GetTags();
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e.Message);
        }
    }

    [HttpPost("tags")]
    public async Task<IResult> AddTags(ProjectDto.Tag[] tags)
    {
        await _admin.SaveTags(tags);
        return Results.Ok();
    }

    // new

    [HttpGet("{projectId}")]
    public async Task<IResult> GetProject(Guid projectId)
    {
        try
        {
            ProjectDto project = await _admin.GetProject(projectId);
            return Results.Json(project);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPost("project/create")]
    public async Task<IResult> CreateProject()
    {
        try
        {
            ProjectModel project = await _admin.CreateProject();
            return Results.Json(project.slug);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpGet("projects")]
    public async Task<IResult> GetProjects()
    {
        try
        {
            ProjectDto[] projects = await _content.GetAllProjects();
            return Results.Json(projects);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    public class SaveProjectRequest
    {
        public string data { get; set; } = "";
    }

    [HttpPost("project/save")]
    [Consumes("multipart/form-data")]
    public async Task<IResult> SaveProject([FromForm] SaveProjectRequest request)
    {
        try
        {
            ProjectDto newData = JsonSerializer.Deserialize<ProjectDto>(request.data)!;
            IFormCollection form = await Request.ReadFormAsync();

            foreach (IFormFile file in form.Files)
            {
                string uri = await _admin.SaveImage(file);
                newData.icon = uri;

                break;
            }

            await _admin.SaveProject(newData);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    public class SaveProjectContentRequest
    {
        public int ContentId { get; set; }
        public string NewData { get; set; } = "";
    }

    [HttpPost("project/{projectId}/save")]
    [Consumes("multipart/form-data")]
    public async Task<IResult> SaveProjectContent(Guid projectId, [FromForm] SaveProjectContentRequest request)
    {
        try
        {
            ProjectDto.ElementGroup newData = JsonSerializer.Deserialize<ProjectDto.ElementGroup>(request.NewData)!;

            IFormCollection form = await Request.ReadFormAsync();
            Dictionary<string, IFormFile> files = new Dictionary<string, IFormFile>();

            foreach (IFormFile file in form.Files)
                files.Add(file.FileName, file);

            foreach (var element in newData.elements ?? [])
            {
                if (files.TryGetValue(element.value1!, out IFormFile? img))
                {
                    string uri = await _admin.SaveImage(img);
                    element.value1 = uri;
                }
            }

            await _admin.SaveProjectContent(projectId, newData);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpGet("project/{projectId}/releases")]
    public async Task<IResult> GetProjectReleases(Guid projectId)
    {
        try
        {
            var releases = await _content.GetReleases(projectId);
            return Results.Json(releases);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPost("project/{projectId}/release")]
    public async Task<IResult> CreateOrUpdateRelease(Guid projectId, [FromBody] ReleaseDTO release)
    {
        try
        {
            await _admin.CreateOrUpdateRelease(projectId, release);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPost("project/{projectId}/release/{releaseId}")]
    public async Task<IResult> CreateReleaseEngineEntry(Guid projectId, int releaseId, [FromQuery] string platform)
    {
        try
        {
            string uploadUri = await _admin.UploadToReleaseEngine(projectId, releaseId, platform);
            return Results.Ok(uploadUri);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPut("project/{ProjectId}/release/{ReleaseId}")]
    [RequestSizeLimit(10L * (1024 * 1024 * 1024))]
    public async Task<IResult> UploadFileToReleaseEngine(Guid projectId, int releaseId, [FromQuery] string platform, [FromQuery] string relativePath)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                string url = Path.Combine(_releaseEngineUrl, "api", "Releases", projectId.ToString(), $"{releaseId}?platform={Uri.EscapeDataString(platform)}&relativePath={Uri.EscapeDataString(relativePath)}");
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
}
