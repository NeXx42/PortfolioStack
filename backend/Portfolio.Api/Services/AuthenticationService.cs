using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class AuthenticationService
{
    public const string AUTH_COOKIE_NAME = "auth_token";

    private readonly PortfolioContext _context;
    private readonly IMemoryCache _cache;
    private readonly byte[] _jwtKey;

    public AuthenticationService(PortfolioContext context, IMemoryCache cache, IConfiguration config)
    {
        _context = context;
        _cache = cache;

        _jwtKey = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "this_is_a_super_secret_key_that_is_at_least_32_bytes_long!");
    }

    public async Task<UserModel> CreateUserEntry(string email, string displayName, string password)
    {
        if (await _context.Users.AnyAsync(x => x.email.Equals(email)))
        {
            throw new Exception("Email already exists");
        }

        UserModel usr = new UserModel()
        {
            displayName = displayName,
            email = email,
            passwordHash = password
        };

        await _context.Users.AddAsync(usr);
        await _context.SaveChangesAsync();

        _cache.Set(usr.userId, usr);
        return usr;
    }

    public async Task<UserModel?> ConfirmLogin(string email, string passwordHash)
    {
        UserModel? usr = await _context.Users
            .Where(x => x.email.Equals(email) && x.passwordHash.Equals(passwordHash))
            .FirstOrDefaultAsync();

        if (usr != null)
        {
            UserModel? existingLogin = await GetLogin(usr.userId);

            if (existingLogin != null)
            {
                throw new Exception("User is already logged in");
            }

            _cache.Set(usr.userId, usr);
        }

        return usr;
    }

    public async Task<UserModel?> GetLogin(Guid userId)
    {
        if (_cache.TryGetValue(userId, out UserModel? usr))
        {
            return usr;
        }

        return null;
    }

    public async Task ClearUserSession(UserModel usr)
    {
        _cache.Remove(usr.userId);
    }

    public string GenerateToken(UserModel usr)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usr.userId.ToString()),
                new Claim(ClaimTypes.Email, usr.email),
                new Claim(ClaimTypes.Name, usr.displayName)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
