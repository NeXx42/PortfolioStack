using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Models;

public class ProjectTagModel
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Guid ProjectId { get; set; }
    public ProjectModel? project { get; set; } = null;

    [Required]
    public required int TagId { get; set; }
    public TagModel? Tag { get; set; } = null;
}
