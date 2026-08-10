using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portfolio.Core.Models;

public class ReleaseModel
{
    [Required]
    public Guid ProjectId { get; set; }
    public ProjectModel Project { get; set; } = null!;

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReleaseId { get; set; }

    public string? VersionName { get; set; }
    public string? PatchNotes { get; set; }

    public ICollection<ReleaseDownloadModel> Downloads { get; set; } = new List<ReleaseDownloadModel>();
}
