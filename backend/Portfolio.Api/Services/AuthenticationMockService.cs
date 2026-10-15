using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthEngineShared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Types;

namespace Portfolio.Api.Services;

public class AuthenticationMockService : IAuthenticationService
{
    private readonly byte[] _jwtSigningKey;

    private UserObject[] userList;

    public AuthenticationMockService(IOptions<SecuritySettings> settings)
    {
        _jwtSigningKey = Convert.FromBase64String(settings.Value.jwtToken);
        userList = [
            new UserObject{
                Id = Guid.NewGuid(),
                DisplayName = "Mocked Admin",
                Email = "mock_admin@gmail.com",
                Role = UserRoles.Admin
            },
            new UserObject{
                Id = Guid.NewGuid(),
                DisplayName = "Mocked",
                Email = "mock@gmail.com",
                Role = UserRoles.None
            },
        ];
    }

    public Task<string?> CreateUserEntry(string email, string displayName, string password) { return Task.FromResult<string?>(CreateJWT()); }

    public Task<UserObject[]> GetUserSummaries() => Task.FromResult(userList);

    public Task<string?> Login(string email, string password) => Task.FromResult<string?>(CreateJWT());

    public Task UpdateUser(UserObject obj)
    {
        for (int i = 0; i < userList.Length; i++)
        {
            if (userList[i].Id == obj.Id)
            {
                userList[i] = obj;
                break;
            }
        }

        return Task.CompletedTask;
    }

    public void ValidateCreation(string email, string displayName, string password, long verification) { }

    private string CreateJWT()
    {
        UserObject fakeUser = userList[0];

        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, fakeUser.Id.ToString()),
                new Claim(ClaimTypes.Email, fakeUser.Email!),
                new Claim(ClaimTypes.Name, fakeUser.DisplayName!),
                new Claim(ClaimTypes.Role, fakeUser.Role.ToString())
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
