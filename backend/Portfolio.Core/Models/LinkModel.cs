using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Models;

public class LinkModel
{
    [Required]
    [Key]
    public required string Name { get; set; }

    [Required]
    public required string Url { get; set; }

    public string? Icon { get; set; }
    public string? CustomColour { get; set; }
}
