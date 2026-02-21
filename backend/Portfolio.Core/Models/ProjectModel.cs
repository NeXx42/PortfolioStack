using System.ComponentModel.DataAnnotations;
using Portfolio.Core.Data;

namespace Portfolio.Core.Models;

public class ProjectModel
{
    [Key]
    public required Guid id { get; set; }
    public required string name { get; set; }
    public string? icon { get; set; }
    public string? description { get; set; }

    public required ProjectType projectType { get; set; }
}
