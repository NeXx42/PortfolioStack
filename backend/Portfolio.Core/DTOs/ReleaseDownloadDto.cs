using Portfolio.Core.Models;

namespace Portfolio.Core.DTOs;

public class ReleaseDownloadDto
{
    public required string platform { get; set; }

    public required string downloadLink { get; set; }
    public string? releaseEngineManifestLink { get; set; }

    public string? entryPoint { get; set; }
    public long size { get; set; }

    public static ReleaseDownloadDto Map(ReleaseDownloadModel d) => new ReleaseDownloadDto()
    {
        downloadLink = d.DownloadUrl,
        platform = d.Platform,
        size = d.DownloadSize,
    };
}
