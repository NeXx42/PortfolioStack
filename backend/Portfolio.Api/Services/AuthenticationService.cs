using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;
using Portfolio.Data;

namespace Portfolio.Api.Services;

public class AuthenticationService
{
    public const string AUTH_COOKIE_NAME = "auth_token";

    private readonly PortfolioContext _context;

    private readonly MailService _mail;
    private readonly CacheService _cache;
    private readonly EncryptionService _encryptionService;

    public AuthenticationService(PortfolioContext context, CacheService cache, MailService mail, EncryptionService encryptionService)
    {
        _encryptionService = encryptionService;

        _mail = mail;
        _cache = cache;
        _context = context;
    }

    public async Task<UserDto> CreateUserEntry(string email, string displayName, string password)
    {
        _cache.Remove(email);

        string emailHash = _encryptionService.ComputeEmailHash(email);

        if (await _context.Users.AnyAsync(x => x.emailHash.Equals(emailHash)))
        {
            throw new Exception("Email already exists");
        }

        UserModel usr = new UserModel()
        {
            displayName = _encryptionService.EncryptData(displayName),
            email = _encryptionService.EncryptData(email),

            emailHash = _encryptionService.ComputeEmailHash(email),
            passwordHash = _encryptionService.EncryptAndHashPassword(password)
        };

        await _context.Users.AddAsync(usr);
        await _context.SaveChangesAsync();

        _cache.SetIfNotExists(usr.userId.ToString(), usr);
        return _encryptionService.DecryptUserModel(usr);
    }

    public async Task<UserDto?> ConfirmLogin(string email, string password)
    {
        string emailHash = _encryptionService.ComputeEmailHash(email);

        UserModel? dbUser = await _context.Users
            .Where(x => x.emailHash.Equals(emailHash))
            .FirstOrDefaultAsync();

        if (dbUser == null)
            throw new Exception("Email not found");

        if (!_encryptionService.CheckPassword(dbUser, password))
            throw new Exception("Invalid password");

        UserDto? usr = await GetLogin(dbUser.userId);

        if (usr != null)
        {
            throw new Exception("User is already logged in");
        }

        usr = _encryptionService.DecryptUserModel(dbUser);
        _cache.SetIfNotExists(usr.id.ToString(), usr);

        return usr;
    }

    public async Task<UserDto?> GetLogin(Guid userId)
    {
        if (_cache.TryGetValue(userId.ToString(), out UserDto? usr))
        {
            return usr;
        }

        var dbUsr = await _context.Users.FirstOrDefaultAsync(u => u.userId == userId);

        if (dbUsr != null)
        {
            usr = _encryptionService.DecryptUserModel(dbUsr);
            _cache.SetIfNotExists(userId.ToString(), usr);

            return usr;
        }

        return usr;
    }

    public async Task ClearUserSession(UserDto usr)
    {
        _cache.Remove(usr.id.ToString());
    }

    public string GenerateToken(UserDto usr)
    {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usr.id.ToString()),
                new Claim(ClaimTypes.Email, usr.email),
                new Claim(ClaimTypes.Name, usr.displayName),
                new Claim( ClaimTypes.Role, usr.role.ToString())
            }),
            Audience = "portfolio",
            Issuer = "portfolio",
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = _encryptionService.GetJWTSingingCredentials()
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
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
