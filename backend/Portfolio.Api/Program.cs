using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Services;
using Portfolio.Api.Types;
using Portfolio.Data;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Encryption:JWTToken"]!))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.TryGetValue(AuthenticationService.AUTH_COOKIE_NAME, out string? cookie))
            {
                context.Token = cookie;
            }

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
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<PortfolioContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.Configure<GeneralSettings>(builder.Configuration.GetSection("Settings"));
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("Encryption"));

builder.Services.AddSingleton<EncryptionService>();

builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<ContentService>();

builder.Services.AddMemoryCache();

var app = builder.Build();


app.UseCors("localhost");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();