using AuthEngineMiddleman;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Data;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"Current environment: {builder.Environment.EnvironmentName}");

string authUrl = builder.Configuration["Encryption:AuthServiceURL"] ?? throw new InvalidOperationException("Authservice url is not defined");

AuthEngineSigningService signingService = new AuthEngineSigningService(authUrl);
await signingService.Setup();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(signingService.HandleSigning);

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