using System.Text.Json;
using AuthEngineShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;

namespace Portfolio.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRoles.Admin))]
[Route("api/admin")]
public class AdminController(ReleaseService _releases, AdminService _admin, CacheService _cache) : ControllerBase
{

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
    public async Task<IResult> AddTags(TagDto[] tags)
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
            ProjectDto[] projects = await _admin.GetAllProjects();
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
            ElementDto newData = JsonSerializer.Deserialize<ElementDto>(request.NewData)!;

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
            var releases = await _releases.GetReleases(projectId);
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
            await _releases.CreateOrUpdateRelease(projectId, release);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }
}
