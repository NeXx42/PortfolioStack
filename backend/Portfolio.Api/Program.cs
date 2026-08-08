using System.Security.Claims;
using System.Text;
using AuthEngineMiddleman;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Data;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"Current environment: {builder.Environment.EnvironmentName}");

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
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Encryption:JWTToken"]!)),
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue(AuthenticationService.AUTH_COOKIE_NAME, out string? cookie))
                context.Token = cookie;

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.Write(context.Exception?.Message);
            return Task.CompletedTask;
        }
    };
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
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

builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<GeneralSettings>(builder.Configuration.GetSection("Settings"));
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("Encryption"));

builder.Services.AddSingleton<CacheService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IAuthenticationService, AuthenticationMockService>();
}
else
{
    builder.Services.AddSingleton(sp =>
    {
        var options = sp.GetRequiredService<IOptions<SecuritySettings>>();
        return new AuthEngineMiddlemanService(options.Value.authServiceURL, options.Value.authServiceToken);
    });
    builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
}

builder.Services.AddScoped<ContentService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<MailService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();