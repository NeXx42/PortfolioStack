using System.Security.Claims;
using Portfolio.Core.Data;

namespace Portfolio.Api.Services;

public interface IAuthenticationService
{
    public Task<string> CreateUserEntry(string email, string displayName, string password);
    public Task<string> Login(string email, string password);
    public void ValidateCreation(string email, string displayName, string password, long verification);
}
