using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public class AuthenticationService
{
    public const string AUTH_COOKIE_NAME = "auth_token";

    private readonly MailService _mail;
    private readonly string _authServiceURL;

    public AuthenticationService(MailService mail, IOptions<SecuritySettings> securitySettings)
    {
        _mail = mail;
        _authServiceURL = securitySettings.Value.authServiceURL;
    }

    public async Task<string> CreateUserEntry(string email, string displayName, string password)
    {
        return (await SendRequestToAuthService<string>(HttpMethod.Post, "register", new
        {
            email,
            displayName,
            password,
        }))!;
    }

    public async Task<string> Login(string email, string password)
    {
        return (await SendRequestToAuthService<string>(HttpMethod.Post, "", new
        {
            email,
            password
        }))!;
    }

    private async Task<T?> SendRequestToAuthService<T>(HttpMethod method, string path, object json)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpRequestMessage msg = new HttpRequestMessage(method, Path.Combine(_authServiceURL, path));
            msg.Content = new StringContent(JsonSerializer.Serialize(json), Encoding.UTF8, "application/json");

            HttpResponseMessage res = await client.SendAsync(msg);
            res.EnsureSuccessStatusCode();

            string resText = await res.Content!.ReadAsStringAsync(); ;
            return JsonSerializer.Deserialize<T>(resText);
        }
    }

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
