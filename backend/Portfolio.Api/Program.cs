using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AuthEngineMiddleman;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Data;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"Current environment: {builder.Environment.EnvironmentName}");

string authUrl = builder.Configuration["Encryption:AuthServiceURL"] ?? throw new InvalidOperationException("Authservice url is not defined");
string publicKey;

using (HttpClient client = new HttpClient())
{
    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, Path.Combine(authUrl, "api", "auth", "publickey"));
    HttpResponseMessage res = await client.SendAsync(req);

    try
    {
        res.EnsureSuccessStatusCode();
    }
    catch (Exception e)
    {
        Console.WriteLine($"Failed to get public key for authservice.\n{e.Message}");
        throw;
    }

    string json = await res.Content.ReadAsStringAsync();
    publicKey = JsonDocument.Parse(json).RootElement.GetProperty("key").GetString() ?? throw new InvalidDataException("Public key from authservice is invalid");
}

RSA rsa = RSA.Create();

rsa.ImportFromPem(publicKey.Replace("\\n", "\n"));
RsaSecurityKey rsaKey = new RsaSecurityKey(rsa) { KeyId = "NexxAuth" };

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "NexxAuth",

        ValidateAudience = true,
        ValidAudience = "NexxAuth",

        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        IssuerSigningKey = rsaKey,
        ValidAlgorithms = [SecurityAlgorithms.RsaSha256],

        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.FromSeconds(30)
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue(AuthenticationService.AUTH_COOKIE_NAME, out string? cookie))
                context.Token = cookie;

            if (context.Request.Headers.TryGetValue("x-key", out StringValues token))
                context.Token = token;

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.Write(context.Exception?.Message);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("localhost", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(
            "https://nexx42.info",
            "https://www.nexx42.info",
            "https://upload.nexx42.info"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<GeneralSettings>(builder.Configuration.GetSection("Settings"));
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("Encryption"));

builder.Services.AddSingleton<CacheService>();

builder.Services.AddSingleton(sp =>
{
    var options = sp.GetRequiredService<IOptions<SecuritySettings>>();
    return new AuthEngineMiddlemanService(options.Value.authServiceURL, options.Value.authServiceToken);
});
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp => new EntitlementService(builder.Configuration["Services:LicenceEngine"]!, sp.GetRequiredService<IHttpContextAccessor>()));
builder.Services.AddScoped<ReleaseService>();
builder.Services.AddScoped<ContentService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<MailService>();

builder.Services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

builder.Services.AddMemoryCache();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PortfolioContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseCors("localhost");
}

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();