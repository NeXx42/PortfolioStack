using Portfolio.Core.Data;

namespace Portfolio.Core.DTOs;

public class ReleaseDTO
{
    public int versionId { get; set; }
    public string? version { get; set; }

    public string? patchNotes { get; set; }

    public ReleaseStatus status { get; set; }
    public ReleaseDownloadDto[] downloads { get; set; } = [];
}
