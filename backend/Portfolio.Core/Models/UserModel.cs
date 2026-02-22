using System.ComponentModel.DataAnnotations;
using Portfolio.Core.Data;

namespace Portfolio.Core.Models;

public class UserModel
{
    [Key]
    public Guid userId { get; set; }

    [Required]
    public required string displayName { get; set; }

    [Required]
    public required string email { get; set; }

    [Required]
    public required string emailHash { get; set; }

    [Required]
    public required string passwordHash { get; set; }

    public UserRoles? Role { get; set; }
}
