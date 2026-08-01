using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Types;
using Portfolio.Core.Data;

namespace Portfolio.Api.Services;

public class AuthenticationMockService : IAuthenticationService
{
    private readonly byte[] _jwtSigningKey;

    public AuthenticationMockService(IOptions<SecuritySettings> settings)
    {
        _jwtSigningKey = Convert.FromBase64String(settings.Value.jwtToken);
    }

    public Task<string> CreateUserEntry(string email, string displayName, string password) { return Task.FromResult(CreateJWT()); }
    public Task<string> Login(string email, string password) => Task.FromResult(CreateJWT());

    public void ValidateCreation(string email, string displayName, string password, long verification) { }

    private string CreateJWT()
    {
        UserObject fakeUser = new UserObject()
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test",
            Email = "Test",
            role = UserRoles.Admin,
        };

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, fakeUser.Id.ToString()),
                new Claim(ClaimTypes.Email, fakeUser.Email),
                new Claim(ClaimTypes.Name, fakeUser.DisplayName),
                new Claim(ClaimTypes.Role, fakeUser.role.ToString())
            }),
            Audience = "NexxAuth",
            Issuer = "NexxAuth",
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtSigningKey), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
