using Portfolio.Core.Models;

namespace Portfolio.Core.DTOs;

public class FileDto
{
    public Guid id { get; set; }
    public required string fileName { get; set; }

    public long? size { get; set; }
    public string? compression { get; set; }

    public static FileDto Map(FileModel file) => new FileDto
    {
        id = file.Id,
        fileName = file.FileName,
        size = file.Size,
        compression = file.CompressionType,
    };
}
