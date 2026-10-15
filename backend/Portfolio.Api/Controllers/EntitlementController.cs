using AuthEngineShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;
using Portfolio.Core.Data;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/Entitlements")]
public class EntitlementController(EntitlementService _service) : ControllerBase
{
    [HttpGet("{ProjectId}")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IResult> GetEntitlements(Guid projectId)
    {
        try
        {
            var res = await _service.GetEntitlements(projectId);
            return Results.Json(res);
        }
        catch (HttpRequestException http)
        {
            if (http.StatusCode == System.Net.HttpStatusCode.NotFound)
                return Results.NotFound();

            return Results.InternalServerError(http);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPost("{ProjectId}/Create")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IResult> UseEntitlements(Guid projectId)
    {
        try
        {
            await _service.UseEntitlements(projectId);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPost("{ProjectId}")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IResult> UpdateEntitlements(Guid projectId, [FromBody] EntitlementInfo[] entitlements)
    {
        try
        {
            await _service.UpdateEntitlements(projectId, entitlements);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpGet("{ProjectId}/UserEntitlements")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IResult> GetUserEntitlements(Guid projectId, [FromQuery] Guid userId)
    {
        try
        {
            var res = await _service.GetUserEntitlements(projectId, userId);
            return Results.Json(res);
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }

    [HttpPost("{ProjectId}/UserEntitlements")]
    [Authorize(Roles = nameof(UserRoles.Admin))]
    public async Task<IResult> UpdateUserEntitlements(Guid projectId, [FromQuery] Guid userId, [FromBody] UserEntitlementInfo[] entitlements)
    {
        try
        {
            await _service.UpdateUserEntitlements(projectId, userId, entitlements);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e);
        }
    }
}
