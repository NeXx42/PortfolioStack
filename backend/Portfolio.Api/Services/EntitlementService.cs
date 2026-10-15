using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Primitives;
using Portfolio.Core.Data;

namespace Portfolio.Api.Services;

public class EntitlementService : IDisposable
{
    private HttpClient _client;
    private string? _authCookie;

    public bool hasValidCookie => !string.IsNullOrEmpty(_authCookie);

    public EntitlementService(string _url, IHttpContextAccessor _httpAccessor)
    {
        if (!_httpAccessor.HttpContext?.Request.Cookies.TryGetValue(AuthenticationService.AUTH_COOKIE_NAME, out _authCookie) ?? false)
        {
            if (_httpAccessor.HttpContext?.Request.Headers.TryGetValue("x-key", out StringValues token) ?? false)
            {
                _authCookie = token.ToString();
            }
        }

        _client = new HttpClient()
        {
            BaseAddress = new Uri(Path.Combine(_url, "api/"))
        };
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task UseEntitlements(Guid projectId)
    {
        await SendAsync(HttpMethod.Post, Path.Combine("licence", $"Create?projectId={projectId}"));
    }

    public async Task<EntitlementInfo[]> GetEntitlements(Guid projectId)
    {
        EntitlementInfo[]? entitlements = await SendAsyncAndDeserialize<EntitlementInfo[]>(HttpMethod.Get, Path.Combine("licence", projectId.ToString(), "Entitlements"));
        return entitlements ?? [];
    }

    public async Task UpdateEntitlements(Guid projectId, EntitlementInfo[] entitlements)
    {
        await SendAsync(HttpMethod.Post, Path.Combine("licence", projectId.ToString(), "Entitlements"), entitlements);
    }

    public async Task<UserEntitlementInfo[]> GetUserEntitlements(Guid projectId, Guid userId)
    {
        return (await SendAsyncAndDeserialize<UserEntitlementInfo[]>(HttpMethod.Get, Path.Combine("licence", projectId.ToString(), $"UserEntitlements?userId={userId}"))) ?? [];
    }

    public async Task<UserEntitlementInfo[]> Enrich(Guid projectId)
    {
        // sends cookie from request sender, so no need to pass it into the function
        return (await SendAsyncAndDeserialize<UserEntitlementInfo[]>(HttpMethod.Get, Path.Combine("licence", projectId.ToString(), $"Enrich"))) ?? [];
    }

    public async Task UpdateUserEntitlements(Guid projectId, Guid userId, UserEntitlementInfo[] entitlements)
    {
        await SendAsync(HttpMethod.Post, Path.Combine("licence", projectId.ToString(), $"PatchUserEntitlement?userId={userId}"), entitlements);
    }


    private async Task<T?> SendAsyncAndDeserialize<T>(HttpMethod method, string uri, object? body = null)
    {
        var obj = await SendAsync(method, uri, body);

        return JsonSerializer.Deserialize<T>(await obj.Content.ReadAsStringAsync(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string uri, object? body = null)
    {
        HttpResponseMessage? obj = null;

        try
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uri);
            httpRequestMessage.Headers.Add("x-key", _authCookie);

            if (body != null)
            {
                httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            }

            obj = await _client.SendAsync(httpRequestMessage);
            obj.EnsureSuccessStatusCode();

            return obj;
        }
        catch
        {
            if (obj != null)
            {
                string res = await obj.Content.ReadAsStringAsync();
                Console.WriteLine(res);
            }

            throw;
        }
    }
}
