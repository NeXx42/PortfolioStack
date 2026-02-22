using Portfolio.Core.Data;

namespace Portfolio.Core.DTOs;

public class UserDto
{
    public required Guid id { get; set; }
    public required string displayName { set; get; }
    public required string email { set; get; }
    public required UserRoles role { get; set; }
}
