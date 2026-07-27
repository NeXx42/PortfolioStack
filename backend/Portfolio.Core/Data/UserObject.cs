namespace Portfolio.Core.Data;

public class UserObject
{
    public required Guid Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public UserRoles role { get; set; }
}
