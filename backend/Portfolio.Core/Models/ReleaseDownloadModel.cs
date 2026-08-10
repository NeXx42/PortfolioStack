using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Models;

public class ReleaseDownloadModel
{
    [Key]
    [Required]
    public Guid ProjectId { get; set; }

    [Key]
    [Required]
    public int ReleaseId { get; set; }
    public ReleaseModel MetaData { get; set; } = null!;

    [Key]
    [Required]
    public required string Platform { get; set; }

    [Required]
    public required string DownloadUrl { get; set; }
    public required long DownloadSize { get; set; }

    public string? EntryPoint { get; set; }
    public string? ReleaseEngineManifest { get; set; }
}
