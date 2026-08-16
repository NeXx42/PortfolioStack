namespace Portfolio.Api.Types;

public class FileDownloadInfo
{
    public required string hash { get; set; }
    public required string relativePath { get; set; }
    public required string downloadUrl { get; set; }

    public long? size { get; set; }
    public long? compressedSize { get; set; }
    public required string compressionAlgorithm { get; set; }
}
