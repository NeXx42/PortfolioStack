using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Models;

public class ReleaseModel
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public Guid ProjectId { get; set; }
    public ProjectModel? Project { get; set; } = null;

    [Required]
    public required string Version { get; set; }

    public string? Size { get; set; }

    [Required]
    public DateTime ReleaseDate { get; set; }

    public ICollection<ReleaseDownloadModel> Downloads { get; set; } = new List<ReleaseDownloadModel>();
}
