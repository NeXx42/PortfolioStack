using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Portfolio.Api.Services;
using Portfolio.Core.Data;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    private readonly ContentService _content;

    public ContentController(ContentService content)
    {
        _content = content;
    }

    [HttpGet]
    public async Task<IResult> GetContent(ProjectType type)
    {
        try
        {
            var res = await _content.GetContentForType(type);
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e.Message);
        }
    }

    [HttpGet("featured")]
    public async Task<IResult> GetFeaturedContent()
    {
        try
        {
            var res = await _content.FeaturedContent();
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e.Message);
        }
    }

    [HttpGet("{slug}")]
    public async Task<IResult> GetGame(string slug)
    {
        var res = await _content.GetGame(slug);

        if (res == null)
            return Results.NotFound();

        return Results.Json(res);
    }



    public class UpdateRequest
    {
        public ProjectDto.ElementGroup[]? newPages { get; set; }
        public ProjectDto.ElementGroup[]? updates { get; set; }
        public int[]? deletions { get; set; }
    }

    [HttpGet("Links")]
    public async Task<IResult> GetLinks()
    {
        try
        {
            var res = await _content.GetLinks();
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.Json(e.Message);
        }
    }
}
