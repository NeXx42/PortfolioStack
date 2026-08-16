using System.ComponentModel.DataAnnotations;
using Portfolio.Core.Data;

namespace Portfolio.Core.Models;

public class ProjectModel
{
    [Key]
    public required Guid id { get; set; }

    [Required]
    public required string name { get; set; }

    [Required]
    public required string slug { get; set; }

    public string? icon { get; set; }
    public string? description { get; set; }
    public string? genre { get; set; }

    [Required]
    public required ProjectType projectType { get; set; }

    [Required]
    public required ReleaseStatus status { get; set; }

    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public ICollection<ProjectElementModel> Elements { get; set; } = new List<ProjectElementModel>();
    public ICollection<ProjectTagModel> Tags { get; set; } = new List<ProjectTagModel>();
    public ICollection<ReleaseModel> Releases { get; set; } = new List<ReleaseModel>();
}
