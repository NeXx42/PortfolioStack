using AuthEngineShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Portfolio.Api.Helpers;
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
            UserObject? usr = await SessionHelper.GetSessionUser(User);
            var res = await _content.GetContentForType(usr, type);

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
            UserObject? usr = await SessionHelper.GetSessionUser(User);
            var res = await _content.FeaturedContent(usr);

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
        UserObject? usr = await SessionHelper.GetSessionUser(User);
        var res = await _content.GetGame(usr, slug);

        if (res == null)
            return Results.NotFound();

        return Results.Json(res);
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

    [HttpGet("GameLauncher")]
    public async Task<IResult> GetGameLauncherMetadata([FromQuery] Guid? featuredId = null, [FromQuery] int? limit = null)
    {
        try
        {
            UserObject? usr = await SessionHelper.GetSessionUser(User);

            var res = await _content.GetGameLauncherMetadata(usr, featuredId, limit);
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }
}
