using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Core.DTOs;

namespace Portfolio.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class UserController : ControllerBase
{
    private readonly MailService _mail;
    private readonly AuthenticationService _authService;

    private readonly GeneralSettings _options;

    public UserController(AuthenticationService authService, MailService mail, IOptions<GeneralSettings> options)
    {
        _mail = mail;
        _authService = authService;

        _options = options.Value;
    }

    public struct SignupRequest
    {
        public string email { get; set; }
        public string displayName { get; set; }
        public string password { get; set; }
        public long emailVerification { get; set; }

        public bool Validate()
        {
            return email.Contains("@") && !string.IsNullOrEmpty(displayName) && !string.IsNullOrEmpty(password);
        }
    }

    [HttpPost("signup")]
    public async Task<IResult> SignUp([FromBody] SignupRequest request)
    {
        if (_options.disableAccountCreation)
        {
            return Results.InternalServerError("Account creation is disabled");
        }

        try
        {
            _authService.ValidateCreation(request.email, request.displayName, request.password, request.emailVerification);
            UserDto usr = await _authService.CreateUserEntry(request.email, request.displayName, request.password);

            AddTokenCookie(usr);
            return Results.Json(usr);
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    public struct LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest login)
    {
        try
        {
            UserDto? usr = await _authService.ConfirmLogin(login.email, login.password);

            if (usr != null)
            {
                AddTokenCookie(usr);
                return Results.Json(usr);
            }

            return Results.BadRequest("Invalid login");
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    [HttpGet("profile")]
    public async Task<IResult> Profile()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            UserDto? usr = await GetSessionUser();

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
        UserDto? usr = await GetSessionUser();

        if (usr == null)
            return Results.Unauthorized();

        HttpContext.Response.Cookies.Delete(AuthenticationService.AUTH_COOKIE_NAME);
        await _authService.ClearUserSession(usr);

        return Results.Ok();
    }


    public struct EmailVerification
    {
        public string address { get; set; }
        public long? code { get; set; }
    }

    [HttpPost("email/verification")]
    public async Task<IResult> RequestEmailVerification([FromBody] EmailVerification req)
    {
        try
        {
            if (string.IsNullOrEmpty(req.address) || !req.address.Contains("@"))
                return Results.BadRequest("Invalid email");

            await _mail.SendEmailVerification(req.address);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.BadRequest(e.Message);
        }
    }

    [HttpPost("email/confirm")]
    public async Task<IResult> ConfirmEmailVerification([FromBody] EmailVerification req)
    {
        try
        {
            if (string.IsNullOrEmpty(req.address) || !req.address.Contains("@"))
                return Results.BadRequest("Invalid email");

            if (_mail.ConfirmCode(req.address, req.code))
            {
                return Results.Ok();
            }
            else
            {
                return Results.BadRequest();
            }
        }
        catch (Exception e)
        {
            return Results.InternalServerError(e.Message);
        }
    }

    private void AddTokenCookie(UserDto usr)
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

    private async Task<UserDto?> GetSessionUser()
    {
        string? userGuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userGuid, out Guid id))
        {
            return await _authService.GetLogin(id);
        }

        return null;
    }
}
