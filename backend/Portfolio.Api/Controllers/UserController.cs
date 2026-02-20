using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Api.Services;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly AuthenticationService _authService;

    public UserController(AuthenticationService authService)
    {
        _authService = authService;
    }

    public struct SignupRequest
    {
        public string email { get; set; }
        public string displayName { get; set; }
        public string password { get; set; }
    }

    public struct SimpleUser
    {
        public string email { get; set; }
        public string displayName { get; set; }

        public SimpleUser(UserModel usr)
        {
            email = usr.email;
            displayName = usr.displayName;
        }
    }

    [HttpPost("signup")]
    public async Task<IResult> SignUp([FromBody] SignupRequest request)
    {
        UserModel usr = await _authService.CreateUserEntry(request.email, request.displayName, request.password);

        AddTokenCookie(usr);
        return Results.Json(new SimpleUser(usr));
    }

    public struct LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest login)
    {
        UserModel? usr = await _authService.ConfirmLogin(login.email, login.password);

        if (usr != null)
        {
            AddTokenCookie(usr);
            return Results.Json(new SimpleUser(usr));
        }

        return Results.BadRequest("Invalid login");
    }

    [HttpGet("profile")]
    public async Task<IResult> Profile()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            UserModel? usr = await GetSessionUser();

            // invalidate token
            if (usr == null)
            {
                HttpContext.Response.Cookies.Delete(AuthenticationService.AUTH_COOKIE_NAME);
                return Results.Ok(null);
            }

            return Results.Ok(usr);
        }

        return Results.Ok(null); // user not logged in, return null
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IResult> Logout()
    {
        UserModel? usr = await GetSessionUser();

        if (usr == null)
            return Results.Unauthorized();

        HttpContext.Response.Cookies.Delete(AuthenticationService.AUTH_COOKIE_NAME);
        await _authService.ClearUserSession(usr);

        return Results.Ok();
    }

    private void AddTokenCookie(UserModel usr)
    {
        string token = _authService.GenerateToken(usr);

        HttpContext.Response.Cookies.Append(AuthenticationService.AUTH_COOKIE_NAME, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });
    }

    private async Task<UserModel?> GetSessionUser()
    {
        string? userGuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userGuid, out Guid id))
        {
            return await _authService.GetLogin(id);
        }

        return null;
    }
}
