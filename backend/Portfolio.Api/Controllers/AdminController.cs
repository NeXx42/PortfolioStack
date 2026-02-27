using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;

namespace Portfolio.Api.Controllers;

[ApiController]
[Authorize(Roles = nameof(UserRoles.Admin))]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private ContentService _content;
    private CacheService _cache;

    public AdminController(ContentService content, CacheService cache)
    {
        _content = content;
        _cache = cache;
    }

    [HttpGet("slugs")]
    public async Task<IResult> GetContent()
    {
        var res = await _content.GetSlugs();
        return Results.Json(res);
    }

    [HttpGet("clearCache")]
    public IResult ClearCache()
    {
        _cache.Clear();
        return Results.Ok();
    }

    [HttpPost("save")]
    public async Task<IResult> SaveItem(ProjectDto project)
    {
        await _content.Save(project);
        return Results.Ok();
    }

    [HttpPost("upload")]
    public async Task<IResult> SaveImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Results.BadRequest("No file provided");

        try
        {
            var res = await _content.SaveImage(file);
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e.Message);
        }
    }

    [HttpGet("images")]
    public async Task<IResult> GetImages()
    {
        try
        {
            var res = await _content.GetImages();
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e.Message);
        }
    }

    [HttpGet("tags")]
    public async Task<IResult> GetTags()
    {
        try
        {
            var res = await _content.GetTags();
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
        await _content.SaveTags(tags);
        return Results.Ok();
    }
}
