using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AuthEngineMiddleman;
using Microsoft.Extensions.Options;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public class AuthenticationService : IAuthenticationService
{
    public const string AUTH_COOKIE_NAME = "auth_token";

    private readonly MailService _mail;
    private readonly AuthEngineMiddlemanService _middleman;

    public AuthenticationService(MailService mail, AuthEngineMiddlemanService middleman)
    {
        _mail = mail;
        _middleman = middleman;
    }

    public async Task<string> CreateUserEntry(string email, string displayName, string password) => await _middleman.Register(email, displayName, password);
    public async Task<string> Login(string email, string password) => await _middleman.Authenticate(email, password);

    public void ValidateCreation(string email, string displayName, string password, long verification)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            throw new Exception("Invalid email");

        if (string.IsNullOrEmpty(password))
            throw new Exception("Invalid password");

        if (string.IsNullOrEmpty(displayName))
            throw new Exception("Invalid name");

        if (!_mail.ConfirmCode(email, verification))
            throw new Exception("Email not verified");
    }
}
