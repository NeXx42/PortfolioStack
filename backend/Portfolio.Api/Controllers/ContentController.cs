using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Portfolio.Api.Services;
using Portfolio.Core.Data;
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
        var res = await _content.GetContentForType(type);
        return Results.Json(res);
    }

    [HttpGet("featured")]
    public async Task<IResult> GetFeaturedContent()
    {
        var res = await _content.FeaturedContent();
        return Results.Json(res);
    }

    [HttpGet("{slug}")]
    public async Task<IResult> GetGame(string slug)
    {
        var res = await _content.GetGame(slug);

        if (res == null)
            return Results.NotFound();

        return Results.Json(res);
    }

    [HttpGet("ClearCache")]
    //[Authorize]
    public async Task<IResult> ClearCache()
    {
        await _content.ClearCache();
        return Results.Ok();
    }
}
