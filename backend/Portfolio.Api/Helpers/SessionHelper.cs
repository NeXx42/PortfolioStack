using System.Security.Claims;
using AuthEngineShared;

namespace Portfolio.Api.Helpers;

public static class SessionHelper
{
    public async static Task<UserObject?> GetSessionUser(ClaimsPrincipal usr)
    {
        string? userGuid = usr.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userGuid))
            return null;

        string? displayName = usr.FindFirst(ClaimTypes.Name)?.Value;
        string? email = usr.FindFirst(ClaimTypes.Email)?.Value;
        string? role = usr.FindFirst(ClaimTypes.Role)?.Value;

        if (Guid.TryParse(userGuid, out Guid id))
        {
            UserRoles userRole = UserRoles.None;
            Enum.TryParse(role, ignoreCase: true, out userRole);

            return new UserObject()
            {
                Id = id,
                Email = email!,
                DisplayName = displayName!,
                Role = userRole,
            };
        }

        return null;
    }
}
