using System.ComponentModel.DataAnnotations;

namespace Portfolio.Core.Models;

public class FileModel
{
    [Key]
    [Required]
    public required Guid Id { get; set; }

    public long? Size { get; set; }
    public long? CompressedSize { get; set; }
    public long? TimeUploaded { get; set; }

    [Required]
    public required string CompressionType { get; set; }

    [Required]
    public required string FileName { get; set; }

    [Required]
    public required string Hash { get; set; }
}
