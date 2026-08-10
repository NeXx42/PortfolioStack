namespace Portfolio.Core.DTOs;

public class ReleaseDownloadDto
{
    public required string platform { get; set; }

    public required string downloadLink { get; set; }
    public string? releaseEngineManifestLink { get; set; }

    public string? entryPoint { get; set; }
    public long size { get; set; }
}
