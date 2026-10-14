using System.Text;
using System.Text.Json;
using Portfolio.Core.Data;

namespace Portfolio.Api.Services;

public class EntitlementService : IDisposable
{
    private HttpClient _client;
    private string? _authCookie;

    public EntitlementService(string _url, IHttpContextAccessor _httpAccessor)
    {
        _httpAccessor.HttpContext?.Request.Cookies.TryGetValue(AuthenticationService.AUTH_COOKIE_NAME, out _authCookie);

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
        try
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, uri);
            httpRequestMessage.Headers.Add("x-key", _authCookie);

            if (body != null)
            {
                httpRequestMessage.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            }

            HttpResponseMessage obj = await _client.SendAsync(httpRequestMessage);
            obj.EnsureSuccessStatusCode();

            return obj;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
