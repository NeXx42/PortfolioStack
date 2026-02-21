using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Models;

public class UserModel
{
    [Key]
    public Guid userId { get; set; }
    public required string displayName { get; set; }
    public required string email { get; set; }
    public required string emailHash { get; set; }
    public required string passwordHash { get; set; }
}
